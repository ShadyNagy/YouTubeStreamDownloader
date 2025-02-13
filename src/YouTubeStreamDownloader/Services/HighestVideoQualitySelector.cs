using System.Collections.Generic;
using YoutubeExplode.Videos.Streams;
using YouTubeStreamDownloader.Interfaces;

namespace YouTubeStreamDownloader.Services;

public class HighestVideoQualitySelector : IStreamSelector<IVideoStreamInfo>
{
  public IVideoStreamInfo SelectStream(IEnumerable<IVideoStreamInfo> streams)
    => streams.TryGetWithHighestVideoQuality();
}

