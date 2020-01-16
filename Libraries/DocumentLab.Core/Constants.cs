namespace DocumentLab.Core
{
  using System;
  using DocumentLab.Core.Storage;
  using System.Collections.Generic;

  public static class Constants
  {
    public static string GlobalConfigConfigurationPath = "data\\configuration\\GlobalConfiguration.json";
    public static Dictionary<string, string> GlobalConfiguration = JsonSerializer.FromFile<Dictionary<string, string>>(Constants.GlobalConfigConfigurationPath);
    public static string Language => GlobalConfiguration["Language"];

    public static string ApplicationGlobalNamespace => "DocumentLab";
    public static string StrategySuffix => "Strategy";
    public static Func<string, string> MissingStrategyImplementation = (operation) => $"No implementation of {operation} was found";
    public static int FuzzyStringLengthLimit = 5;
  }
}
