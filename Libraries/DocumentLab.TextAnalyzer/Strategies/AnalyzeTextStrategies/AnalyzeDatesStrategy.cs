namespace DocumentLab.TextAnalyzer.Strategies.AnalyzeTextStrategies
{
  using Contracts;
  using Contracts.Ocr;
  using Contracts.Extensions;
  using Interfaces;
  using System;
  using System.Linq;
  using System.Globalization;
  using System.Collections.Generic;
  using System.Text.RegularExpressions;

  public class AnalyzeDatesStrategy : IAnalyzeTextStrategy
  {
    public IEnumerable<AnalyzedText> Analyze(OcrResult ocrResult)
    {
      var possibleDates = Constants.TextAnalysisConfiguration.DateRegexes
        .Select(x => Regex.Matches(ocrResult.AsContinuousText(), x))
        .SelectMany(y => y.Cast<Match>()
        .Select(x => x.Value));

      return possibleDates
        .Select(x => new AnalyzedText()
        {
          Text = TryParseAndFixDate(x),
          TextType = TextType.Date.ToString(),
          BoundingBox = ocrResult.BoundingBox
        })
        .Where(x => !string.IsNullOrWhiteSpace(x.Text))
        .Distinct();
    }

    private static string TryParseAndFixDate(string date)
    {
      if(date.Where(c => Constants.TextAnalysisConfiguration.KnownDateDelimiters.Contains(c.ToString())).Count() == 0)
      {
        int delimiterOffset =
          date.Length == 6 ?
          2 :
          0;

        if(date.Count() > 4 - delimiterOffset)
          date = date.Insert(4, "-");

        if(date.Count() > 7 - delimiterOffset)
          date = date.Insert(7, "-");
      }

      DateTime outDate = default(DateTime);
      foreach(var format in Constants.TextAnalysisConfiguration.TryParseDateTimeFormats)
      {
        if (DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out outDate))
          break;
      }

      if (outDate == default(DateTime))
      {
        return string.Empty;
      }

      return outDate.ToString(Constants.TextAnalysisConfiguration.ConvertDatesToFormat);
    }
  }
}
