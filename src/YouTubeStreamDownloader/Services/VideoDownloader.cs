using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YouTubeStreamDownloader.Helpers;
using YouTubeStreamDownloader.Interfaces;

namespace YouTubeStreamDownloader.Services;

public class VideoDownloader : BaseService, IVideoDownloader
{
  private readonly YoutubeClient _youtubeClient;
  private readonly IStreamSelector<IVideoStreamInfo> _defaultStreamSelector;

  public VideoDownloader(YoutubeClient youtubeClient, IStreamSelector<IVideoStreamInfo> defaultStreamSelector = null)
  {
    _youtubeClient = youtubeClient;
    _defaultStreamSelector = defaultStreamSelector ?? new HighestVideoQualitySelector();
  }

  public async Task<string> DownloadVideoAsync(string videoUrl, string outputPath, IStreamSelector<IVideoStreamInfo> streamSelector = null, CancellationToken cancellationToken = default)
  {
    var video = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
    return await DownloadVideoAsync(videoUrl, FileHelper.SanitizeFileName(video.Title), outputPath, streamSelector, cancellationToken);
  }

  public async Task<string> DownloadVideoAsync(string videoUrl, string fileName, string outputPath, IStreamSelector<IVideoStreamInfo> streamSelector = null, CancellationToken cancellationToken = default)
    => await ExecuteWithExceptionHandlingAsync(async () =>
    {
      var video = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
      var streamInfo = (streamSelector ?? _defaultStreamSelector).SelectStream(streamManifest.GetVideoStreams());

      if (streamInfo == null) throw new InvalidOperationException("No suitable video stream found");

      var filePath = Path.Combine(outputPath, $"{fileName}.mp4");
      FileHelper.EnsureDirectoryExists(filePath);
      await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, cancellationToken: cancellationToken);
      return filePath;
    }, "Error downloading video");
}
