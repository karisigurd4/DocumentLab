namespace DocumentLab.ImageProcessor
{
  using DocumentLab.Core.Storage;
  using System.Collections.Generic;
  
  public static class Constants
  {
    public static string StrategySuffix => "Strategy";

    public static string ImageProcessorConfigurationPath = "data\\configuration\\ImageProcessorConfiguration.json";

    public static Dictionary<string, int> ImageProcessorConfiguration = JsonSerializer.FromFile<Dictionary<string, int>>(Constants.ImageProcessorConfigurationPath);

    public static int HighlightIntensity = ImageProcessorConfiguration["HighlightIntensity"];
    public static int ScaleDownPercentage = ImageProcessorConfiguration["ScaleDownPercentage"];
  }
}
