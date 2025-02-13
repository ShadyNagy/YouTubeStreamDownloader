using System;
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

public class PlaylistService : IPlaylistService
{
  private readonly YoutubeClient _youtubeClient = new();

	public async Task<List<PlaylistData>> GetAllPlaylistsAsync(string channelUrl, CancellationToken cancellationToken = default)
  {
    try
    {
      var channel = await _youtubeClient.Channels.GetAsync(ChannelHandle.Parse(channelUrl).Value, cancellationToken);
      var playlists = await _youtubeClient.Channels.GetUploadsAsync(channel.Id, cancellationToken);

      return playlists.Select(p => new PlaylistData
      {
        Title = p.Title,
        Url = $"https://www.youtube.com/playlist?list={p.Id}"
      }).ToList();
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error fetching playlists: {ex.Message}", ex);
    }
  }

  public async Task<List<VideoData>> GetAllVideosFromPlaylistAsync(string playlistUrl, CancellationToken cancellationToken = default)
  {
    try
    {
      var playlistId = PlaylistId.Parse(playlistUrl);
      var videos = await _youtubeClient.Playlists.GetVideosAsync(playlistId, cancellationToken);

      return videos.Select(v => new VideoData
      {
        Title = v.Title,
        Url = $"https://www.youtube.com/watch?v={v.Id}",
        Author = v.Author.ChannelTitle,
        Duration = v.Duration.ToString()?? string.Empty
      }).ToList();
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error fetching videos from playlist: {ex.Message}", ex);
    }
  }
}
