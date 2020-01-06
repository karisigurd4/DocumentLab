namespace DocumentLab.TextAnalyzer.Strategies.AnalyzeTextStrategies
{
  using Contracts;
  using Contracts.Ocr;
  using Contracts.Extensions;
  using Interfaces;
  using System.Linq;
  using System.Collections.Generic;
  using System.Text.RegularExpressions;

  public class AnalyzeNumbersStrategy : IAnalyzeTextStrategy
  {
    public IEnumerable<AnalyzedText> Analyze(OcrResult ocrResult)
    {
      var continuousText = ocrResult.AsContinuousText().ReplaceFaultyCharacters(Constants.NumericAnalysisOcrFixDictionary);

      var continuousNumericSequences = Constants.TextAnalysisConfiguration.NumberRegexes
        .Select(x => Regex.Matches(continuousText.Replace("-", "").Replace(".00", "").Replace(",00", ""), x))
        .SelectMany(y => y.Cast<Match>()
        .Select(x => (x.Value)));

      return continuousNumericSequences
        .Select(x => new AnalyzedText()
        {
          Text = x,
          TextType = TextType.Number.ToString(),
          BoundingBox = ocrResult.BoundingBox
        });
    }
  }
}
