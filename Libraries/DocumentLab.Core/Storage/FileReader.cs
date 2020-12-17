namespace DocumentLab.Core.Storage
{
  using AutoCacheLib.Implementation;
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Runtime.Caching;

  public static class FileReader
  {
    public static IEnumerable<string> GetFileLinesOrdered(string filePath)
    {
      var cacheKey = $"cache-orderedlines-{filePath}";

      if (MemoryCache.Default.Contains(cacheKey))
        return (IEnumerable<string>)MemoryCache.Default[cacheKey];

      var orderedLines = GetFileLines(filePath)
          .OrderBy(x => x)
          .ToList();

      MemoryCache.Default.Add(new CacheItem(cacheKey, orderedLines), new CacheItemPolicy());

      return orderedLines;
    }

    public static IEnumerable<string> GetFileLines(string filePath)
    {
      var cacheKey = $"cache-filelines-{filePath}";

      if (MemoryCache.Default.Contains(cacheKey))
        return (IEnumerable<string>)MemoryCache.Default[cacheKey];

      var fileLines =  GetFileContent(Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory, filePath))
          .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

      MemoryCache.Default.Add(new CacheItem(cacheKey, fileLines), new CacheItemPolicy());

      return fileLines;
    }

    public static string GetFileContent(string filePath)
    {
      var cacheKey = $"cache-filecontents-{filePath}";

      if (MemoryCache.Default.Contains(cacheKey))
        return (string)MemoryCache.Default[cacheKey];

      var fileContent = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory, filePath));
      
      MemoryCache.Default.Add(new CacheItem(cacheKey, fileContent), new CacheItemPolicy());

      return fileContent;
    }
  }
}
