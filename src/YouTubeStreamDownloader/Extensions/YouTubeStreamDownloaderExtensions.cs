using Microsoft.Extensions.DependencyInjection;
using SambaFileManager.Extensions;
using SambaFileManager.Models;
using YoutubeExplode;
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
      .AddScoped<YoutubeClient>()
      .AddScoped<IDownloadAudioService, DownloadAudioService>()
      .AddScoped<IDownloadSubtitleService, DownloadSubtitleService>()
      .AddScoped<IDownloadVideoService, DownloadVideoService>()
      .AddScoped<IPlaylistService, PlaylistService>()
      .AddScoped<IVideoInfoService, VideoInfoService>();
  }

  public static IServiceCollection AddYouTubeStreamDownloaderService(this IServiceCollection services, SambaSettings sambaSettings)
  {
    return services
      .AddYouTubeStreamDownloaderService()
      .AddSambaFileManagerServices(sambaSettings);

  }

  /// <summary>
  /// Registers YouTubeMetadataService as a singleton service.
  /// </summary>
  public static IServiceCollection AddYouTubeStreamDownloaderSingletonService(this IServiceCollection services)
  {
    return services
      .AddYouTubeVideoMergerSingletonService()
      .AddSingleton<YoutubeClient>()
      .AddSingleton<IDownloadAudioService, DownloadAudioService>()
      .AddSingleton<IDownloadSubtitleService, DownloadSubtitleService>()
      .AddSingleton<IDownloadVideoService, DownloadVideoService>()
      .AddSingleton<IPlaylistService, PlaylistService>()
      .AddSingleton<IVideoInfoService, VideoInfoService>();
  }

  public static IServiceCollection AddYouTubeStreamDownloaderSingletonService(this IServiceCollection services, SambaSettings sambaSettings)
  {
    return services
      .AddYouTubeStreamDownloaderSingletonService()
      .AddSambaFileManagerSingletonServices(sambaSettings);
  }
}
