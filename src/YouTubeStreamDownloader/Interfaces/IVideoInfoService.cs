using System.Threading;
using System.Threading.Tasks;
using YouTubeStreamDownloader.Models;

namespace YouTubeStreamDownloader.Interfaces;
public interface IVideoInfoService
{
  Task<VideoData> GetVideoInfoAsync(string videoUrl, CancellationToken cancellationToken = default);
}
