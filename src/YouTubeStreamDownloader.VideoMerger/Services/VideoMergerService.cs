using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg.Downloader;
using Xabe.FFmpeg;
using YouTubeStreamDownloader.VideoMerger.Interfaces;

namespace YouTubeStreamDownloader.VideoMerger.Services;

public class VideoMergerService : IVideoMerger
{
  public async Task MergeAudioAndVideoAsync(string inputVideoPath, string inputAudioPath, string outputFilePath)
  {

		// Ensure input files exist
		if (!File.Exists(inputVideoPath) || !File.Exists(inputAudioPath))
      throw new FileNotFoundException("Input file not found.");

    await DownloadFFmpegAsync();

    // Extract video stream (without audio) from the input MP4
    var videoStream = (await FFmpeg.GetMediaInfo(inputVideoPath))
      .VideoStreams.FirstOrDefault();

    // Extract audio stream from the input MP3
    var audioStream = (await FFmpeg.GetMediaInfo(inputAudioPath))
      .AudioStreams.FirstOrDefault();

    if (videoStream == null || audioStream == null)
      throw new InvalidOperationException("Invalid input streams.");

    // Merge the streams into a new MP4
    var conversion = FFmpeg.Conversions.New()
      .AddStream(videoStream)
      .AddStream(audioStream)
      .SetOutput(outputFilePath)
      .SetOverwriteOutput(true);

    await conversion.Start();
  }

  public async Task MergeAudioAndVideoWithoutEncodeAsync(string inputVideoPath, string inputAudioPath, string outputFilePath)
  {
    // Ensure input files exist
    if (!File.Exists(inputVideoPath) || !File.Exists(inputAudioPath))
      throw new FileNotFoundException("Input file not found.");

    await DownloadFFmpegAsync();

    try
    {
      // Extract video stream (without audio) from the input MP4
      var videoStream = (await FFmpeg.GetMediaInfo(inputVideoPath))
        .VideoStreams.First().CopyStream();

      // Extract audio stream from the input MP3
      var audioStream = (await FFmpeg.GetMediaInfo(inputAudioPath))
        .AudioStreams.First().CopyStream();

      if (videoStream == null || audioStream == null)
        throw new InvalidOperationException("Invalid input streams.");

      // Merge the streams into a new MP4
      var conversion = FFmpeg.Conversions.New()
        .AddStream(videoStream)
        .AddStream(audioStream)
        .SetOutput(outputFilePath)
        .SetOverwriteOutput(true);

      IConversionResult result = await conversion.Start();

      if (!File.Exists(outputFilePath))
        throw new InvalidOperationException("Output file was not created");
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Merge failed", ex);
    }
  }

  private async Task DownloadFFmpegAsync()
  {
    // Specify where FFmpeg will be downloaded/unpacked
    string ffmpegPath = Environment.CurrentDirectory;
    // Or pick any folder you want, e.g.: C:\\Tools\\FFmpeg

    // (Optional) Tell Xabe.FFmpeg where to look for the executables
    FFmpeg.SetExecutablesPath(Environment.CurrentDirectory);

    // Download the latest official build of ffmpeg into the specified folder
    await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, ffmpegPath);
  }
}
