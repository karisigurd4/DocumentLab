namespace DocumentLab.TextAnalyzer.Strategies.AnalyzeTextStrategies
{
  using System.Collections.Generic;
  using DocumentLab.Contracts;
  using DocumentLab.TextAnalyzer.Interfaces;
  using System.Linq;
  using System.Text.RegularExpressions;
  using DocumentLab.Contracts.Enums.Types;

  public class AnalyzeTextStrategy : IAnalyzeTextStrategy
  {
    public IEnumerable<AnalyzedText> Analyze(OcrResult ocrResult)
    {
      var matchedText = ocrResult.Result
        .Select(x => Regex.Matches(x, Constants.TextAnalysisConfiguration.TextRegex, RegexOptions.IgnoreCase))
        .SelectMany(x => x.Cast<Match>());

      matchedText = matchedText
        .Where(x => !x.Value.All(y => char.IsDigit(y)));

      return matchedText.Select(x => new AnalyzedText()
      {
        Text = x.Value,
        TextType = TextType.Text.ToString(),
        BoundingBox = ocrResult.BoundingBox
      });
    }
  }
}
