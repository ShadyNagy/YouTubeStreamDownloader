using System.Threading;
using System.Threading.Tasks;

namespace YouTubeStreamDownloader.VideoMerger.Interfaces;

public interface IExtendedYouTubeService
{
  Task<string> DownloadAndMergeVideoWithAudioAsFileAsync(
    string videoUrl,
    string fileName,
    string outputPath,
    CancellationToken cancellationToken = default);

  Task<byte[]> DownloadAndMergeVideoWithAudioAsync(
    string videoUrl,
    CancellationToken cancellationToken = default);

  Task<string> DownloadAndMergeVideoWithAudioAllSubtitlesAsFileAsync(
    string videoUrl,
    string outputPath,
    CancellationToken cancellationToken = default);

  Task<string> DownloadAndMergeVideoWithAudioAllSubtitlesAsFileAsync(
    string videoUrl,
    string fileName,
    string outputPath,
    CancellationToken cancellationToken = default);
}