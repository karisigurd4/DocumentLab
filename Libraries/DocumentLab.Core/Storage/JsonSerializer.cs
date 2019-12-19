namespace DocumentLab.Core.Storage
{
  using Newtonsoft.Json;

  public static class JsonSerializer
  {
    public static string Serialize(object obj)
    {
      return JsonConvert.SerializeObject(obj);
    }

    public static T FromFile<T>(string path)
    {
      return JsonConvert.DeserializeObject<T>(FileReader.GetFileContent(path));
    }

    public static T FromText<T>(string text)
    {
      try
      {
        return JsonConvert.DeserializeObject<T>(text);
      }
      catch (System.Exception e)
      {
        return default(T);
      }
    }
  }
}


