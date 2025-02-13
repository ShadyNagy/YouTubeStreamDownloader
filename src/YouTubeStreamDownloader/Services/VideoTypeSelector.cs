using System.Collections.Generic;
using YoutubeExplode.Videos.Streams;
using YouTubeStreamDownloader.Interfaces;
using YouTubeStreamDownloader.Models;

namespace YouTubeStreamDownloader.Services;

public class VideoTypeSelector : IStreamSelector<IVideoStreamInfo>
{
  private readonly VideoType _videoType;
  public VideoTypeSelector(VideoType videoType) => _videoType = videoType;

  public IVideoStreamInfo SelectStream(IEnumerable<IVideoStreamInfo> streams)
    => VideoTypeEngine.GetMp4ByVideoType(_videoType, streams);
}
