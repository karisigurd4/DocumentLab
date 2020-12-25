namespace DocumentLab.TextAnalyzer.Utilities
{
  using System.IO;
  using System.Linq;
  
  public static class TextTypeHelper
  {
    public static string[] GetDefinedTextTypes()
    {
      return Directory
        .GetFiles((System.AppDomain.CurrentDomain.RelativeSearchPath ?? System.AppDomain.CurrentDomain.BaseDirectory) + "\\" + Constants.TextTypeDataFilePath)
          .Select(x => Path.GetFileNameWithoutExtension(x))
        .Union
        (
          Constants.TextTypeDefinitions.Select(x => x.Name)
        )
        .Union
        (
          new string[]
          {
            "AmountOrNumber",
            "Amount",
            "Date",
            "Email",
            "Letters",
            "Number",
            "PageNumber",
            "Percentage",
            "Text",
            "WebAddress"
          }
        )
        .ToArray();
    }
  }
}
