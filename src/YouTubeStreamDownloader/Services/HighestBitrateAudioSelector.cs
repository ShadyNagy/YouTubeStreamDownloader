using System.Collections.Generic;
using YoutubeExplode.Videos.Streams;
using YouTubeStreamDownloader.Interfaces;

namespace YouTubeStreamDownloader.Services;

public class HighestBitrateAudioSelector : IStreamSelector<IStreamInfo>
{
  public IStreamInfo SelectStream(IEnumerable<IStreamInfo> streams)
    => streams.TryGetWithHighestBitrate();
}
