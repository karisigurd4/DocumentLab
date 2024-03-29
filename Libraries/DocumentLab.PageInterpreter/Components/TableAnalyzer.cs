﻿namespace DocumentLab.PageInterpreter.Components
{
  using DocumentLab.Contracts;
  using DocumentLab.Contracts.PageInterpreter;
  using DocumentLab.Core.Utils;
  using DocumentLab.PageAnalyzer.Implementation;
  using DocumentLab.PageInterpreter.DataModel;
  using DocumentLab.PageInterpreter.Interfaces;
  using Newtonsoft.Json;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class TableAnalyzer : ITableAnalyzer
  {
    public InterpreterResult AnalyzeTable(Page page, string tableName, TableColumn[] tableColumns)
    {
      var columns = GetAnalyzedTableColumns(page, tableColumns);
      if (columns == null || columns.Length == 0)
        return null;

      var analyzedRowCoordinates = GetAnalyzedRowCoordinates(columns);
      if (analyzedRowCoordinates == null || analyzedRowCoordinates.Length == 0)
        return null;

      for (int i = 0; i < columns.Count(); i++)
      {
        columns[i].Rows = columns[i].Rows.Where(x => analyzedRowCoordinates.Any(z => z == x.Coordinate.Y)).ToArray();
      }

      var table = new List<Dictionary<string, string>>();
      for (int i = 0; i < analyzedRowCoordinates.Count(); i++)
      {
        Dictionary<string, string> results = new Dictionary<string, string>();

        for (int x = 0; x < columns.Length; x++)
        {
          results.Add(tableColumns[columns[x].ColumnIndex].ColumnName, columns[x].Rows.Count() > i ? columns[x].Rows[i].Value : string.Empty);
        }
        table.Add(results);
      }

      return new InterpreterResult()
      {
        Results = new Dictionary<string, PatternResult>()
        {
          {
            tableName,
            new PatternResult()
            {
              Result = table.Select((x, i) => new
              {
                Index = i,
                Json = JsonConvert.SerializeObject(x)
              })
              .ToDictionary(x => x.Index.ToString(), x => x.Json)
            }
          }
        }
      };
    }

    private int[] GetAnalyzedRowCoordinates(AnalyzedTableColumns[] columns)
    {
      var analyzedRowCoordinates = columns
        .SelectMany(x => x.Rows.Select(y => y.Coordinate.Y))
        .GroupBy(x => x)
        .Where(x => x.Count() > Constants.TableAnalyzer.Configuration.MinimumNumberOfColumnsForRow)
        .Select(x => x.First())
        .ToArray();

      if (analyzedRowCoordinates.Count() == 0)
        return null;

      var continuousRows = new List<int>();
      continuousRows.Add(analyzedRowCoordinates.First());
      for (int i = 1; i < analyzedRowCoordinates.Count(); i++)
      {
        if (analyzedRowCoordinates[i] - continuousRows.Last() < Constants.TableAnalyzer.Configuration.MinimumDistanceFromLastRow)
        {
          continuousRows.Add(analyzedRowCoordinates[i]);
        }
        else
        {
          break;
        }
      }

      for (int i = 0; i < columns.Count(); i++)
      {
        columns[i].Rows = columns[i].Rows.Where(x => continuousRows.Any(z => z == x.Coordinate.Y)).ToArray();
      }

      return continuousRows.ToArray();
    }

    private AnalyzedTableColumns[] GetAnalyzedTableColumns(Page page, TableColumn[] tableColumns)
    {
      // Experimentation shows that trimming the Y index more can help normalise table headers
      var tableAnalysisPage = new PageTrimmer().TrimPage(page, 20, 20, true);

      // This finds the row in the page that can best match the TextType and label definitions along with the indices where they match. 
      var bestMatch = tableColumns
        .SelectMany((tableColumn, TableIndex) => tableAnalysisPage
          .GetIndexWhere(t =>
            tableColumn.LabelParameters.Any(v => FuzzyTextComparer.FuzzyEquals(v, t.Value))
          )
          .Select(Index => new
          {
            TableIndex,
            Index,
            tableColumn.TextType
          }))
        .GroupBy(x => x.Index.Coordinate.Y)
        .OrderByDescending(x => x.Count())
        .FirstOrDefault();

      if (bestMatch == null)
      {
        return null;
      }

      var columns = bestMatch
        .GroupBy(x => x.TableIndex)
        .Select(x => x.First())
        .Select(x => new AnalyzedTableColumns()
        {
          Rows = GetTableColumn(page, (PageIndex)x.Index.Clone(), x.TextType),
          ColumnIndex = x.TableIndex
        })
        .ToArray();

      return columns;
    }

    private PageUnit[] GetTableColumn(Page page, PageIndex index, string type)
    {
      bool stopAtNextEmpty = false;
      bool active = true;
      var columnMatches = new List<PageUnit>();

      var traverser = new PageTraverser(page, index);
      traverser.Traverse(Direction.Down, 1);

      while (active && !traverser.ErrorOccurred)
      {
        string match = string.Empty;
        var surroundingPageUnits = new PageUnit[9][]
        {
          traverser.Peek(Direction.Left),
          traverser.GetCurrentPageUnits(),
          traverser.Peek(Direction.Right),
          traverser.Peek(Direction.Left, 2),
          traverser.Peek(Direction.Right, 2),
          traverser.Peek(Direction.Left, 3),
          traverser.Peek(Direction.Right, 3),
          traverser.Peek(Direction.Left, 4),
          traverser.Peek(Direction.Right, 4)
        };

        var pageUnits = surroundingPageUnits
          .Where(x => x != null)
          .Where(x => x.Any(p => p.TextType == type))
          .OrderBy(x => Math.Abs(traverser.GetCurrentPosition().Coordinate.X - x.First().Coordinate.X));

        var matchingPageUnit = pageUnits.FirstOrDefault();

        if (matchingPageUnit != null)
          columnMatches.Add(matchingPageUnit.Where(x => x.TextType == type).First());

        traverser.Traverse(Direction.Down, 1);

        if (!string.IsNullOrWhiteSpace(match))
          stopAtNextEmpty = true;

        if (string.IsNullOrWhiteSpace(match) && stopAtNextEmpty)
          active = false;
      }

      return columnMatches.ToArray();
    }
  }
}
