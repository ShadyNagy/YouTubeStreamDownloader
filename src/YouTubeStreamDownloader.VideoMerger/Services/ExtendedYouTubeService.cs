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

			string mergedOutput = Path.Combine(outputPath, $"{fileName}.mp4");

			// Merge audio and video
			await videoMerger.MergeAudioAndVideoAsync(downloadedVideo, downloadedAudio, mergedOutput);

			return mergedOutput;
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Error downloading and merging video: {ex.Message}", ex);
		}
	}

  public async Task<byte[]> DownloadAndMergeVideoWithAudioAsFileAsync(
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
			string mergedOutput = Path.Combine(outputPath, parts[^1]);

      // Merge audio and video
      await videoMerger.MergeAudioAndVideoAsync(downloadedVideo, downloadedAudio, mergedOutput);
      
      return await File.ReadAllBytesAsync(mergedOutput, cancellationToken);
		}
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading and merging video: {ex.Message}", ex);
    }
  }
}