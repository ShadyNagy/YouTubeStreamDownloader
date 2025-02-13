using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YouTubeStreamDownloader.Models;

namespace YouTubeStreamDownloader.Interfaces;
public interface IPlaylistService
{
  Task<List<PlaylistData>> GetAllPlaylistsAsync(string channelUrl, CancellationToken cancellationToken = default);
  Task<List<VideoData>> GetAllVideosFromPlaylistAsync(string playlistUrl, CancellationToken cancellationToken = default);
}
