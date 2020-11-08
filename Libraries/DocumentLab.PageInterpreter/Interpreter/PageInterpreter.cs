namespace DocumentLab.PageInterpreter.Interpreter
{
  using Antlr4.Runtime.Misc;
  using Components;
  using Contracts;
  using Contracts.PageInterpreter;
  using Core.Utils;
  using DataModel;
  using Enum;
  using Grammar;
  using Interfaces;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class PageInterpreter : PageInterpreterBaseVisitor<Symbol>
  {
    public InterpreterResult Result { get; private set; } = new InterpreterResult();
    public PatternResult currentPatternResult = null;

    private string queryLabel;
    private readonly Page page;
    private IPageTraverser pageTraverser;

    public PageInterpreter(Page page)
    {
      this.page = page;
    }

    private bool ValidateTextTypeMatch(PageUnit pageUnit, string textType, string[] matchContent)
      => pageUnit != null
        && pageUnit?.TextType == textType
        && (matchContent?.Any(x => FuzzyTextComparer.FuzzyEquals(x, pageUnit.Value))
        ?? true);

    private string[] GetTextTypeParameters(PageInterpreterParser.TextTypeContext context, int index)
      => context.textTypeParameters(index) != null
        ? context?.textTypeParameters(index).Accept(this)?.GetValue<string[]>() ?? null
        : null;

    public override Symbol VisitSubset([NotNull] PageInterpreterParser.SubsetContext context)
    {
      return new Symbol(
        SymbolType.Array,
        context
          .Parameters().GetText().Substring(1, context.Parameters().GetText().Length - 2)
          .Split(',')
          .Select(x => new SubsetParameter()
          {
            Percentage = int.Parse(x.Split(' ')[1]),
            SubsetPart = (SubsetPart)Enum.Parse(typeof(SubsetPart), x.Split(' ')[0])
          }).ToDictionary(x => x.SubsetPart, x => x.Percentage));
    }

    public override Symbol VisitQuery([NotNull] PageInterpreterParser.QueryContext context)
    {
      queryLabel = context?.Text()?.GetText() ?? Result.Results.Count.ToString();

      base.VisitQuery(context);

      return new Symbol(SymbolType.Success);
    }

    public override Symbol VisitTable([NotNull] PageInterpreterParser.TableContext context)
    {
      var tableColumns = context.tableColumn().Select(x => (TableColumn)x.Accept(this).Value).ToArray();
      var tableAnalyzer = new TableAnalyzer();
      var result = tableAnalyzer.AnalyzeTable(page, queryLabel, tableColumns);
      if (result != null || result?.Results?.Count > 0)
      {
        Result.Results.Add(queryLabel, result.Results.First().Value);
      }
      return new Symbol(SymbolType.Success);
    }

    public override Symbol VisitTableColumn([NotNull] PageInterpreterParser.TableColumnContext context)
    {
      var propertyName = (string)context.propertyName().Accept(this).Value;
      var textTypeParameters = GetTextTypeParameters(context.textType(), 0);
      var textType = context.textType().Text(0).GetText();

      return new Symbol(SymbolType.Single, new TableColumn()
      {
        ColumnName = propertyName,
        LabelParameters = textTypeParameters,
        TextType = textType
      });
    }

    private int? calculateSubset(int length, int? percentage)
      => percentage.HasValue
        ? (int)(length * (0.01M * percentage))
        : (int?)null;

    private Dictionary<SubsetPart, int> subsetPartToPageDimension = new Dictionary<SubsetPart, int>()
    {
      { SubsetPart.Top, 1 }, { SubsetPart.Bottom, 1 },
      { SubsetPart.Left, 0 }, {SubsetPart.Right, 0 }
    };

    private int calculateSubsetOnPage(Dictionary<SubsetPart, int> subsets, SubsetPart subset)
      => subsets != null
          ? calculateSubset
            (
              page.Contents.GetLength(subsetPartToPageDimension[subset]),
              subsets.ContainsKey(subset)
                ? subsets?[subset]
                : null
            ) ?? page.Contents.GetLength(subsetPartToPageDimension[subset])
          : page.Contents.GetLength(subsetPartToPageDimension[subset]);

    public override Symbol VisitPattern([NotNull] PageInterpreterParser.PatternContext context)
    {
      var onlyFirstCaptured = context.Any() == null ? true : false;

      var subsets = context?.subset()?.Accept(this)?.GetValue<Dictionary<SubsetPart, int>>();

      for
      (
        int x = page.Contents.GetLength(0) - calculateSubsetOnPage(subsets, SubsetPart.Right);
        x < calculateSubsetOnPage(subsets, SubsetPart.Left);
        x++
      )
      {
        for
        (
          int y = page.Contents.GetLength(1) - calculateSubsetOnPage(subsets, SubsetPart.Bottom);
          y < calculateSubsetOnPage(subsets, SubsetPart.Top);
          y++
        )
        {
          if (page.Contents[x, y] == null || page.Contents[x, y].Count == 0)
            continue;

          pageTraverser = new PageTraverser(page, new PageIndex(x, y));

          currentPatternResult = new PatternResult();

          try
          {
            base.VisitPattern(context);
            if (currentPatternResult != null)
            {
              foreach (var item in currentPatternResult.Result)
              {
                Result.AddResult(queryLabel, item.Key, item.Value);
              }
            }
            if (onlyFirstCaptured)
            {
              x = page.Contents.GetLength(0);
              break;
            }
          }
          catch (ParseCanceledException)
          {
            currentPatternResult = null;
          }
        }
      }

      return new Symbol(SymbolType.Success);
    }

    public override Symbol VisitRightDownSearch([NotNull] PageInterpreterParser.RightDownSearchContext context)
    {
      var maxSteps = int.Parse(context.Steps.Text);
      var results = new List<Tuple<Direction, TraversalResult>>();
      var currentPageTraverser = pageTraverser;
      var rightPageTraverser = (PageTraverser)pageTraverser.Clone();
      var downPageTraverser = (PageTraverser)pageTraverser.Clone();

      results.Add(Tuple.Create(Direction.Right, rightPageTraverser.Traverse(Direction.Right, maxSteps)));
      results.Add(Tuple.Create(Direction.Down, downPageTraverser.Traverse(Direction.Down, maxSteps)));

      results = results.Where(x => x.Item2 != null && x.Item2.Steps < maxSteps).OrderBy(x => x.Item2.Steps).ToList();

      foreach (var result in results)
      {
        try
        {
          pageTraverser = result.Item1 == Direction.Right ? rightPageTraverser : downPageTraverser;
          base.VisitRightDownSearch(context);
          return new Symbol(SymbolType.Success);
        }
        catch { }
      }

      throw new ParseCanceledException();
    }

    public override Symbol VisitTextType([NotNull] PageInterpreterParser.TextTypeContext context)
    {
      for (int i = 0; i < context.Text().Length; i++)
      {
        var textType = context.Text(i).GetText();
        var pageUnit = pageTraverser.GetMatchingPageUnit(textType);
        var matchContent = GetTextTypeParameters(context, i);

        if (ValidateTextTypeMatch(pageUnit, textType, matchContent))
        {
          return new Symbol(SymbolType.Single, textType);
        }
      }

      throw new ParseCanceledException();
    }

    public override Symbol VisitTextTypeParameters([NotNull] PageInterpreterParser.TextTypeParametersContext context)
    {
      if (context.Parameters() == null)
      {
        return null;
      }

      return new Symbol
      (
        SymbolType.Array,
        context.Parameters().GetText()
          .Substring(1, context.Parameters().GetText().Length - 2)
          .Replace("||", "~")
          .Split('~')
      );
    }

    public override Symbol VisitTraverse([NotNull] PageInterpreterParser.TraverseContext context)
    {
      pageTraverser.Traverse((Direction)Enum.Parse(typeof(Direction), context.Direction.Text));

      if (pageTraverser.ErrorOccurred)
      {
        throw new ParseCanceledException();
      }

      return new Symbol(SymbolType.Success);
    }

    public override Symbol VisitPropertyName([NotNull] PageInterpreterParser.PropertyNameContext context)
    {
      return new Symbol(SymbolType.Single, context.Text().GetText());
    }

    public override Symbol VisitCapture([NotNull] PageInterpreterParser.CaptureContext context)
    {
      var matchedTextType = context.textType().Accept(this).GetValue<string>();
      var propertyName = context.propertyName()?.Accept(this).GetValue<string>();
      var capturedValue = pageTraverser.GetMatchingPageUnit(matchedTextType)?.Value;

      if (!string.IsNullOrWhiteSpace(capturedValue))
      {
        currentPatternResult.AddResult(propertyName, capturedValue);
      }

      return new Symbol(SymbolType.Success);
    }
  }
}
