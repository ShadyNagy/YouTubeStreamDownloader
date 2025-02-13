using System.Collections.Generic;
using YoutubeExplode.Videos.Streams;

namespace YouTubeStreamDownloader.Interfaces;

public interface IStreamSelector<T> where T : IStreamInfo
{
  T SelectStream(IEnumerable<T> streams);
}
