using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YouTubeStreamDownloader.Helpers;
using YouTubeStreamDownloader.Interfaces;

namespace YouTubeStreamDownloader.Services;

public class DownloadSubtitleService : IDownloadSubtitleService
{
  private readonly YoutubeClient _youtubeClient = new();

	public async Task<string> GetSubtitleAsync(string videoUrl, string fileName, string outputPath, string? languageCode, CancellationToken cancellationToken = default)
  {
    var trackManifest = await _youtubeClient.Videos.ClosedCaptions.GetManifestAsync(videoUrl, cancellationToken);
    var sanitizedTitle = FileHelper.SanitizeFileName(fileName);
    var langCode = languageCode ?? "en";
		var trackInfo = trackManifest.TryGetByLanguage(languageCode ?? "en");

    if (trackInfo is null)
    {
			return string.Empty;
		}

    string filePath = Path.Combine(outputPath, $"{sanitizedTitle}-{langCode}.srt");
    string? directoryPath = Path.GetDirectoryName(filePath);
    if (!string.IsNullOrEmpty(directoryPath))
    {
      Directory.CreateDirectory(directoryPath);
    }

		await _youtubeClient.Videos.ClosedCaptions.DownloadAsync(trackInfo, filePath, cancellationToken: cancellationToken);

    return fileName;
  }

  public async Task<List<string>> GetAllSubtitlesAsync(string videoUrl, string fileName, string outputPath, CancellationToken cancellationToken = default)
  {
    var trackManifest = await _youtubeClient.Videos.ClosedCaptions.GetManifestAsync(videoUrl, cancellationToken);
    var trackInfos = trackManifest.Tracks;

    var result = new List<string>();
    foreach (var trackInfo in trackInfos)
    {
      var path = await GetSubtitleAsync(videoUrl, fileName, outputPath, trackInfo.Language.Code, cancellationToken);
      result.Add(path);
		}

    return result;
  }

	public async Task<string> GetSubtitleAsync(string videoUrl, string? languageCode, CancellationToken cancellationToken = default)
  {
    var trackManifest = await _youtubeClient.Videos.ClosedCaptions.GetManifestAsync(videoUrl, cancellationToken);
    var trackInfo = trackManifest.TryGetByLanguage(languageCode ?? "en");

    if (trackInfo is null)
    {
      return string.Empty;
    }

    var subtitleContent = await _youtubeClient.Videos.ClosedCaptions.GetAsync(trackInfo, cancellationToken);

    return subtitleContent.Captions.ToString() ?? string.Empty;
  }
}
