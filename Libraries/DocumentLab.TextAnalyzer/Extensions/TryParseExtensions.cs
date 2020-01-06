namespace DocumentLab.TextAnalyzer.Extensions
{
  using Contracts;
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;

  public static class TryParseExtensions
  {
    public static IEnumerable<DateTime?> ParseValidDates(this IEnumerable<AnalyzedText> strings)
    {
      List<DateTime?> validDates = new List<DateTime?>();

      strings.ToList().ForEach(x =>
      {
        DateTime outDate;
        if (DateTime.TryParse(x.Text, out outDate))
          validDates.Add(outDate);
      });

      return validDates;
    }

    public static IEnumerable<decimal?> ParseValidDecimals(this IEnumerable<AnalyzedText> strings)
    {
      List<decimal?> validDecimals = new List<decimal?>();

      strings.ToList().ForEach(x =>
      {
        decimal outDecimal;
        if (decimal.TryParse(x.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out outDecimal))
          validDecimals.Add(outDecimal);
      });

      return validDecimals;
    }
  }
}
