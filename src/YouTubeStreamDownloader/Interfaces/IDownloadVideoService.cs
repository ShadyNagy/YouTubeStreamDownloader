using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace YouTubeStreamDownloader.Interfaces;

public interface IDownloadVideoService
{
  Task<string> DownloadVideoAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoWithSubtitlesAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<string> DownloadHighestVideoOnlyAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoOnlyAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoAsFileAsync(string videoUrl, string fileName, string outputPath, CancellationToken cancellationToken = default);
  Task<Stream> DownloadVideoAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default);
  Task<Stream> DownloadVideoOnlyAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default);
  Task<string> DownloadAndMergeVideoWithAudioAsFileAsync(string videoUrl, string fileName, string outputPath, CancellationToken cancellationToken = default);
  Task<string> DownloadAndMergeVideoWithAudioAllSubtitlesAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<string> DownloadAndMergeVideoWithAudioAllSubtitlesAsFileAsync(string videoUrl, string fileName, string outputPath, CancellationToken cancellationToken = default);
  Task<byte[]> DownloadAndMergeVideoWithAudioAsync(string videoUrl, CancellationToken cancellationToken = default);
}
