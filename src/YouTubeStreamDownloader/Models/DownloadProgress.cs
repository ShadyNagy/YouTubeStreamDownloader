namespace YouTubeStreamDownloader.Models;

public record DownloadProgress(
  double Percentage,
  long BytesReceived,
  long TotalBytes
);
