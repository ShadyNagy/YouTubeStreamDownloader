using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YouTubeStreamDownloader.Models;

namespace YouTubeStreamDownloader.Interfaces;

public interface IYouTubeMetadataService
{
  Task<VideoData> GetVideoInfoAsync(string videoUrl, CancellationToken cancellationToken = default);

	Task<List<PlaylistData>> GetAllPlaylistsAsync(string channelUrl, CancellationToken cancellationToken = default);
  Task<List<VideoData>> GetAllVideosFromPlaylistAsync(string playlistUrl, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);

  Task<string> DownloadVideoAsFileAsync(string videoUrl, string fileName, string outputPath, CancellationToken cancellationToken = default);

	Task<Stream> DownloadVideoAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default);

  Task<string> GetSubtitleAsync(string videoUrl, string fileName, string outputPath, string? languageCode = null, CancellationToken cancellationToken = default);
  Task<string> GetSubtitleAsync(string videoUrl, string? languageCode = null, CancellationToken cancellationToken = default);

  Task<string> DownloadVideoOnlyAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<string> DownloadAudioOnlyAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<Stream> DownloadVideoOnlyAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default);
  Task<Stream> DownloadAudioOnlyAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default);
}
