using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode.Videos.Streams;

namespace YouTubeStreamDownloader.Interfaces;

public interface IAudioDownloader
{
  Task<string> DownloadAudioAsync(string videoUrl, string outputPath, IStreamSelector<IAudioStreamInfo> streamSelector = null, CancellationToken cancellationToken = default);
}
