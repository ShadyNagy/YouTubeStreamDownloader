using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YouTubeStreamDownloader.Helpers;
using YouTubeStreamDownloader.Interfaces;
using YouTubeStreamDownloader.Models;
using YouTubeStreamDownloader.VideoMerger.Interfaces;

namespace YouTubeStreamDownloader.Services;

public class DownloadVideoService(YoutubeClient youtubeClient, IDownloadSubtitleService downloadSubtitleService, IVideoMerger videoMerger, IDownloadAudioService downloadAudioService) : IDownloadVideoService
{
  public async Task<string> DownloadVideoWithProgressAndMergeAsync(
    string videoUrl,
    string outputPath,
    VideoType quality,
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
      var streamInfos = streamManifest.GetVideoStreams();
      var streamInfo = VideoTypeEngine.GetMp4ByVideoType(quality, streamInfos);

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable video stream found.");

      string videoFilePath = Path.Combine(outputPath, $"{Guid.NewGuid()}_video.mp4");
      string audioFilePath = Path.Combine(outputPath, $"{Guid.NewGuid()}_audio.mp3");
      string mergedOutput = Path.Combine(outputPath, $"{sanitizedTitle}.mkv");

      await youtubeClient.Videos.Streams.DownloadAsync(
        streamInfo,
        videoFilePath,
        new Progress<double>(p =>
        {
          progress?.Report(p);
        }),
        cancellationToken: cancellationToken);

      if (!File.Exists(audioFilePath))
      {
        audioFilePath = await downloadAudioService.DownloadAudioAsFileAsync(videoUrl, audioFilePath, cancellationToken);
      }

      await videoMerger.MergeAudioAndVideoWithoutEncodeAsync(videoFilePath, audioFilePath, mergedOutput);

      File.Delete(videoFilePath);
      File.Delete(audioFilePath);

      return mergedOutput;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading and merging video with progress: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadVideoWithProgressAndMergeAsync(
    string videoUrl,
    VideoType quality,
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
      var streamInfos = streamManifest.GetVideoStreams();
      var streamInfo = VideoTypeEngine.GetMp4ByVideoType(quality, streamInfos);

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable video stream found.");

      string tempDir = Path.GetTempPath();
      string videoFilePath = Path.Combine(tempDir, $"{Guid.NewGuid()}_video.mp4");
      string audioFilePath = Path.Combine(tempDir, $"{Guid.NewGuid()}_audio.mp3");
      string mergedOutput = Path.Combine(tempDir, $"{sanitizedTitle}.mkv");

      await youtubeClient.Videos.Streams.DownloadAsync(
          streamInfo,
          videoFilePath,
          new Progress<double>(p =>
          {
            progress?.Report(p);
          }),
          cancellationToken: cancellationToken);

      if (!File.Exists(audioFilePath))
      {
        audioFilePath = await downloadAudioService.DownloadAudioAsFileAsync(videoUrl, audioFilePath, cancellationToken);
      }

      await videoMerger.MergeAudioAndVideoWithoutEncodeAsync(videoFilePath, audioFilePath, mergedOutput);

      File.Delete(videoFilePath);
      File.Delete(audioFilePath);

      return mergedOutput;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading and merging video with progress: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadVideoAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default)
	{
		try
		{
			var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
			var sanitizedTitle = FileHelper.SanitizeFileName(video.Title);
			var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
			var streamInfo = streamManifest.GetVideoStreams().TryGetWithHighestVideoQuality();

			if (streamInfo == null)
				throw new InvalidOperationException("No suitable video stream found.");

			string filePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp4");
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
			throw new InvalidOperationException($"Error downloading video as file: {ex.Message}", ex);
		}
	}

  public async Task<string> DownloadVideoAsFileAsync(string videoUrl, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = FileHelper.SanitizeFileName(video.Title);
      var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetVideoStreams().TryGetWithHighestVideoQuality();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable video stream found.");

      string filePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp4");
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
      throw new InvalidOperationException($"Error downloading video as file: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadVideoWithSubtitlesAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = FileHelper.SanitizeFileName(video.Title);
      var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
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

      await youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, cancellationToken: cancellationToken);

      _ = await downloadSubtitleService.GetAllSubtitlesAsync(videoUrl, sanitizedTitle, outputPath, cancellationToken);

			return filePath;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading video as file: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadVideoWithSubtitlesAsFileAsync(string videoUrl, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = FileHelper.SanitizeFileName(video.Title);
      var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
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

      await youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, progress: new Progress<double>(p => progress?.Report(p)), cancellationToken: cancellationToken);

      _ = await downloadSubtitleService.GetAllSubtitlesAsync(videoUrl, sanitizedTitle, outputPath, cancellationToken);

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
      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = FileHelper.SanitizeFileName(video.Title);
      var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetVideoOnlyStreams().TryGetWithHighestVideoQuality();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable video stream found.");

      string filePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp4");
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
      throw new InvalidOperationException($"Error downloading video as file: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadHighestVideoOnlyAsFileAsync(string videoUrl, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = FileHelper.SanitizeFileName(video.Title);
      var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetVideoOnlyStreams().TryGetWithHighestVideoQuality();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable video stream found.");

      string filePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp4");
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
      throw new InvalidOperationException($"Error downloading video as file: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadVideoOnlyAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = FileHelper.SanitizeFileName(video.Title);
      var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
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

      await youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, cancellationToken: cancellationToken);

      return filePath;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading video as file: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadVideoOnlyAsFileAsync(string videoUrl, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = FileHelper.SanitizeFileName(video.Title);
      var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
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

      await youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, progress: new Progress<double>(p => progress?.Report(p)), cancellationToken: cancellationToken);

      return filePath;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading video as file: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadVideoAsFileAsync(string videoUrl, string fileName, string outputPath, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = FileHelper.SanitizeFileName(fileName);
			var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetVideoStreams().TryGetWithHighestVideoQuality();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable video stream found.");

      string filePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp4");
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
      throw new InvalidOperationException($"Error downloading video as file: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadVideoAsFileAsync(string videoUrl, string fileName, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default)
  {
    try
    {
      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var sanitizedTitle = FileHelper.SanitizeFileName(fileName);
      var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetVideoStreams().TryGetWithHighestVideoQuality();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable video stream found.");

      string filePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp4");
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
      throw new InvalidOperationException($"Error downloading video as file: {ex.Message}", ex);
    }
  }

  public async Task<Stream> DownloadVideoAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default)
	{
		try
		{
			var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
			var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
			var streamInfo = streamManifest.GetVideoStreams().TryGetWithHighestVideoQuality();

			if (streamInfo == null)
				throw new InvalidOperationException("No suitable video stream found.");

			return await youtubeClient.Videos.Streams.GetAsync(streamInfo, cancellationToken);
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
      var video = await youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = streamManifest.GetVideoOnlyStreams().TryGetWithHighestVideoQuality();

      if (streamInfo == null)
        throw new InvalidOperationException("No suitable video stream found.");

      return await youtubeClient.Videos.Streams.GetAsync(streamInfo, cancellationToken);
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error streaming video: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadAndMergeVideoWithAudioAsFileAsync(string videoUrl, string fileName, string outputPath, CancellationToken cancellationToken = default)
  {
    try
    {
      if (!Directory.Exists(outputPath))
        Directory.CreateDirectory(outputPath);

      string downloadedVideo = await DownloadVideoOnlyAsFileAsync(videoUrl, outputPath, cancellationToken);
      string downloadedAudio = await downloadAudioService.DownloadAudioAsFileAsync(videoUrl, outputPath, cancellationToken);
      var sanitizedTitle = FileHelper.SanitizeFileName(fileName);

      var nameGuid = Guid.NewGuid().ToString();
      string mergedOutput = Path.Combine(outputPath, $"{nameGuid}.mkv");

      await videoMerger.MergeAudioAndVideoWithoutEncodeAsync(downloadedVideo, downloadedAudio, mergedOutput);
      RenameAndRemoveOld(downloadedVideo, downloadedAudio, mergedOutput);

      return mergedOutput;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading and merging video: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadAndMergeVideoWithAudioAsFileAsync(string videoUrl, string fileName, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default)
  {
    try
    {
      if (!Directory.Exists(outputPath))
        Directory.CreateDirectory(outputPath);

      string downloadedVideo = await DownloadVideoOnlyAsFileAsync(videoUrl, outputPath, progress, cancellationToken);
      string downloadedAudio = await downloadAudioService.DownloadAudioAsFileAsync(videoUrl, outputPath, progress, cancellationToken);
      var sanitizedTitle = FileHelper.SanitizeFileName(fileName);

      var nameGuid = Guid.NewGuid().ToString();
      string mergedOutput = Path.Combine(outputPath, $"{nameGuid}.mkv");

      await videoMerger.MergeAudioAndVideoWithoutEncodeAsync(downloadedVideo, downloadedAudio, mergedOutput);
      RenameAndRemoveOld(downloadedVideo, downloadedAudio, mergedOutput);

      return mergedOutput;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading and merging video: {ex.Message}", ex);
    }
  }

  public async Task<string> DownloadAndMergeVideoWithAudioAllSubtitlesAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default)
  {
    try
    {
      if (!Directory.Exists(outputPath))
        Directory.CreateDirectory(outputPath);

      string downloadedVideo = await DownloadVideoOnlyAsFileAsync(videoUrl, outputPath, cancellationToken);
      string downloadedAudio = await downloadAudioService.DownloadAudioAsFileAsync(videoUrl, outputPath, cancellationToken);

      var parts = downloadedVideo.Split(Path.DirectorySeparatorChar);
      var fileName = parts[^1];
      string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
      var sanitizedTitle = FileHelper.SanitizeFileName(fileName);
      var nameGuid = Guid.NewGuid().ToString();
      string mergedOutput = Path.Combine(outputPath, $"{nameGuid}.mkv");

      await videoMerger.MergeAudioAndVideoWithoutEncodeAsync(downloadedVideo, downloadedAudio, mergedOutput);
      RenameAndRemoveOld(downloadedVideo, downloadedAudio, mergedOutput);

      _ = await downloadSubtitleService.GetAllSubtitlesAsync(videoUrl, fileNameWithoutExtension, outputPath, cancellationToken);

      return mergedOutput;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading and merging video: {ex.Message}", ex);
    }
  }
  public async Task<string> DownloadAndMergeVideoWithAudioAllSubtitlesAsFileAsync(string videoUrl, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default)
  {
    try
    {
      if (!Directory.Exists(outputPath))
        Directory.CreateDirectory(outputPath);

      string downloadedVideo = await DownloadVideoOnlyAsFileAsync(videoUrl, outputPath, progress, cancellationToken);
      string downloadedAudio = await downloadAudioService.DownloadAudioAsFileAsync(videoUrl, outputPath, progress, cancellationToken);

      var parts = downloadedVideo.Split(Path.DirectorySeparatorChar);
      var fileName = parts[^1];
      string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
      var sanitizedTitle = FileHelper.SanitizeFileName(fileName);
      var nameGuid = Guid.NewGuid().ToString();
      string mergedOutput = Path.Combine(outputPath, $"{nameGuid}.mkv");

      await videoMerger.MergeAudioAndVideoWithoutEncodeAsync(downloadedVideo, downloadedAudio, mergedOutput);
      RenameAndRemoveOld(downloadedVideo, downloadedAudio, mergedOutput);

      _ = await downloadSubtitleService.GetAllSubtitlesAsync(videoUrl, fileNameWithoutExtension, outputPath, cancellationToken);

      return mergedOutput;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading and merging video: {ex.Message}", ex);
    }
  }


  public async Task<string> DownloadAndMergeVideoWithAudioAllSubtitlesAsFileAsync(string videoUrl, string fileName, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default)
  {
    try
    {
      if (!Directory.Exists(outputPath))
        Directory.CreateDirectory(outputPath);

      string downloadedVideo = await DownloadVideoOnlyAsFileAsync(videoUrl, outputPath, progress, cancellationToken);
      string downloadedAudio = await downloadAudioService.DownloadAudioAsFileAsync(videoUrl, outputPath, progress, cancellationToken);

      var sanitizedTitle = FileHelper.SanitizeFileName(fileName);
      var nameGuid = Guid.NewGuid().ToString();
      string mergedOutput = Path.Combine(outputPath, $"{nameGuid}.mkv");

      await videoMerger.MergeAudioAndVideoWithoutEncodeAsync(downloadedVideo, downloadedAudio, mergedOutput);
      RenameAndRemoveOld(downloadedVideo, downloadedAudio, mergedOutput);

      _ = await downloadSubtitleService.GetAllSubtitlesAsync(videoUrl, sanitizedTitle, outputPath, cancellationToken);

      return mergedOutput;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading and merging video: {ex.Message}", ex);
    }
  }
  public async Task<string> DownloadAndMergeVideoWithAudioAllSubtitlesAsFileAsync(string videoUrl, string fileName, string outputPath, CancellationToken cancellationToken = default)
  {
    try
    {
      if (!Directory.Exists(outputPath))
        Directory.CreateDirectory(outputPath);

      string downloadedVideo = await DownloadVideoOnlyAsFileAsync(videoUrl, outputPath, cancellationToken);
      string downloadedAudio = await downloadAudioService.DownloadAudioAsFileAsync(videoUrl, outputPath, cancellationToken);

      var sanitizedTitle = FileHelper.SanitizeFileName(fileName);
      var nameGuid = Guid.NewGuid().ToString();
      string mergedOutput = Path.Combine(outputPath, $"{nameGuid}.mkv");

      await videoMerger.MergeAudioAndVideoWithoutEncodeAsync(downloadedVideo, downloadedAudio, mergedOutput);
      RenameAndRemoveOld(downloadedVideo, downloadedAudio, mergedOutput);

      _ = await downloadSubtitleService.GetAllSubtitlesAsync(videoUrl, sanitizedTitle, outputPath, cancellationToken);

      return mergedOutput;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading and merging video: {ex.Message}", ex);
    }
  }

  public async Task<byte[]> DownloadAndMergeVideoWithAudioAsync(string videoUrl, CancellationToken cancellationToken = default)
  {
    try
    {
      var outputPath = "temp";

      if (!Directory.Exists(outputPath))
        Directory.CreateDirectory(outputPath);

      string downloadedVideo = await DownloadVideoOnlyAsFileAsync(videoUrl, outputPath, cancellationToken);
      string downloadedAudio = await downloadAudioService.DownloadAudioAsFileAsync(videoUrl, outputPath, cancellationToken);

      var parts = downloadedVideo.Split(Path.PathSeparator);
      var sanitizedTitle = FileHelper.SanitizeFileName(parts[^1]);
      string mergedOutput = Path.Combine(outputPath, $"{sanitizedTitle}.mkv");

      await videoMerger.MergeAudioAndVideoWithoutEncodeAsync(downloadedVideo, downloadedAudio, mergedOutput);

      return await File.ReadAllBytesAsync(mergedOutput, cancellationToken);
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading and merging video: {ex.Message}", ex);
    }
  }

  private void RenameAndRemoveOld(string downloadedVideo, string downloadedAudio, string tempFileName)
  {
    File.Delete(downloadedVideo);
    File.Delete(downloadedAudio);

    var parts = downloadedVideo.Split(Path.PathSeparator);
    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(parts[^1]);
    var fileName = FileHelper.SanitizeFileName(fileNameWithoutExtension);
    var outputPath = Path.GetDirectoryName(downloadedVideo);
    string mergedOutputDes = Path.Combine(outputPath, $"{fileName}.mkv");
    File.Delete(mergedOutputDes);
    File.Move(tempFileName, mergedOutputDes);
  }
}
