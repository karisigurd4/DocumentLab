namespace DocumentLab.TextAnalyzer.Strategies.AnalyzeTextStrategies
{
  using Contracts;
  using Contracts.Ocr;
  using Contracts.Extensions;
  using TextAnalyzer.Interfaces;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;
  using DocumentLab.TextAnalyzer.Configuration;
  using System;

  public class AnalyzeCustomTextTypeDefinitionsStrategy : IAnalyzeTextStrategy
  {
    public IEnumerable<AnalyzedText> Analyze(OcrResult ocrResult)
    {
      var analyzed = Constants.TextTypeDefinitions
        .SelectMany(x => ParseFromTextTypeDefintion(x, ocrResult)
        .Select(y => new AnalyzedText()
        {
          Text = y,
          TextType = x.Name,
          BoundingBox = ocrResult.BoundingBox
        }));

      return analyzed;
    }

    private string[] ParseFromTextTypeDefintion(TextTypeDefinition textTypeDefinition, OcrResult ocrResult)
    {
      if (string.IsNullOrWhiteSpace(textTypeDefinition.Name))
      {
        throw new Exception("Could not analyze custom text type definition, a definition is missing a name");
      }

      if (textTypeDefinition.Regexes.Length == 0)
      {
        return new string[] { };
      }

      if (ocrResult.AsString() is var text && textTypeDefinition.Text != null)
      {
        text = ParseTextDefinition(textTypeDefinition.Text, ocrResult);
      }

      var matches = textTypeDefinition.Regexes
        .Select(x => Regex.Matches(text, x, RegexOptions.IgnoreCase))
        .SelectMany(x => x.Cast<Match>())
        .Select(x => x.Value).ToArray();

      return matches;
    }

    private string ParseTextDefinition(TextDefinition textDefinition, OcrResult ocrResult)
    {
      string text = string.Empty;
      switch (textDefinition.GetAs)
      {
        case GetTextAs.Text:
          text = ocrResult.AsString();
          break;
        case GetTextAs.Continuous:
          text = ocrResult.AsContinuousText();
          break;
      }

      if (textDefinition.Replace != null && textDefinition.Replace.Length > 0)
      {
        foreach (var replaceDefinition in textDefinition.Replace)
        {
          text = ParseReplaceDefinition(replaceDefinition, text);
        }
      }

      return text;
    }

    private string ParseReplaceDefinition(ReplaceDefintion replaceDefintion, string text)
    {
      foreach (var find in replaceDefintion.Find)
      {
        text = Regex.Replace(text, find, replaceDefintion.Replace);
      }

      return text;
    }
  }
}
