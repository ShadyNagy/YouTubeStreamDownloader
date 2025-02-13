using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YouTubeStreamDownloader;

public static class YouTubeParser
{
  private static readonly HttpClient httpClient = new HttpClient(new HttpClientHandler
  {
    AutomaticDecompression = DecompressionMethods.All
  });

  public static async Task<YouTubeVideoInfo> GetVideoInfoAsync(string videoUrl)
  {
    var videoId = ExtractVideoId(videoUrl);
    var html = await FetchVideoPage(videoUrl);
    var json = ExtractInitialPlayerResponse(html);
    return ParseVideoInfo(json, videoId);
  }

  private static string ExtractVideoId(string url)
  {
    var match = Regex.Match(url, @"(?:v=|\/)([a-zA-Z0-9_-]{11})");
    return match.Success ? match.Groups[1].Value : string.Empty;
  }

  private static async Task<string> FetchVideoPage(string url)
  {
    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
    return await httpClient.GetStringAsync(url);
  }

  private static string ExtractInitialPlayerResponse(string html)
  {
    var pattern = @"var\s+ytInitialPlayerResponse\s*=\s*({.*?});\s*var\s+meta";
    var match = Regex.Match(html, pattern, RegexOptions.Singleline);
    return match.Success ? match.Groups[1].Value : string.Empty;
	}

  private static YouTubeVideoInfo ParseVideoInfo(string json, string videoId)
  {
    using var doc = JsonDocument.Parse(json);
    var root = doc.RootElement;

    var videoInfo = new YouTubeVideoInfo
    {
      VideoId = videoId,
      Title = root.GetProperty("videoDetails").GetProperty("title").GetString()?? string.Empty,
      Description = root.GetProperty("videoDetails").GetProperty("shortDescription").GetString() ?? string.Empty
		};

    var formats = new List<MediaFormat>();
    ParseFormats(root.GetProperty("streamingData").GetProperty("formats"), formats);
    ParseFormats(root.GetProperty("streamingData").GetProperty("adaptiveFormats"), formats);

    videoInfo.MediaFormats = formats;
    return videoInfo;
  }

  private static void ParseFormats(JsonElement formatsElement, List<MediaFormat> formats)
  {
    foreach (var format in formatsElement.EnumerateArray())
    {
      var url = format.TryGetProperty("url", out var urlElement)
        ? urlElement.GetString()
        : null;

      // Skip formats that require signature deciphering
      if (url == null || url.Contains("signature") || url.Contains("sig"))
        continue;

      var formatInfo = new MediaFormat
      {
        Url = url,
        MimeType = format.GetProperty("mimeType").GetString() ?? string.Empty,
        Bitrate = format.TryGetProperty("bitrate", out var bitrate)
          ? bitrate.GetInt64()
          : 0
      };

      if (format.TryGetProperty("qualityLabel", out var qualityLabel))
        formatInfo.QualityLabel = qualityLabel.GetString() ?? string.Empty;

      if (format.TryGetProperty("audioQuality", out var audioQuality))
        formatInfo.AudioQuality = audioQuality.GetString() ?? string.Empty;

      formats.Add(formatInfo);
    }
  }
}
