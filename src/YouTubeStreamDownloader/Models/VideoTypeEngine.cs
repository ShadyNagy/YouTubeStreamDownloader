using System;
using System.Collections.Generic;
using System.Linq;
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

	public static IVideoStreamInfo GetByVideoType(VideoType videoType, IEnumerable<IVideoStreamInfo> streamInfos)
  {
		foreach (var videoStreamInfo in streamInfos)
    {
      if (Map(videoStreamInfo) == videoType)
      {
        return videoStreamInfo;
      }
    }

    return streamInfos.ElementAt(0);
  }
}
