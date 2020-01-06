namespace DocumentLab.PageAnalyzer
{
  using Core.Storage;
  using System.Collections.Generic;

  public static class Constants
  {
    public static string PageAnalyzerConfigurationPath = "data\\configuration\\PageAnalyzerConfiguration.json";

    public static Dictionary<string, int> PageAnalyzerConfiguration = JsonSerializer.FromFile<Dictionary<string, int>>(Constants.PageAnalyzerConfigurationPath);
  }
}
