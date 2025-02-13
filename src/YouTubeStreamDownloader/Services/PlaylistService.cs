using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Channels;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YouTubeStreamDownloader.Interfaces;
using YouTubeStreamDownloader.Models;

namespace YouTubeStreamDownloader.Services;

public class PlaylistService : BaseService, IPlaylistService
{
  private readonly YoutubeClient _youtubeClient;

  public PlaylistService(YoutubeClient youtubeClient) => _youtubeClient = youtubeClient;

  public async Task<List<PlaylistData>> GetAllPlaylistsAsync(string channelUrl, CancellationToken cancellationToken = default)
    => await ExecuteWithExceptionHandlingAsync(async () =>
    {
      var channel = await _youtubeClient.Channels.GetAsync(ChannelHandle.Parse(channelUrl).Value, cancellationToken);
      var playlists = await _youtubeClient.Channels.GetUploadsAsync(channel.Id, cancellationToken);
      return playlists.Select(p => new PlaylistData { Title = p.Title, Url = $"https://youtube.com/playlist?list={p.Id}" }).ToList();
    }, "Error fetching playlists");

  public async Task<List<VideoData>> GetAllVideosFromPlaylistAsync(string playlistUrl, CancellationToken cancellationToken = default)
    => await ExecuteWithExceptionHandlingAsync(async () =>
    {
      var playlistId = PlaylistId.Parse(playlistUrl);
      var videos = await _youtubeClient.Playlists.GetVideosAsync(playlistId, cancellationToken);
      return videos.Select(v => new VideoData { Title = v.Title, Url = $"https://youtube.com/watch?v={v.Id}" }).ToList();
    }, "Error fetching playlist videos");
}
