using System;
using System.Threading.Tasks;

namespace YouTubeStreamDownloader.Services;

public abstract class BaseService
{
  protected async Task<T> ExecuteWithExceptionHandlingAsync<T>(Func<Task<T>> action, string errorMessage)
  {
    try
    {
      return await action();
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"{errorMessage}: {ex.Message}", ex);
    }
  }
}
