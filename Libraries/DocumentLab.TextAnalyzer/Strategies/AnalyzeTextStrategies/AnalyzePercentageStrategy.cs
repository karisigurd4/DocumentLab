namespace DocumentLab.TextAnalyzer.Strategies.AnalyzeTextStrategies
{
  using DocumentLab.TextAnalyzer.Interfaces;
  using DocumentLab.Contracts;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;
  using DocumentLab.Contracts.Enums.Types;
  using DocumentLab.Contracts.Extensions;

  public class AnalyzePercentageStrategy : IAnalyzeTextStrategy
  {
    public IEnumerable<AnalyzedText> Analyze(OcrResult ocrResult)
    {
      var possibleNames = Constants.TextAnalysisConfiguration.PercentageRegexes
        .Select(x => Regex.Matches(ocrResult.AsContinuousText().Replace("-", ""), x, RegexOptions.IgnoreCase))
        .SelectMany(x => x.Cast<Match>());

      return possibleNames.Select(x => new AnalyzedText()
      {
        Text = x.Value,
        TextType = TextType.Percentage.ToString(),
        BoundingBox = ocrResult.BoundingBox
      });
    }
  }
}