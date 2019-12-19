namespace DocumentLab.TextAnalyzer.Strategies.AnalyzeTextStrategies
{
  using System.Collections.Generic;
  using Interfaces;
  using System.Linq;
  using Contracts;
  using System.Text.RegularExpressions;
  using Contracts.Enums.Types;
  using DocumentLab.Core.Storage;

  public class AnalyzeEmailsStrategy : IAnalyzeTextStrategy
  {
    IEnumerable<AnalyzedText> IAnalyzeTextStrategy.Analyze(OcrResult ocrResult)
    {
      var possibleEmails =
        string.Join(" ", ocrResult.Result)
        .Split(' ')
        .Select(x => TryParseAndFixEmail(x.Trim()))
        .Where(x => !string.IsNullOrWhiteSpace(x));

      return possibleEmails
        .Select(x => new AnalyzedText()
        {
          Text = x,
          TextType = TextType.Email.ToString(),
          BoundingBox = ocrResult.BoundingBox
        });
    }

    private string TryParseAndFixEmail(string emailAddress)
    {
      if (Regex.IsMatch(emailAddress, Constants.TextAnalysisConfiguration.EmailAddressRegex))
        return emailAddress;

      if (Regex.IsMatch(emailAddress, Constants.TextAnalysisConfiguration.BadOcrEmailAddressRegex))
      {
        for (int i = 2; i < 4; i++)
        {
          var emailTopLevelDomain = new string(
            emailAddress
              .Reverse()
              .Take(i)
              .Reverse()
              .ToArray()
            );

          if(FileReader.GetFileLines(Constants.TopLevelDomainListPath).Contains(emailTopLevelDomain.ToUpper()))
          {
            return emailAddress.Insert(emailAddress.IndexOf(emailTopLevelDomain), ".");
          }
        }
      }

      return string.Empty;
    }
  }
}
