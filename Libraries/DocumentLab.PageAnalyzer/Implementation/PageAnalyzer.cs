namespace DocumentLab.PageAnalyzer.Implementation
{
  using Contracts;
  using Contracts.Ocr;
  using Contracts.PageInterpreter;
  using Interfaces;
  using TextAnalyzer.Interfaces;
  using Core.Extensions;
  using System;
  using System.Linq;
  using System.Collections.Generic;

  public class PageAnalyzer : IPageAnalyzer
  {
    private readonly ITextAnalyzer textAnalyzer;

    public PageAnalyzer(ITextAnalyzer textAnalyzer)
    {
      this.textAnalyzer = textAnalyzer;
    }

    public int[] GetHorizontalIndices(IEnumerable<OcrResult> ocrResults)
    {
      return ocrResults
        .Select(x => x.BoundingBox.X.RoundOff(Constants.PageAnalyzerConfiguration["RoundOffX"]))
        .Distinct()
        .OrderBy(x => x)
        .ToArray();
    }

    public int[] GetVerticalIndices(IEnumerable<OcrResult> ocrResults)
    {
      return ocrResults
        .Select(x => x.BoundingBox.Y.RoundOff(Constants.PageAnalyzerConfiguration["RoundOffY"]))
        .Distinct()
        .OrderBy(x => x)
        .ToArray();
    }

    public Page PerformPageAnalysis(IEnumerable<OcrResult> ocrResults)
    {
      var horizontalIndices = GetHorizontalIndices(ocrResults).ToList();
      var verticalIndices = GetVerticalIndices(ocrResults).ToList();
      var pageContents = new List<PageUnit>[horizontalIndices.Count(), verticalIndices.Count()];

      ocrResults.ToList().ForEach(result =>
      {
        var closestIndexPair = FindClosestIndexPair(result, horizontalIndices, verticalIndices);

        if (pageContents[closestIndexPair.Item1, closestIndexPair.Item2] == null)
          pageContents[closestIndexPair.Item1, closestIndexPair.Item2] = new List<PageUnit>();

        pageContents[closestIndexPair.Item1, closestIndexPair.Item2].AddRange(
          textAnalyzer
            .AnalyzeOcrResult(result)
            .Select(x => new PageUnit()
            {
              Coordinate = new Coordinate()
              {
                X = closestIndexPair.Item1,
                Y = closestIndexPair.Item2
              },
              TextType = x.TextType,
              Value = x.Text,
              BoundingBox = new PageUnitBoundingBox()
              {
                Height = result.BoundingBox.Height,
                Width = result.BoundingBox.Width,
                X = result.BoundingBox.X,
                Y = result.BoundingBox.Y
              },
              CountourOffset = new PageUnitBoundingBox()
              {
                Height = result.ContourOffset.Height,
                Width = result.ContourOffset.Width,
                X = result.ContourOffset.X,
                Y = result.ContourOffset.Y
              }
            })
        );
      });

      return new Page()
      {
        Contents = pageContents
      };
    }

    private static Tuple<int, int> FindClosestIndexPair(OcrResult ocrResult, List<int> horizontalIndices, List<int> verticalIndices)
    {
      int xPosition = ocrResult.BoundingBox.X, 
        yPosition = ocrResult.BoundingBox.Y, 
        xIndex = 0, 
        yIndex = 0;

      xIndex = FindClosestIndex(horizontalIndices, xPosition);
      yIndex = FindClosestIndex(verticalIndices, yPosition);

      return Tuple.Create(xIndex, yIndex);
    }

    private static int FindClosestIndex(List<int> indices, int value)
    {
      for (int i = 1; i < indices.Count(); i++)
      {
        if (indices[i] > value)
        {
          int distanceToNext = Math.Abs(indices[i] - value);
          int distanceToPrevious = Math.Abs(indices[i - 1] - value);

          if (distanceToNext > distanceToPrevious)
            return i - 1;
          else
            return i;
        }
      }

      return indices.Count() - 1;
    }
  }
}
