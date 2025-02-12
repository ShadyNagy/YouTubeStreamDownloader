using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using YoutubeExplode;
using YoutubeExplode.Channels;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;
using YouTubeStreamDownloader.Services;

namespace YouTubeStreamDownloader.Tests;
public class GetAllVideosFromPlaylistAsyncTests
{
  private readonly YoutubeClient _youtubeClientMock;
  private readonly YouTubeMetadataService _service;

  public GetAllVideosFromPlaylistAsyncTests()
  {
    _youtubeClientMock = Substitute.For<YoutubeClient>();
    _service = new YouTubeMetadataService(_youtubeClientMock);
  }

  [Fact]
  public async Task GetAllVideosFromPlaylistAsync_ShouldReturnVideoDataList()
  {
    // Arrange
    var playlistId = new PlaylistId("PL123");
    var channelId = new ChannelId("UC123456");
    var videos = new List<PlaylistVideo>
            {
                new(playlistId, new VideoId("abc123"), "Video One", new Author(channelId, ""), null, new List<Thumbnail>()),
                new(playlistId, new VideoId("xyz456"), "Video Two", new Author(channelId, ""), null, new List<Thumbnail>())
            };

    _youtubeClientMock.Playlists.GetVideosAsync(playlistId, Arg.Any<CancellationToken>())
        .Returns(videos.ToAsyncEnumerable());

    // Act
    var result = await _service.GetAllVideosFromPlaylistAsync("https://www.youtube.com/playlist?list=PL123");

    // Assert
    result.Should().HaveCount(2);
    result[0].Title.Should().Be("Video One");
    result[0].Url.Should().Be("https://www.youtube.com/watch?v=abc123");
    result[1].Title.Should().Be("Video Two");
  }

  [Fact]
  public async Task GetAllVideosFromPlaylistAsync_ShouldThrowException_WhenPlaylistNotFound()
  {
    // Arrange
    _youtubeClientMock.Playlists.GetVideosAsync(Arg.Any<PlaylistId>(), Arg.Any<CancellationToken>())
        .Throws(new InvalidOperationException("Playlist not found"));

    // Act
    var act = async () => await _service.GetAllVideosFromPlaylistAsync("https://www.youtube.com/playlist?list=INVALID");

    // Assert
    await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Error fetching videos from playlist*");
  }
}
