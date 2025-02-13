using Microsoft.Extensions.DependencyInjection;
using YouTubeStreamDownloader.Extensions;
using YouTubeStreamDownloader.VideoMerger.Interfaces;
using YouTubeStreamDownloader.VideoMerger.Services;

namespace YouTubeStreamDownloader.VideoMerger.Extensions;
public static class YouTubeVideoMergerExtensions
{
  public static IServiceCollection AddYouTubeVideoMergerService(this IServiceCollection services)
  {
    return services
      .AddYouTubeMetadataService()
			.AddScoped<IVideoMerger, VideoMergerService>()
      .AddScoped<IExtendedYouTubeService, ExtendedYouTubeService>();
	}

  public static IServiceCollection AddYouTubeVideoMergerSingletonService(this IServiceCollection services)
  {
    return services
      .AddYouTubeMetadataSingletonService()
			.AddSingleton<IVideoMerger, VideoMergerService>()
      .AddSingleton<IExtendedYouTubeService, ExtendedYouTubeService>();
	}
}
