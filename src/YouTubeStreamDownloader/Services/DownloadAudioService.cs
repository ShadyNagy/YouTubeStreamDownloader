using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YouTubeStreamDownloader.Helpers;
using YouTubeStreamDownloader.Interfaces;

namespace YouTubeStreamDownloader.Services;

public class DownloadAudioService(YoutubeClient youtubeClient) : IDownloadAudioService
{
  public async Task<string> DownloadAudioWithProgressAsync(
    string videoUrl,
    string fileName,
    string outputPath,
    IProgress<double>? progress,
    CancellationToken cancellationToken = default)
  {
    try
    {
      if (!Uri.IsWellFormedUriString(videoUrl, UriKind.Absolute))
        throw new ArgumentException("Invalid YouTube URL.");

      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable audio stream found.");

      string tempFilePath = Path.Combine(outputPath, $"{fileName}.mp3");

      await youtubeClient.Videos.Streams.DownloadAsync(
        streamInfo,
        tempFilePath,
        new Progress<double>(p => progress?.Report(p)),
        cancellationToken
      );

      return tempFilePath;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading audio with progress: {ex.Message}", ex);
    }
  }
  public async Task<string> DownloadAudioWithProgressAsync(
    string videoUrl,
    string outputPath,
    IProgress<double>? progress,
    CancellationToken cancellationToken = default)
  {
    try
    {
      if (!Uri.IsWellFormedUriString(videoUrl, UriKind.Absolute))
        throw new ArgumentException("Invalid YouTube URL.");

      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = FileHelper.SanitizeFileName(video.Title);
      var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable audio stream found.");

      string tempFilePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp3");

      await youtubeClient.Videos.Streams.DownloadAsync(
        streamInfo,
        tempFilePath,
        new Progress<double>(p => progress?.Report(p)),
        cancellationToken
      );

      return tempFilePath;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading audio with progress: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadAudioWithProgressAsync(
    string videoUrl,
    IProgress<double>? progress,
    CancellationToken cancellationToken = default)
  {
    try
    {
      if (!Uri.IsWellFormedUriString(videoUrl, UriKind.Absolute))
        throw new ArgumentException("Invalid YouTube URL.");

      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable audio stream found.");

      string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp3");

      await youtubeClient.Videos.Streams.DownloadAsync(
        streamInfo,
        tempFilePath,
        new Progress<double>(p => progress?.Report(p)),
        cancellationToken
      );

      return tempFilePath;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading audio with progress: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadAudioAsFileAsync(string videoUrl, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default)
  {
    try
    {
      if (!Uri.IsWellFormedUriString(videoUrl, UriKind.Absolute))
        throw new ArgumentException("Invalid YouTube URL.");

      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = FileHelper.SanitizeFileName(video.Title);
      var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable audio stream found.");

      string filePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp3");
      string? directoryPath = Path.GetDirectoryName(filePath);
      if (!string.IsNullOrEmpty(directoryPath))
      {
        Directory.CreateDirectory(directoryPath);
      }

      await youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, progress: new Progress<double>(p => progress?.Report(p)), cancellationToken: cancellationToken);

      return filePath;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading audio as file: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadAudioAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default)
  {
    try
    {
      if (!Uri.IsWellFormedUriString(videoUrl, UriKind.Absolute))
        throw new ArgumentException("Invalid YouTube URL.");

      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = FileHelper.SanitizeFileName(video.Title);
      var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable audio stream found.");

      string filePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp3");
      string? directoryPath = Path.GetDirectoryName(filePath);
      if (!string.IsNullOrEmpty(directoryPath))
      {
        Directory.CreateDirectory(directoryPath);
      }

      await youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, cancellationToken: cancellationToken);

      return filePath;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading audio as file: {ex.Message}", ex);
    }
  }

  public async Task<Stream> DownloadAudioAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default)
  {
    try
    {
      if (!Uri.IsWellFormedUriString(videoUrl, UriKind.Absolute))
        throw new ArgumentException("Invalid YouTube URL.");

      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable audio stream found.");

      return await youtubeClient.Videos.Streams.GetAsync(streamInfo, cancellationToken);
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error streaming audio: {ex.Message}", ex);
    }
  }
}
