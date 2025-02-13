using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YouTubeStreamDownloader.Interfaces;
using YouTubeStreamDownloader.VideoMerger.Interfaces;

namespace YouTubeStreamDownloader.VideoMerger.Services;

public class ExtendedYouTubeService(IVideoMerger videoMerger, IYouTubeMetadataService youTubeMetadataService)
  : IExtendedYouTubeService
{
  /// <summary>
	/// Downloads a YouTube video and merges it with the given audio file.
	/// </summary>
	public async Task<string> DownloadAndMergeVideoWithAudioAsFileAsync(
			string videoUrl,
			string fileName,
			string outputPath,
			CancellationToken cancellationToken = default)
	{
		try
		{
			// Ensure output directory exists
			if (!Directory.Exists(outputPath))
				Directory.CreateDirectory(outputPath);

			// Download video and audio
			string downloadedVideo = await youTubeMetadataService.DownloadVideoOnlyAsFileAsync(videoUrl, outputPath, cancellationToken);
      string downloadedAudio = await youTubeMetadataService.DownloadAudioOnlyAsFileAsync(videoUrl, outputPath, cancellationToken);
      var sanitizedTitle = youTubeMetadataService.SanitizeFileName(fileName);

      var nameGuid = Guid.NewGuid().ToString();
      string mergedOutput = Path.Combine(outputPath, $"{nameGuid}.mkv");

      // Merge audio and video
      await videoMerger.MergeAudioAndVideoWithoutEncodeAsync(downloadedVideo, downloadedAudio, mergedOutput);
      RenameAndRemoveOld(downloadedVideo, downloadedAudio, mergedOutput);

      return mergedOutput;
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Error downloading and merging video: {ex.Message}", ex);
		}
	}
  
  public async Task<string> DownloadAndMergeVideoWithAudioAllSubtitlesAsFileAsync(
			string videoUrl,
			string outputPath,
			CancellationToken cancellationToken = default)
	{
		try
		{
			// Ensure output directory exists
			if (!Directory.Exists(outputPath))
				Directory.CreateDirectory(outputPath);

			// Download video and audio
			string downloadedVideo = await youTubeMetadataService.DownloadVideoOnlyAsFileAsync(videoUrl, outputPath, cancellationToken);
      string downloadedAudio = await youTubeMetadataService.DownloadAudioOnlyAsFileAsync(videoUrl, outputPath, cancellationToken);

      var parts = downloadedVideo.Split(Path.DirectorySeparatorChar);
      var fileName = parts[^1];
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			var sanitizedTitle = youTubeMetadataService.SanitizeFileName(fileName);
      var nameGuid = Guid.NewGuid().ToString();
      string mergedOutput = Path.Combine(outputPath, $"{nameGuid}.mkv");

			// Merge audio and video
			await videoMerger.MergeAudioAndVideoWithoutEncodeAsync(downloadedVideo, downloadedAudio, mergedOutput);
      RenameAndRemoveOld(downloadedVideo, downloadedAudio, mergedOutput);

      _ = await youTubeMetadataService.GetAllSubtitlesAsync(videoUrl, fileNameWithoutExtension, outputPath, cancellationToken);

			return mergedOutput;
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Error downloading and merging video: {ex.Message}", ex);
		}
	}


  public async Task<string> DownloadAndMergeVideoWithAudioAllSubtitlesAsFileAsync(
    string videoUrl,
    string fileName,
		string outputPath,
    CancellationToken cancellationToken = default)
  {
    try
    {
      // Ensure output directory exists
      if (!Directory.Exists(outputPath))
        Directory.CreateDirectory(outputPath);

      // Download video and audio
      string downloadedVideo = await youTubeMetadataService.DownloadVideoOnlyAsFileAsync(videoUrl, outputPath, cancellationToken);
      string downloadedAudio = await youTubeMetadataService.DownloadAudioOnlyAsFileAsync(videoUrl, outputPath, cancellationToken);

      var sanitizedTitle = youTubeMetadataService.SanitizeFileName(fileName);
      var nameGuid = Guid.NewGuid().ToString();
      string mergedOutput = Path.Combine(outputPath, $"{nameGuid}.mkv");

      // Merge audio and video
      await videoMerger.MergeAudioAndVideoWithoutEncodeAsync(downloadedVideo, downloadedAudio, mergedOutput);
      RenameAndRemoveOld(downloadedVideo, downloadedAudio, mergedOutput);

      _ = await youTubeMetadataService.GetAllSubtitlesAsync(videoUrl, sanitizedTitle, outputPath, cancellationToken);

      return mergedOutput;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading and merging video: {ex.Message}", ex);
    }
  }

	public async Task<byte[]> DownloadAndMergeVideoWithAudioAsync(
    string videoUrl,
    CancellationToken cancellationToken = default)
  {
    try
    {
      // Ensure output directory exists
      var outputPath = "temp";

			if (!Directory.Exists(outputPath))
        Directory.CreateDirectory(outputPath);

      // Download video and audio
      string downloadedVideo = await youTubeMetadataService.DownloadVideoOnlyAsFileAsync(videoUrl, outputPath, cancellationToken);
      string downloadedAudio = await youTubeMetadataService.DownloadAudioOnlyAsFileAsync(videoUrl, outputPath, cancellationToken);

      var parts = downloadedVideo.Split(Path.PathSeparator);
      var sanitizedTitle = youTubeMetadataService.SanitizeFileName(parts[^1]);
			string mergedOutput = Path.Combine(outputPath, $"{sanitizedTitle}.mkv");

      // Merge audio and video
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
    var fileName = youTubeMetadataService.SanitizeFileName(fileNameWithoutExtension);
    var outputPath = Path.GetDirectoryName(downloadedVideo);
    string mergedOutputDes = Path.Combine(outputPath, $"{fileName}.mkv");
    File.Move(tempFileName, mergedOutputDes);
  }
}
