namespace DocumentLab.TextAnalyzer.Strategies.AnalyzeTextStrategies
{
  using DocumentLab.Contracts;
  using DocumentLab.Contracts.Enums.Types;
  using DocumentLab.Contracts.Extensions;
  using DocumentLab.TextAnalyzer.Interfaces;
  using System;
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
