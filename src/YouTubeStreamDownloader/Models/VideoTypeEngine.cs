using System;
using System.Collections.Generic;
using YoutubeExplode.Videos.Streams;

namespace YouTubeStreamDownloader.Models;

public class VideoTypeEngine
{
  public static VideoType Map(IVideoStreamInfo streamInfo)
  {
		return Map(streamInfo.VideoQuality.MaxHeight);
	}

	public static VideoType Map(string videoLabel)
  {
    if (videoLabel.ToLower() == "144")
    {
      return VideoType.Q144;
    }

    return VideoType.Q144;
	}

	public static VideoType Map(int maxHeight)
  {
    return Enum.IsDefined(typeof(VideoType), maxHeight)
      ? (VideoType)maxHeight
      : VideoType.Q144;
  }

	public static IVideoStreamInfo? GetMp4ByVideoType(VideoType videoType, IEnumerable<IVideoStreamInfo> streamInfos)
  {
		foreach (var videoStreamInfo in streamInfos)
    {
      if (Map(videoStreamInfo) == videoType && videoStreamInfo.Container.Name.ToLower() == "mp4")
      {
        return videoStreamInfo;
      }
    }

    return null;
  }
}
