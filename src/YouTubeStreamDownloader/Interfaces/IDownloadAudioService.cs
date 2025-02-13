using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace YouTubeStreamDownloader.Interfaces;
public interface IDownloadAudioService
{
  Task<string> DownloadAudioOnlyAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<Stream> DownloadAudioOnlyAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default);
}
