using System;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YouTubeStreamDownloader.Interfaces;
using YouTubeStreamDownloader.Models;

namespace YouTubeStreamDownloader.Services;

public class VideoInfoService : IVideoInfoService
{
  private readonly YoutubeClient _youtubeClient = new();

  public async Task<VideoData> GetVideoInfoAsync(string videoUrl, CancellationToken cancellationToken = default)
  {
    try
    {
      var videoInfo = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);

      return new VideoData
      {
        Title = videoInfo.Title,
        Url = $"https://www.youtube.com/playlist?list={videoInfo.Id}",
        Author = videoInfo.Author.ChannelTitle,
        Duration = videoInfo.Duration == null? string.Empty: videoInfo.Duration.ToString()!
			};
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error fetching playlists: {ex.Message}", ex);
    }
  }
}
