namespace DocumentLab.TextAnalyzer.Strategies.AnalyzeTextStrategies
{
  using Contracts;
  using Contracts.Ocr;
  using Contracts.Extensions;
  using TextAnalyzer.Interfaces;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;

  public class AnalyzeLettersStrategy : IAnalyzeTextStrategy
  {
    public IEnumerable<AnalyzedText> Analyze(OcrResult ocrResult)
    {
      var matchedText = Constants.TextAnalysisConfiguration.LettersRegexes
        .Select(x => Regex.Matches(ocrResult.AsString(), x))
        .SelectMany(y => y.Cast<Match>()
        .Select(x => x.Value));

      return matchedText.Select(x => new AnalyzedText()
      {
        Text = x,
        TextType = TextType.Letters.ToString(),
        BoundingBox = ocrResult.BoundingBox
      });

    }
  }
}
