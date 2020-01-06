namespace DocumentLab.TextAnalyzer.Strategies.AnalyzeTextStrategies
{
  using Contracts;
  using Contracts.Ocr;
  using Contracts.Extensions;
  using TextAnalyzer.Extensions;
  using TextAnalyzer.Interfaces;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;

  public class AnalyzeFromDataFiles : IAnalyzeTextStrategy
  {
    public IEnumerable<AnalyzedText> Analyze(OcrResult ocrResult)
    {
      var files = Directory.GetFiles(Constants.TextTypeDataFilePath);

      return files.SelectMany(filePath => 
      {
        var fileName = Path.GetFileNameWithoutExtension(filePath);

        var matchedResult = ocrResult.BinarySearchMatchFromFile(fileName, filePath, false);
        if (matchedResult.Count() == 0)
          return matchedResult;

        var fromFileConfiguration = Constants.FromFileConfigurations.FirstOrDefault(x => x.TextType.Equals(fileName, System.StringComparison.InvariantCultureIgnoreCase));
        if(fromFileConfiguration != null)
        {
          if(!string.IsNullOrWhiteSpace(fromFileConfiguration.PostCaptureRegex))
          {
            return matchedResult
              .Select(x => 
              {
                var matches = new Regex(fromFileConfiguration.PostCaptureRegex.Replace("{textpart}", x.Text)).Matches(ocrResult.AsContinuousText());

                return matches;
              })
              .SelectMany(x => x.Cast<Match>()
              .Select(y => new AnalyzedText()
              {
                BoundingBox = ocrResult.BoundingBox,
                TextType = fileName,
                Text = $"{y.Groups["textpart"].Value} {y.Groups["numberpart"]?.Value?.PadLeft(1)}".Trim()
              }));
          }
        }

        return matchedResult;
      });
    }
  }
}
