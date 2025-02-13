using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YouTubeStreamDownloader.Helpers;
using YouTubeStreamDownloader.Interfaces;

namespace YouTubeStreamDownloader.Services;

public class DownloadAudioService : IDownloadAudioService
{
  private readonly YoutubeClient _youtubeClient = new();

	public async Task<string> DownloadAudioOnlyAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = FileHelper.SanitizeFileName(video.Title);
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
}
