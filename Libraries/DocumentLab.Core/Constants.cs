namespace DocumentLab.Core
{
  using System;

  public static class Constants
  {
    public static string ApplicationGlobalNamespace => "DocumentLab";
    public static string StrategySuffix => "Strategy";
    public static Func<string, string> MissingStrategyImplementation = (operation) => $"No implementation of {operation} was found";
    public static int FuzzyStringLengthLimit = 5;
  }
}
