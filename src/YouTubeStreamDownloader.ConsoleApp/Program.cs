using Microsoft.Extensions.DependencyInjection;
using YoutubeExplode;
using YouTubeStreamDownloader.Interfaces;
using YouTubeStreamDownloader.Services;

namespace YouTubeStreamDownloader.ConsoleApp;

internal class Program
{
  static string TEST_VIDEO_URL = "https://www.youtube.com/watch?v=eld6m3KLEHo";

  static async Task Main(string[] args)
  {
    await GetVideoInfoAsync();

    await DownloadVideoAsync();

    await GetSubtitleAsync();

		Console.WriteLine("Press Any Key To Exit");
    Console.ReadKey();
  }

  static async Task GetVideoInfoAsync()
  {
    IYouTubeMetadataService downloader = new YouTubeMetadataService(new YoutubeClient());
    var video = await downloader.GetVideoInfoAsync(TEST_VIDEO_URL);

    Console.WriteLine($"Title: {video.Title}");
    Console.WriteLine($"Duration: {video.Duration}");
    Console.WriteLine($"Author: {video.Author}");
	}

  static async Task DownloadVideoAsync()
  {
		IYouTubeMetadataService downloader = new YouTubeMetadataService(new YoutubeClient());
		var outputPath = "C:\\Videos";
    string filePath = await downloader.DownloadVideoAsFileAsync(TEST_VIDEO_URL, outputPath);
    Console.WriteLine($"Video downloaded successfully: {filePath}");
	}

  static async Task GetSubtitleAsync()
  {
    IYouTubeMetadataService downloader = new YouTubeMetadataService(new YoutubeClient());
    var subtitle = await downloader.GetSubtitleAsync(TEST_VIDEO_URL);
    if (string.IsNullOrEmpty(subtitle))
    {
      Console.WriteLine("No Subtitle!");
      return;
    }
    Console.WriteLine($"Subtitle downloaded successfully: {subtitle}");
	}
}
