namespace DocumentLab.TextAnalyzer
{
  using DocumentLab.Core.Storage;
  using DocumentLab.TextAnalyzer.Configuration;
  using System;
  using System.Collections.Generic;
  using System.Configuration;

  public static class Constants
  {
    // Temp
    public static bool ValidateWebAddresses = false;
    
    // Configuration references
    public static int NumberOfThreads => Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfThreads"]);
    public static string TextTypeDataFilePath = "Data\\Context\\";
    public static string TopLevelDomainListPath = "Data\\Static\\topleveldomains.txt";
    public static string TextAnalysisConfigurationPath = "data\\configuration\\TextAnalysis.json";
    public static string TextAnalysisOcrFixDictionaryPath = "data\\configuration\\NumericAnalysisOcrFix.json";
    public static string SpecialCharactersTranslationDictionaryPath = "data\\configuration\\SpecialCharactersTranslation.json";
    public static string FromFileConfigurationPath = "data\\configuration\\FromFileConfiguration.json";

    // Configuration objects
    public static TextAnalysisConfiguration TextAnalysisConfiguration = JsonSerializer.FromFile<TextAnalysisConfiguration>(TextAnalysisConfigurationPath);
    public static Dictionary<string, string> NumericAnalysisOcrFixDictionary = JsonSerializer.FromFile<Dictionary<string, string>>(Constants.TextAnalysisOcrFixDictionaryPath);
    public static Dictionary<string, string> SpecialCharactersTranslationDictionary = JsonSerializer.FromFile<Dictionary<string, string>>(Constants.SpecialCharactersTranslationDictionaryPath);
    public static FromFileConfiguration[] FromFileConfigurations = JsonSerializer.FromFile<FromFileConfiguration[]>(Constants.FromFileConfigurationPath);
  }
}
