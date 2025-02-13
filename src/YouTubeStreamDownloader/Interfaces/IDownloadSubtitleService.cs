using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace YouTubeStreamDownloader.Interfaces;
public interface IDownloadSubtitleService
{
  Task<string> GetSubtitleAsync(string videoUrl, string fileName, string outputPath, string? languageCode = null, CancellationToken cancellationToken = default);
  Task<List<string>> GetAllSubtitlesAsync(string videoUrl, string fileName, string outputPath, CancellationToken cancellationToken = default);
  Task<string> GetSubtitleAsync(string videoUrl, string? languageCode = null, CancellationToken cancellationToken = default);
}
