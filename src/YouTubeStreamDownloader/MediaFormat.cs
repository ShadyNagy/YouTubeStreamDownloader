namespace YouTubeStreamDownloader;

public class MediaFormat
{
  public string Url { get; set; } = string.Empty;
  public string MimeType { get; set; } = string.Empty;
	public string QualityLabel { get; set; } = string.Empty;
	public long Bitrate { get; set; }
  public string AudioQuality { get; set; } = string.Empty;
}
