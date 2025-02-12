using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YouTubeStreamDownloader.Models;

namespace YouTubeStreamDownloader.Interfaces;

public interface IYouTubeMetadataService
{
  Task<List<PlaylistData>> GetAllPlaylistsAsync(string channelUrl, CancellationToken cancellationToken = default);
  Task<List<VideoData>> GetAllVideosFromPlaylistAsync(string playlistUrl, CancellationToken cancellationToken = default);
  Task<string> DownloadVideoAsFileAsync(string videoUrl, string outputPath, CancellationToken cancellationToken = default);
  Task<Stream> DownloadVideoAsStreamAsync(string videoUrl, CancellationToken cancellationToken = default);
}
