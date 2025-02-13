using NSubstitute;
using NSubstitute.ExceptionExtensions;
using YoutubeExplode;
using YoutubeExplode.Channels;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace YouTubeStreamDownloader.Tests;
  public class GetAllPlaylistsAsyncTests
  {
    private readonly YoutubeClient _youtubeClientMock;
    private readonly YouTubeMetadataService _service;

    public GetAllPlaylistsAsyncTests()
    {
      _youtubeClientMock = Substitute.For<YoutubeClient>();
      _service = new YouTubeMetadataService(_youtubeClientMock);
    }

    [Fact]
    public async Task GetAllPlaylistsAsync_ShouldReturnPlaylistDataList()
    {
      // Arrange
      var channelId = new ChannelId("UC123456");
      var playlists = new List<PlaylistVideo>
            {
                new(new PlaylistId("PL111"), new VideoId("abc123"), "Playlist One", new Author(channelId, ""), null, new List<Thumbnail>()),
                new(new PlaylistId("PL222"), new VideoId("abc123"), "Playlist Two", new Author(channelId, ""), null, new List<Thumbnail>())
            };

    _youtubeClientMock.Channels.GetByHandleAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
      .Returns(callInfo => new ValueTask<Channel>(
        Task.FromResult(new Channel(new ChannelId("UC123456"), "Test Channel", new List<Thumbnail>()))
      ));

    _youtubeClientMock.Channels.GetUploadsAsync(channelId, Arg.Any<CancellationToken>())
          .Returns(playlists.ToAsyncEnumerable());

      // Act
      var result = await _service.GetAllPlaylistsAsync("https://www.youtube.com/@TestChannel");

      // Assert
      result.Should().HaveCount(2);
      result[0].Title.Should().Be("Playlist One");
      result[0].Url.Should().Be("https://www.youtube.com/playlist?list=PL111");
      result[1].Title.Should().Be("Playlist Two");
    }

    [Fact]
    public async Task GetAllPlaylistsAsync_ShouldThrowException_WhenChannelNotFound()
    {
      // Arrange
      _youtubeClientMock.Channels.GetByHandleAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
          .Throws(new InvalidOperationException("Channel not found"));

      // Act
      var act = async () => await _service.GetAllPlaylistsAsync("https://www.youtube.com/@InvalidChannel");

      // Assert
      await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Error fetching playlists*");
    }
  }
