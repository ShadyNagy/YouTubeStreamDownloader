using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Channels;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos.Streams;
using YouTubeStreamDownloader.Interfaces;
using YouTubeStreamDownloader.Models;

namespace YouTubeStreamDownloader.Services;

public class YouTubeMetadataService : IYouTubeMetadataService
{
  private readonly YoutubeClient _youtubeClient;

  public YouTubeMetadataService(YoutubeClient? youtubeClient)
  {
    _youtubeClient = youtubeClient ?? new YoutubeClient();
  }

  public async Task<VideoData> GetVideoInfoAsync(string videoUrl, CancellationToken cancellationToken = default)
  {
    try
    {
      var videoInfo = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);

      return new VideoData
      {
        Title = videoInfo.Title,
        Url = $"https://www.youtube.com/playlist?list={videoInfo.Id}",
        Author = videoInfo.Author.ChannelTitle,
        Duration = videoInfo.Duration == null? string.Empty: videoInfo.Duration.ToString()!
			};
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error fetching playlists: {ex.Message}", ex);
    }
  }

	/// <summary>
	/// Gets all playlists from a YouTube channel.
	/// </summary>
	public async Task<List<PlaylistData>> GetAllPlaylistsAsync(
      string channelUrl, CancellationToken cancellationToken = default)
  {
    try
    {
      var channel = await _youtubeClient.Channels.GetAsync(ChannelHandle.Parse(channelUrl).Value, cancellationToken);
      var playlists = await _youtubeClient.Channels.GetUploadsAsync(channel.Id, cancellationToken);

      return playlists.Select(p => new PlaylistData
      {
        Title = p.Title,
        Url = $"https://www.youtube.com/playlist?list={p.Id}"
      }).ToList();
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error fetching playlists: {ex.Message}", ex);
    }
  }

  /// <summary>
  /// Gets all video links and titles from a given playlist.
  /// </summary>
  public async Task<List<VideoData>> GetAllVideosFromPlaylistAsync(
      string playlistUrl, CancellationToken cancellationToken = default)
  {
    try
    {
      var playlistId = PlaylistId.Parse(playlistUrl);
      var videos = await _youtubeClient.Playlists.GetVideosAsync(playlistId, cancellationToken);

      return videos.Select(v => new VideoData
      {
        Title = v.Title,
        Url = $"https://www.youtube.com/watch?v={v.Id}"
      }).ToList();
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error fetching videos from playlist: {ex.Message}", ex);
    }
  }

	/// <summary>
	/// Downloads a YouTube video as a file.
	/// </summary>
	public async Task<string> DownloadVideoAsFileAsync(
			string videoUrl, string outputPath, CancellationToken cancellationToken = default)
	{
		try
		{
			var video = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
			var sanitizedTitle = SanitizeFileName(video.Title);
			var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
			var streamInfo = streamManifest.GetVideoStreams().TryGetWithHighestVideoQuality();

			if (streamInfo == null)
				throw new InvalidOperationException("No suitable video stream found.");

			string filePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp4");
      string? directoryPath = Path.GetDirectoryName(filePath);
      if (!string.IsNullOrEmpty(directoryPath))
      {
        Directory.CreateDirectory(directoryPath);
      }

			await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, cancellationToken: cancellationToken);

			return filePath;
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Error downloading video as file: {ex.Message}", ex);
		}
	}

  public async Task<string> DownloadVideoWithSubtitlesAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = SanitizeFileName(video.Title);
      var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      IEnumerable<IVideoStreamInfo> streamInfos = streamManifest.GetVideoStreams();
      if (streamInfos == null)
        throw new InvalidOperationException("No suitable video stream found.");

			var streamInfo = VideoTypeEngine.GetMp4ByVideoType(VideoType.Q1080, streamInfos);
      if (streamInfo == null)
      {
        streamInfo = VideoTypeEngine.GetMp4ByVideoType(VideoType.Q720, streamInfos);
        if (streamInfo == null)
        {
          streamInfo = VideoTypeEngine.GetMp4ByVideoType(VideoType.Q480, streamInfos);
				}
			}
      string filePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp4");
      string? directoryPath = Path.GetDirectoryName(filePath);
      if (!string.IsNullOrEmpty(directoryPath))
      {
        Directory.CreateDirectory(directoryPath);
      }

      await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, cancellationToken: cancellationToken);

      _ = await GetAllSubtitlesAsync(videoUrl, sanitizedTitle, outputPath, cancellationToken);

			return filePath;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading video as file: {ex.Message}", ex);
    }
  }

	public async Task<string> DownloadHighestVideoOnlyAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = SanitizeFileName(video.Title);
      var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetVideoOnlyStreams().TryGetWithHighestVideoQuality();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable video stream found.");

      string filePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp4");
      string? directoryPath = Path.GetDirectoryName(filePath);
      if (!string.IsNullOrEmpty(directoryPath))
      {
        Directory.CreateDirectory(directoryPath);
      }

      await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, cancellationToken: cancellationToken);

      return filePath;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading video as file: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadVideoOnlyAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = SanitizeFileName(video.Title);
      var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
			IEnumerable<IVideoStreamInfo> streamInfos = streamManifest.GetVideoOnlyStreams();
      if (streamInfos == null)
        throw new InvalidOperationException("No suitable video stream found.");

      var streamInfo = VideoTypeEngine.GetMp4ByVideoType(VideoType.Q1080, streamInfos);
      if (streamInfo == null)
      {
        streamInfo = VideoTypeEngine.GetMp4ByVideoType(VideoType.Q720, streamInfos);
        if (streamInfo == null)
        {
          streamInfo = VideoTypeEngine.GetMp4ByVideoType(VideoType.Q480, streamInfos);
        }
      }

			if (streamInfo == null)
        throw new InvalidOperationException("No suitable video stream found.");

      string filePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp4");
      string? directoryPath = Path.GetDirectoryName(filePath);
      if (!string.IsNullOrEmpty(directoryPath))
      {
        Directory.CreateDirectory(directoryPath);
      }

      await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, cancellationToken: cancellationToken);

      return filePath;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading video as file: {ex.Message}", ex);
    }
  }

	public async Task<string> DownloadAudioOnlyAsFileAsync(
    string videoUrl, string outputPath, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = SanitizeFileName(video.Title);
      var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable audio stream found.");

      string filePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp3");
      string? directoryPath = Path.GetDirectoryName(filePath);
      if (!string.IsNullOrEmpty(directoryPath))
      {
        Directory.CreateDirectory(directoryPath);
      }

      await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, cancellationToken: cancellationToken);

      return filePath;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading audio as file: {ex.Message}", ex);
    }
  }

	public async Task<string> DownloadVideoAsFileAsync(
    string videoUrl, string fileName, string outputPath, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = SanitizeFileName(fileName);
			var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetVideoStreams().TryGetWithHighestVideoQuality();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable video stream found.");

      string filePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp4");
      string? directoryPath = Path.GetDirectoryName(filePath);
      if (!string.IsNullOrEmpty(directoryPath))
      {
        Directory.CreateDirectory(directoryPath);
      }

      await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, cancellationToken: cancellationToken);

      return filePath;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading video as file: {ex.Message}", ex);
    }
  }

	/// <summary>
	/// Streams a YouTube video directly.
	/// </summary>
	public async Task<Stream> DownloadVideoAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default)
	{
		try
		{
			var video = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
			var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
			var streamInfo = streamManifest.GetVideoStreams().TryGetWithHighestVideoQuality();

			if (streamInfo == null)
				throw new InvalidOperationException("No suitable video stream found.");

			return await _youtubeClient.Videos.Streams.GetAsync(streamInfo, cancellationToken);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Error streaming video: {ex.Message}", ex);
		}
	}

  public async Task<Stream> DownloadVideoOnlyAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetVideoOnlyStreams().TryGetWithHighestVideoQuality();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable video stream found.");

      return await _youtubeClient.Videos.Streams.GetAsync(streamInfo, cancellationToken);
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error streaming video: {ex.Message}", ex);
    }
  }

  public async Task<Stream> DownloadAudioOnlyAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable audio stream found.");

      return await _youtubeClient.Videos.Streams.GetAsync(streamInfo, cancellationToken);
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error streaming audio: {ex.Message}", ex);
    }
  }

	public async Task<string> GetSubtitleAsync(string videoUrl, string fileName, string outputPath, string? languageCode, CancellationToken cancellationToken = default)
  {
    var trackManifest = await _youtubeClient.Videos.ClosedCaptions.GetManifestAsync(videoUrl, cancellationToken);
    var sanitizedTitle = SanitizeFileName(fileName);
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

	public string SanitizeFileName(string name)
	{
		foreach (char c in Path.GetInvalidFileNameChars())
		{
			name = name.Replace(c, '_');
		}
		return name;
	}
}
