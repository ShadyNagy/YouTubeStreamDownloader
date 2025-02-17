using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeStreamDownloader.Helpers;

public class AutoDeleteFileStream(string path) : FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
  FileOptions.DeleteOnClose)
{
  private readonly string _filePath = path;

  protected override void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (File.Exists(_filePath))
    {
      try { File.Delete(_filePath); }
      catch { /* Ignore cleanup errors */ }
    }
  }
}
