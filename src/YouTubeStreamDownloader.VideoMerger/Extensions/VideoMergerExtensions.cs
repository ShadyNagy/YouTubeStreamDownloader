using Microsoft.Extensions.DependencyInjection;
using YouTubeStreamDownloader.VideoMerger.Interfaces;
using YouTubeStreamDownloader.VideoMerger.Services;

namespace YouTubeStreamDownloader.VideoMerger.Extensions;
public static class VideoMergerExtensions
{
  public static IServiceCollection AddYouTubeVideoMergerService(this IServiceCollection services)
  {
    return services
      .AddScoped<IVideoMerger, VideoMergerService>();
  }

  public static IServiceCollection AddYouTubeVideoMergerSingletonService(this IServiceCollection services)
  {
    return services
      .AddSingleton<IVideoMerger, VideoMergerService>();
  }
}
