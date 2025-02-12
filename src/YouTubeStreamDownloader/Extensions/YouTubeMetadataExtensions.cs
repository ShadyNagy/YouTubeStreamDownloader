using Microsoft.Extensions.DependencyInjection;
using YouTubeStreamDownloader.Interfaces;
using YouTubeStreamDownloader.Services;

namespace YouTubeStreamDownloader.Extensions;
public static class YouTubeMetadataExtensions
{
  /// <summary>
  /// Registers YouTubeMetadataService as a scoped service.
  /// </summary>
  public static IServiceCollection AddYouTubeMetadataService(this IServiceCollection services)
  {
    return services.AddScoped<IYouTubeMetadataService, YouTubeMetadataService>();
  }

  /// <summary>
  /// Registers YouTubeMetadataService as a singleton service.
  /// </summary>
  public static IServiceCollection AddYouTubeMetadataSingletonService(this IServiceCollection services)
  {
    return services.AddSingleton<IYouTubeMetadataService, YouTubeMetadataService>();
  }
}
