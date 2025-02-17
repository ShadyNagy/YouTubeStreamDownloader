using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YouTubeStreamDownloader.Models;

namespace YouTubeStreamDownloader.Interfaces;
public interface IDownloadAudioService
{
  Task<Stream> DownloadAudioWithProgressAsync(string videoUrl, IProgress<DownloadProgress> progress, CancellationToken cancellationToken = default);
  Task<string> DownloadAudioAsFileAsync(string videoUrl, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default);
  Task<string> DownloadAudioAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<Stream> DownloadAudioAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default);
}
