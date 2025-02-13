using System.Threading.Tasks;

namespace YouTubeStreamDownloader.VideoMerger.Interfaces;

public interface IVideoMerger
{
  Task MergeAudioAndVideoAsync(string inputVideoPath, string inputAudioPath, string outputFilePath);
}

