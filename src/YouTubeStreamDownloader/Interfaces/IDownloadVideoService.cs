using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using YouTubeStreamDownloader.Models;

namespace YouTubeStreamDownloader.Interfaces;

public interface IDownloadVideoService
{
  Task<string> DownloadVideoWithProgressAndMergeAsync(string videoUrl, string fileName, string outputPath, VideoType quality, IProgress<double>? progress, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoWithProgressAndMergeAsync(string videoUrl, string outputPath, VideoType quality, IProgress<double>? progress, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoWithProgressAndMergeAsync(string videoUrl, VideoType quality, IProgress<double>? progress, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoAsFileAsync(string videoUrl, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoWithSubtitlesAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoWithSubtitlesAsFileAsync(string videoUrl, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default);
  Task<string> DownloadHighestVideoOnlyAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<string> DownloadHighestVideoOnlyAsFileAsync(string videoUrl, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoOnlyAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoOnlyAsFileAsync(string videoUrl, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoAsFileAsync(string videoUrl, string fileName, string outputPath, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoAsFileAsync(string videoUrl, string fileName, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default);
  Task<Stream> DownloadVideoAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default);
  Task<Stream> DownloadVideoOnlyAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default);
  Task<string> DownloadAndMergeVideoWithAudioAsFileAsync(string videoUrl, string fileName, string outputPath, CancellationToken cancellationToken = default);
  Task<string> DownloadAndMergeVideoWithAudioAsFileAsync(string videoUrl, string fileName, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default);
  Task<string> DownloadAndMergeVideoWithAudioAllSubtitlesAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<string> DownloadAndMergeVideoWithAudioAllSubtitlesAsFileAsync(string videoUrl, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default);
  Task<string> DownloadAndMergeVideoWithAudioAllSubtitlesAsFileAsync(string videoUrl, string fileName, string outputPath, CancellationToken cancellationToken = default);
  Task<string> DownloadAndMergeVideoWithAudioAllSubtitlesAsFileAsync(string videoUrl, string fileName, string outputPath, IProgress<double>? progress, CancellationToken cancellationToken = default);
  Task<byte[]> DownloadAndMergeVideoWithAudioAsync(string videoUrl, CancellationToken cancellationToken = default);
}
