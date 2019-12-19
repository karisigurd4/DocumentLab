namespace DocumentLab.TextAnalyzer.Utilities
{
  using System;
  using System.Net.Http;

  public static class HttpClientHelpers
  {
    public static bool IsWebsiteAvailable(Uri webAddress)
    {
      using (var httpClient = new HttpClient())
      {
        try
        {
          var result = httpClient.GetAsync(webAddress).Result;
          return result.IsSuccessStatusCode;
        }
        catch
        {
          return false;
        }
      }
    }
  }
}
