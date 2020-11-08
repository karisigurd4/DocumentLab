namespace DocumentLab.Core.Storage
{
  using AutoCacheLib.Implementation;
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;

  public static class FileReader
  {
    public static IEnumerable<string> GetFileLinesOrdered(string filePath)
    {
      return AutoCache.CacheRetrieve(() =>
      {
        return GetFileLines(filePath)
          .OrderBy(x => x)
          .ToList();
      });
    }

    public static IEnumerable<string> GetFileLines(string filePath)
    {
      return AutoCache.CacheRetrieve(() =>
      {
        return GetFileContent(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath))
          .Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
      });
    }

    public static string GetFileContent(string filePath)
    {
      return AutoCache.CacheRetrieve(() =>
      {
        return File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath));
      });
    }
  }
}
