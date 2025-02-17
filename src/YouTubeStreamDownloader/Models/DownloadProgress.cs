using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeStreamDownloader.Models;

public record DownloadProgress(
  double Percentage,
  long BytesReceived,
  long TotalBytes
);
