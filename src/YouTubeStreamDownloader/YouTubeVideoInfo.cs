using System.Collections.Generic;

namespace YouTubeStreamDownloader;

public class YouTubeVideoInfo
{
  public string VideoId { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public List<MediaFormat> MediaFormats { get; set; } = new();
}
