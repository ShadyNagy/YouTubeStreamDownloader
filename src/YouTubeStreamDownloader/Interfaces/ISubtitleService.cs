using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace YouTubeStreamDownloader.Interfaces;

public interface ISubtitleService
{
  Task<string> GetSubtitleAsync(string videoUrl, string fileName, string outputPath, string? languageCode, CancellationToken cancellationToken = default);
  Task<List<string>> GetAllSubtitlesAsync(string videoUrl, string fileName, string outputPath, CancellationToken cancellationToken = default);
  Task<string> GetSubtitleContentAsync(string videoUrl, string? languageCode, CancellationToken cancellationToken = default);
}
