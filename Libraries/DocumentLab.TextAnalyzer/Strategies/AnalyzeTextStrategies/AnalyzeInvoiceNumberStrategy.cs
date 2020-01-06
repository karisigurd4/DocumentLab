namespace DocumentLab.TextAnalyzer.Strategies.AnalyzeTextStrategies
{
  using Contracts;
  using Contracts.Ocr;
  using Contracts.Extensions;
  using TextAnalyzer.Interfaces;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;

  public class AnalyzeInvoiceNumberStrategy : IAnalyzeTextStrategy
  {
    public IEnumerable<AnalyzedText> Analyze(OcrResult ocrResult)
    {
      var invoiceNumbers = Constants.TextAnalysisConfiguration.InvoiceNumberRegexes
        .Select(x => Regex.Matches(ocrResult.AsString().Replace("-", ""), x, RegexOptions.IgnoreCase))
        .SelectMany(x => x.Cast<Match>());

      return invoiceNumbers.Select(x => new AnalyzedText()
      {
        Text = x.Value,
        TextType = TextType.InvoiceNumber.ToString(),
        BoundingBox = ocrResult.BoundingBox
      });
    }
  }
}
