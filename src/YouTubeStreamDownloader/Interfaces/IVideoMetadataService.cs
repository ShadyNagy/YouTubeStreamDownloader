using System.Threading;
using System.Threading.Tasks;
using YouTubeStreamDownloader.Models;

namespace YouTubeStreamDownloader.Interfaces;

public interface IVideoMetadataService
{
  Task<VideoData> GetVideoInfoAsync(string videoUrl, CancellationToken cancellationToken = default);
}
