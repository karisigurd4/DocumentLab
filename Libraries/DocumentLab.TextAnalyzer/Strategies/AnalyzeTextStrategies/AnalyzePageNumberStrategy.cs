namespace DocumentLab.TextAnalyzer.Strategies.AnalyzeTextStrategies
{
  using Contracts;
  using Contracts.Ocr;
  using Contracts.Extensions;
  using TextAnalyzer.Interfaces;
  using System.Linq;
  using System.Collections.Generic;
  using System.Text.RegularExpressions;

  public class AnalyzePageNumberStrategy : IAnalyzeTextStrategy
  {
    public IEnumerable<AnalyzedText> Analyze(OcrResult ocrResult)
    {
      var firstPassPageNumbers = Constants.TextAnalysisConfiguration.FirstPassPageNumberRexeges
        .Select(x => Regex.Matches(ocrResult.AsContinuousText(false), x, RegexOptions.IgnoreCase))
        .SelectMany(y => y.Cast<Match>()
        .Select(x => (x.Groups["page"])))
        .FirstOrDefault()
        ?.Value;

      if (string.IsNullOrWhiteSpace(firstPassPageNumbers))
        return new AnalyzedText[] { };

      var secondPassPageNumber = Constants.TextAnalysisConfiguration.SecondPassPageNumberRegexes
        .Select(x => Regex.Matches(firstPassPageNumbers, x, RegexOptions.IgnoreCase))
        .SelectMany(y => y.Cast<Match>()
        .Select(x => (x.Groups["page"])))
        .FirstOrDefault()
        ?.Value;

      if (string.IsNullOrWhiteSpace(secondPassPageNumber))
        return new AnalyzedText[] { };

      return new AnalyzedText[]
      {
        new AnalyzedText()
        {
          Text = secondPassPageNumber,
          TextType = TextType.PageNumber.ToString(),
          BoundingBox = ocrResult.BoundingBox
        }
      };
    }
  }
}
