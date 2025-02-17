using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YouTubeStreamDownloader.Extensions;
using YouTubeStreamDownloader.Interfaces;
using YouTubeStreamDownloader.Models;

namespace YouTubeStreamDownloader.ConsoleApp;

internal class Program
{
  const string TEST_VIDEO_URL = "https://www.youtube.com/watch?v=dWQAUyBi-b4";
  static IDownloadAudioService _downloadAudioService = null!;
  static IPlaylistService _playlistService = null!;
  static IDownloadSubtitleService _downloadSubtitleService = null!;
  static IDownloadVideoService _downloadVideoService = null!;
  static IVideoInfoService _videoInfoService = null!;

  static async Task Main(string[] args)
  {
    SetupDi();

    await TryProgressVideoAsync();

    await TryProgressAudioAsync();

    await GetVideoWithSubtitleAsync();

		await GetVideoInfoAsync();

    await DownloadVideoAsync();

    await GetVideoMergedAsync();

		await GetSubtitleAsync();

		Console.WriteLine("Press Any Key To Exit");
    Console.ReadKey();
  }

  static async Task TryAsync()
  {
    var videoInfo = await YouTubeParser.GetVideoInfoAsync(TEST_VIDEO_URL);

    Console.WriteLine($"Title: {videoInfo.Title}");
    Console.WriteLine($"Description: {videoInfo.Description}");

    foreach (var format in videoInfo.MediaFormats)
    {
      Console.WriteLine($"Type: {(format.MimeType.StartsWith("audio") ? "Audio" : "Video")}");
      Console.WriteLine($"Format: {format.MimeType}");
      Console.WriteLine($"Quality: {format.QualityLabel ?? format.AudioQuality}");
      Console.WriteLine($"URL: {format.Url}");
      Console.WriteLine();
    }
	}

  static async Task TryProgressVideoAsync()
  {
    var progress = new Progress<double>(p =>
    {
      Console.WriteLine($"Download progress: {p:P2}");
    });

    using var cts = new CancellationTokenSource();
    var downloadTask = _downloadVideoService.DownloadVideoWithProgressAndMergeAsync(TEST_VIDEO_URL, VideoType.Q720, progress, cts.Token);
    var cancelTask = Task.Run(() =>
    {
      Console.ReadKey(true);
      cts.Cancel();
    }, cts.Token);

    await Task.WhenAny(downloadTask, cancelTask);

    if (downloadTask.IsCompletedSuccessfully)
    {
      var filePath = await downloadTask;
      Console.WriteLine($"\nDownload complete!");
      Console.WriteLine($"Temporary file path: {filePath}");
    }
    else
    {
      Console.WriteLine("\nDownload was canceled or failed.");
    }

    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
  }

  static async Task TryProgressAudioAsync()
  {
    var progress = new Progress<double>(p =>
    {
      Console.WriteLine($"Download progress: {p:P2}");
    });

    using var cts = new CancellationTokenSource();
    var downloadTask = _downloadAudioService.DownloadAudioWithProgressAsync(TEST_VIDEO_URL, progress, cts.Token);
    var cancelTask = Task.Run(() =>
    {
      Console.ReadKey(true);
      cts.Cancel();
    }, cts.Token);

    await Task.WhenAny(downloadTask, cancelTask);

    if (downloadTask.IsCompletedSuccessfully)
    {
      var filePath = await downloadTask;
      Console.WriteLine($"\nDownload complete!");
      Console.WriteLine($"Temporary file path: {filePath}");
    }
    else
    {
      Console.WriteLine("\nDownload was canceled or failed.");
    }

    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
  }

  static async Task GetVideoInfoAsync()
  {
    var video = await _videoInfoService.GetVideoInfoAsync(TEST_VIDEO_URL);

    Console.WriteLine($"Title: {video.Title}");
    Console.WriteLine($"Duration: {video.Duration}");
    Console.WriteLine($"Author: {video.Author}");
	}

  static async Task DownloadVideoAsync()
  {
		var outputPath = "C:\\Videos";
    string filePath = await _downloadVideoService.DownloadVideoAsFileAsync(TEST_VIDEO_URL, outputPath);
    Console.WriteLine($"Video downloaded successfully: {filePath}");
	}

  static async Task GetSubtitleAsync()
  {
    var subtitle = await _downloadSubtitleService.GetSubtitleAsync(TEST_VIDEO_URL);
    if (string.IsNullOrEmpty(subtitle))
    {
      Console.WriteLine("No Subtitle!");
      return;
    }
    Console.WriteLine($"Subtitle downloaded successfully: {subtitle}");
	}

  static async Task GetVideoMergedAsync()
  {
		var videoBytes = await _downloadVideoService.DownloadAndMergeVideoWithAudioAsync(TEST_VIDEO_URL);
    if (videoBytes.Length <= 0)
    {
      Console.WriteLine("No Video!");
      return;
    }
    Console.WriteLine("Video downloaded successfully");
  }

  static async Task GetVideoWithSubtitleAsync()
  {
		var outputPath = "C:\\Videos";
		var videoPath = await _downloadVideoService.DownloadAndMergeVideoWithAudioAllSubtitlesAsFileAsync(TEST_VIDEO_URL, outputPath);
    if (string.IsNullOrEmpty(videoPath))
    {
      Console.WriteLine("No Video!");
      return;
    }
    Console.WriteLine("Video downloaded successfully");
  }


  static void SetupDi()
  {
    using IHost host = Host.CreateDefaultBuilder()
      .ConfigureServices((_, services) =>
      {
        services.AddYouTubeStreamDownloaderService();
      })
      .Build();

    _downloadAudioService = host.Services.GetRequiredService<IDownloadAudioService>();
    _downloadSubtitleService = host.Services.GetRequiredService<IDownloadSubtitleService>();
    _downloadVideoService = host.Services.GetRequiredService<IDownloadVideoService>();
    _playlistService = host.Services.GetRequiredService<IPlaylistService>();
    _videoInfoService = host.Services.GetRequiredService<IVideoInfoService>();
  }
}
