using DocumentLab.Core.Storage;
using System.Collections.Generic;

namespace DocumentLab.PageInterpreter
{
  public static class Constants
  {
    public static class TableAnalyzer
    {
      public static string TableAnalyzerConfigurationPath = "Data\\Configuration\\TableAnalyzerConfiguration.json";
      public static Dictionary<string, int> TableAnalyzerConfiguration = JsonSerializer.FromFile<Dictionary<string, int>>(TableAnalyzerConfigurationPath);

      public static class Configuration
      {
        public static int MinimumNumberOfColumnsForRow = Constants.TableAnalyzer.TableAnalyzerConfiguration["MinimumNumberOfColumnsForRow"];
        public static int MinimumDistanceFromLastRow = Constants.TableAnalyzer.TableAnalyzerConfiguration["MinimumDistanceFromLastRow"];
      }
    }
  }
}
