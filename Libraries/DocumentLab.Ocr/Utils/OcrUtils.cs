namespace DocumentLab.Ocr.Utils
{
  using Contracts.Ocr;
  using Core.Extensions;
  using System.Collections.Generic;
  using System.Drawing;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Xml.Linq;

  public static class OcrUtils
  {
    public static string RemoveOcrArtifacts(string rawText)
    {
      Constants.OcrFixDictionary.ForEach(x =>
      {
        rawText = rawText.Replace(x.Key, x.Value);
      });

      Constants.HOcrModifiers.ForEach(x =>
      {
        rawText = rawText.Replace(x, string.Empty);
      });

      return rawText;
    }

    public static Dictionary<Rectangle, string> GetWords(string tesseractHtml, Rectangle offset)
    {
      var xml = XDocument.Parse(tesseractHtml);
      var rectsWords = new Dictionary<Rectangle, string>();
      var ocrLines = xml.Descendants("span").Where(e => e.Attribute("class").Value == "ocr_line").ToList();

      foreach (var line in ocrLines)
      {
        var wordsInLine = line
          .Descendants("span")
          .Where(element => element.Attribute("class").Value == "ocrx_word")
          .ToArray();

        var previousBox = ParseHocrBoundingBox(wordsInLine.FirstOrDefault(), offset);
        var currentString = wordsInLine.FirstOrDefault().Value;
        for (int i = 1; i < wordsInLine.Count(); i++)
        {
          var newBox = ParseHocrBoundingBox(wordsInLine[i], offset);
          if (
            (newBox.Left - previousBox.Right < 28)                    // Normal text
            || (i < wordsInLine.Count() - 1                           // Numbers separated by longer spaces
                && Regex.IsMatch(wordsInLine[i].Value, "\\d+")
                && Regex.IsMatch(wordsInLine[i - 1].Value, "\\d+")
                && newBox.Left - previousBox.Right < 35)
          )
          {
            previousBox.Right = newBox.Right;
            currentString += " " + wordsInLine[i].Value;
          }
          else
          {
            rectsWords.Add(HOcrBoundingBoxToRectangle(previousBox), currentString);
            previousBox = newBox;
            currentString = wordsInLine[i].Value;
          }
        }

        rectsWords.Add(HOcrBoundingBoxToRectangle(previousBox), currentString);
      }

      return rectsWords;
    }

    public static Rectangle HOcrBoundingBoxToRectangle(HOcrBoundingBox bbox)
    {
      return new Rectangle(bbox.Left, bbox.Top, bbox.Right - bbox.Left, bbox.Bottom - bbox.Top);
    }

    public static HOcrBoundingBox ParseHocrBoundingBox(XElement hocrLine, Rectangle offset)
    {
      var strs = hocrLine.Attribute("title").Value.Split(' ');

      return new HOcrBoundingBox()
      {
        Left = int.Parse(strs[1]) + offset.Left,
        Top = int.Parse(strs[2]) + offset.Top,
        Right = int.Parse(strs[3]) + offset.Left,
        Bottom = int.Parse(Regex.Match(strs[4], "\\d+").Value) + offset.Top
      };
    }
  }
}
