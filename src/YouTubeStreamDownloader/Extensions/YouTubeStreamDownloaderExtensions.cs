using Microsoft.Extensions.DependencyInjection;
using YouTubeStreamDownloader.Interfaces;
using YouTubeStreamDownloader.Services;
using YouTubeStreamDownloader.VideoMerger.Extensions;

namespace YouTubeStreamDownloader.Extensions;
public static class YouTubeStreamDownloaderExtensions
{
  /// <summary>
  /// Registers YouTubeMetadataService as a scoped service.
  /// </summary>
  public static IServiceCollection AddYouTubeStreamDownloaderService(this IServiceCollection services)
  {
    return services
      .AddYouTubeVideoMergerService()
      .AddScoped<IDownloadAudioService, DownloadAudioService>()
      .AddScoped<IDownloadSubtitleService, DownloadSubtitleService>()
      .AddScoped<IDownloadVideoService, DownloadVideoService>()
      .AddScoped<IPlaylistService, PlaylistService>()
      .AddScoped<IVideoInfoService, VideoInfoService>();
  }

  /// <summary>
  /// Registers YouTubeMetadataService as a singleton service.
  /// </summary>
  public static IServiceCollection AddYouTubeStreamDownloaderSingletonService(this IServiceCollection services)
  {
    return services
      .AddYouTubeVideoMergerSingletonService()
      .AddSingleton<IDownloadAudioService, DownloadAudioService>()
      .AddSingleton<IDownloadSubtitleService, DownloadSubtitleService>()
      .AddSingleton<IDownloadVideoService, DownloadVideoService>()
      .AddSingleton<IPlaylistService, PlaylistService>()
      .AddSingleton<IVideoInfoService, VideoInfoService>();
  }
}
