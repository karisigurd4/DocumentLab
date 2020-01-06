namespace DocumentLab.TextAnalyzer.Strategies.AnalyzeTextStrategies
{
  using Contracts;
  using Contracts.Ocr;
  using TextAnalyzer.Interfaces;
  using System.Collections.Generic;
  using System.Linq;

  public class AnalyzeAmountOrNumberStrategy : IAnalyzeTextStrategy
  {
    public IEnumerable<AnalyzedText> Analyze(OcrResult ocrResult)
    {
      var results = new AnalyzeAmountsStrategy().Analyze(ocrResult);

      if (results.Count() == 0)
        results = new AnalyzeNumbersStrategy().Analyze(ocrResult);

      return results
        .Select(x => new AnalyzedText()
        {
          BoundingBox = x.BoundingBox,
          Text = x.Text,
          TextType = TextType.AmountOrNumber.ToString()
        });
    }
  }
}
