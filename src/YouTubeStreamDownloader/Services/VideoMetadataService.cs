using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YouTubeStreamDownloader.Interfaces;
using YouTubeStreamDownloader.Models;

namespace YouTubeStreamDownloader.Services;

public class VideoMetadataService : BaseService, IVideoMetadataService
{
  private readonly YoutubeClient _youtubeClient;

  public VideoMetadataService(YoutubeClient youtubeClient) => _youtubeClient = youtubeClient;

  public async Task<VideoData> GetVideoInfoAsync(string videoUrl, CancellationToken cancellationToken = default)
    => await ExecuteWithExceptionHandlingAsync(async () =>
    {
      var video = await _youtubeClient.Videos.GetAsync(videoUrl, cancellationToken);
      return new VideoData
      {
        Title = video.Title,
        Url = $"https://www.youtube.com/watch?v={video.Id}",
        Author = video.Author.ChannelTitle,
        Duration = video.Duration?.ToString() ?? string.Empty
      };
    }, "Error fetching video info");
}
