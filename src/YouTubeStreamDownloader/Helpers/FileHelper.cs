using System.IO;

namespace YouTubeStreamDownloader.Helpers;

public static class FileHelper
{
  public static string SanitizeFileName(string name)
  {
    foreach (char c in Path.GetInvalidFileNameChars())
      name = name.Replace(c, '_');
    return name;
  }

  public static void EnsureDirectoryExists(string filePath)
  {
    var directory = Path.GetDirectoryName(filePath);
    if (!string.IsNullOrEmpty(directory))
      Directory.CreateDirectory(directory);
  }
}
