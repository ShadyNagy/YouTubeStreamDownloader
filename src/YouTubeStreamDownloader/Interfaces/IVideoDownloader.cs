using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode.Videos.Streams;

namespace YouTubeStreamDownloader.Interfaces;

public interface IVideoDownloader
{
  Task<string> DownloadVideoAsync(string videoUrl, string outputPath, IStreamSelector<IVideoStreamInfo> streamSelector = null, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoAsync(string videoUrl, string fileName, string outputPath, IStreamSelector<IVideoStreamInfo> streamSelector = null, CancellationToken cancellationToken = default);
}
