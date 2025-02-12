﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Channels;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos.Streams;
using YouTubeStreamDownloader.Interfaces;
using YouTubeStreamDownloader.Models;

namespace YouTubeStreamDownloader.Services;

public class YouTubeMetadataService : IYouTubeMetadataService
{
  private readonly YoutubeClient _youtubeClient;

  public YouTubeMetadataService(YoutubeClient youtubeClient)
  {
    _youtubeClient = youtubeClient ?? throw new ArgumentNullException(nameof(youtubeClient));
  }

  /// <summary>
  /// Gets all playlists from a YouTube channel.
  /// </summary>
  public async Task<List<PlaylistData>> GetAllPlaylistsAsync(
      string channelUrl, CancellationToken cancellationToken = default)
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

  /// <summary>
  /// Gets all video links and titles from a given playlist.
  /// </summary>
  public async Task<List<VideoData>> GetAllVideosFromPlaylistAsync(
      string playlistUrl, CancellationToken cancellationToken = default)
  {
    try
    {
      var playlistId = PlaylistId.Parse(playlistUrl);
      var videos = await _youtubeClient.Playlists.GetVideosAsync(playlistId, cancellationToken);

      return videos.Select(v => new VideoData
      {
        Title = v.Title,
        Url = $"https://www.youtube.com/watch?v={v.Id}"
      }).ToList();
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error fetching videos from playlist: {ex.Message}", ex);
    }
  }

	/// <summary>
	/// Downloads a YouTube video as a file.
	/// </summary>
	public async Task<string> DownloadVideoAsFileAsync(
			string videoUrl, string outputPath, CancellationToken cancellationToken = default)
	{
		try
		{
			var video = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
			var sanitizedTitle = SanitizeFileName(video.Title);
			var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
			var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

			if (streamInfo == null)
				throw new InvalidOperationException("No suitable video stream found.");

			string filePath = Path.Combine(outputPath, $"{sanitizedTitle}.mp4");

			await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, cancellationToken: cancellationToken);

			return filePath;
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Error downloading video as file: {ex.Message}", ex);
		}
	}

	/// <summary>
	/// Streams a YouTube video directly.
	/// </summary>
	public async Task<Stream> DownloadVideoAsStreamAsync(
			string videoUrl, CancellationToken cancellationToken = default)
	{
		try
		{
			var video = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
			var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id, cancellationToken);
			var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

			if (streamInfo == null)
				throw new InvalidOperationException("No suitable video stream found.");

			return await _youtubeClient.Videos.Streams.GetAsync(streamInfo, cancellationToken);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Error streaming video: {ex.Message}", ex);
		}
	}

	private string SanitizeFileName(string name)
	{
		foreach (char c in Path.GetInvalidFileNameChars())
		{
			name = name.Replace(c, '_');
		}
		return name;
	}
}
