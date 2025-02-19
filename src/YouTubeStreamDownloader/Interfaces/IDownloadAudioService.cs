using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace YouTubeStreamDownloader.Interfaces;
public interface IDownloadAudioService
{
  Task<string> DownloadAudioWithProgressAsync(string videoUrl, IProgress<double>? progress, CancellationToken cancellationToken = default);
  Task<string> DownloadAudioWithProgressAsync(string videoUrl, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default);
  Task<string> DownloadAudioAsFileAsync(string videoUrl, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default);
  Task<string> DownloadAudioAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<Stream> DownloadAudioAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default);
}
