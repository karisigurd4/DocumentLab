using DocumentLab.Core.Storage;
using System.Collections.Generic;

namespace DocumentLab.ImageProcessor
{
  public static class Constants
  {
    // Global 
    public static string StrategySuffix => "Strategy";

    public static string ImageProcessorConfigurationPath = "data\\configuration\\ImageProcessorConfiguration.json";

    public static int HighlightIntensity = JsonSerializer.FromFile<Dictionary<string, int>>(Constants.ImageProcessorConfigurationPath)["HighlightIntensity"];
  }
}
