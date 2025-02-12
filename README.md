# YouTubeStreamDownloader 🎬

**YouTubeStreamDownloader** is a **.NET 8 package** that allows developers to interact with **YouTube**.  
It provides functionality to **retrieve video metadata, download videos, extract audio, get subtitles, and fetch playlist & channel details** using the [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) library.

## 🚀 Features
✅ **Retrieve Video Metadata** (title, duration, author, etc.)  
✅ **Download YouTube Videos** (highest quality available)  
✅ **Extract & Download Subtitles** (SRT format)  
✅ **Fetch Playlists & Channel Videos**  
✅ **Supports Dependency Injection (DI)**  

---

## 📦 Installation
You can install **YouTubeStreamDownloader** via **NuGet**:

```sh
dotnet add package YouTubeStreamDownloader
```

OR via **Package Manager**:
```sh
Install-Package YouTubeStreamDownloader
```

---

## 🔥 Quick Start

### **1️⃣ Retrieve Video Metadata**
```csharp
IYouTubeMetadataService downloader = new YouTubeMetadataService(new YoutubeClient());
var video = await downloader.GetVideoInfoAsync(TEST_VIDEO_URL);

Console.WriteLine($"Title: {video.Title}");
Console.WriteLine($"Duration: {video.Duration}");
Console.WriteLine($"Author: {video.Author}");
```

---

### **2️⃣ Download YouTube Video**
```csharp
IYouTubeMetadataService downloader = new YouTubeMetadataService(new YoutubeClient());
var outputPath = "C:\\Videos";
string filePath = await downloader.DownloadVideoAsFileAsync(TEST_VIDEO_URL, outputPath);
Console.WriteLine($"Video downloaded successfully: {filePath}");
```

---

### **3️⃣ Fetch All Playlists from a Channel**
```csharp
using Microsoft.Extensions.DependencyInjection;
using YouTubeStreamDownloader.Services;

var services = new ServiceCollection();
services.AddYouTubeMetadataService();
var provider = services.BuildServiceProvider();

var metadataService = provider.GetRequiredService<IYouTubeMetadataService>();

var playlists = await metadataService.GetAllPlaylistsAsync("https://www.youtube.com/@YourChannel");

foreach (var playlist in playlists)
{
    Console.WriteLine($"{playlist.Title}: {playlist.Url}");
}
```

---

### **4️⃣ Fetch All Videos from a Playlist**
```csharp
using Microsoft.Extensions.DependencyInjection;
using YouTubeStreamDownloader.Services;

var provider = new ServiceCollection()
    .AddYouTubeMetadataService()
    .BuildServiceProvider();

var metadataService = provider.GetRequiredService<IYouTubeMetadataService>();

var videos = await metadataService.GetAllVideosFromPlaylistAsync("https://www.youtube.com/playlist?list=YOUR_PLAYLIST_ID");

foreach (var video in videos)
{
    Console.WriteLine($"{video.Title}: {video.Url}");
}
```

---

## 🏗 Dependency Injection (DI)
To use **YouTubeStreamDownloader** in an **ASP.NET Core** or **Console Application**, register it in **`Program.cs`**:

```csharp
using Microsoft.Extensions.DependencyInjection;
using YouTubeStreamDownloader.Services;

var builder = WebApplication.CreateBuilder(args);

// Register the service
builder.Services.AddYouTubeMetadataService();

var app = builder.Build();
```

For **Singleton DI**:
```csharp
builder.Services.AddYouTubeMetadataSingletonService();
```

---

## 🧪 Unit Testing
Unit tests use **NSubstitute for mocking** and **FluentAssertions for validation**.

Run all tests:
```sh
dotnet test
```

---

## 💡 Roadmap
- [ ] **Audio-only downloads**
- [ ] **Download progress tracking**
- [ ] **Convert video format (MP4, MP3)**
- [ ] **Multi-threaded downloads**
- [ ] **User authentication for private videos**
- [ ] **YouTube Live Stream support**

---

## 🤝 Contributing
### **Want to contribute?**  
Follow these steps:
1. **Fork the repository**.
2. **Clone your fork**:  
   ```sh
   git clone https://github.com/your-username/YouTubeStreamDownloader.git
   ```
3. **Create a new branch**:  
   ```sh
   git checkout -b feature/your-feature-name
   ```
4. **Make changes and commit**:
   ```sh
   git commit -m "Added new feature"
   ```
5. **Push to your fork**:
   ```sh
   git push origin feature/your-feature-name
   ```
6. **Create a Pull Request (PR)**.

---

## 📝 License
**YouTubeStreamDownloader** is licensed under the [MIT License](LICENSE).

---

## 📌 Contact
👤 **Author**: **Shady Nagy**  
📧 **Email**: [info@ShadyNagy.com](mailto:info@ShadyNagy.com)  
📌 **GitHub**: [ShadyNagy](https://github.com/ShadyNagy)  
🌍 **Website**: [https://shadynagy.com](https://shadynagy.com)  
