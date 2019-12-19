namespace DocumentLab.Core.Storage
{
  using Newtonsoft.Json;
  using System.IO;

  public static class FileWriter
  {
    public static void AsJson<T>(string filePath, T data)
    {
      File.WriteAllText(filePath, JsonConvert.SerializeObject(data, Formatting.Indented));
    }
  }
}
