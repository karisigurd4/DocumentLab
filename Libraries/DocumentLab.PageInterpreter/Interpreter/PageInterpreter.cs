namespace DocumentLab.PageInterpreter.Interpreter
{
  using Contracts;
  using Contracts.PageInterpreter;
  using Core.Utils;
  using DataModel;
  using Enum;
  using Grammar;
  using Components;
  using Interfaces;
  using Antlr4.Runtime.Misc;
  using System;
  using System.Linq;
  using System.Collections.Generic;

  public class PageInterpreter : PageInterpreterBaseVisitor<Symbol>
  {
    public InterpreterResult Result { get; private set; } = new InterpreterResult();
    
    private readonly Page page;
    private string queryLabel;
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
      =>  context.textTypeParameters(index) != null 
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

    private int? calculateSubset(int length, int? percentage) 
      => percentage.HasValue ? (int)(length * (0.01M * percentage)) : (int?)null;

    public override Symbol VisitPattern([NotNull] PageInterpreterParser.PatternContext context)
    {
      var onlyFirstCaptured = context.Any() == null ? true : false;
      var subsets = context?.subset()?.Accept(this)?.GetValue<Dictionary<SubsetPart, int>>();

      int xEnd = calculateSubset(page.Contents.GetLength(0), subsets.ContainsKey(SubsetPart.Left) ? subsets?[SubsetPart.Left] : null) ?? page.Contents.GetLength(0);
      int yEnd = calculateSubset(page.Contents.GetLength(1), subsets.ContainsKey(SubsetPart.Top) ? subsets?[SubsetPart.Top] : null) ?? page.Contents.GetLength(1);
      int xStartDiff = calculateSubset(page.Contents.GetLength(0), subsets.ContainsKey(SubsetPart.Right) ? subsets?[SubsetPart.Right] : null) ?? page.Contents.GetLength(0);
      int yStartDiff = calculateSubset(page.Contents.GetLength(1), subsets.ContainsKey(SubsetPart.Bottom) ? subsets?[SubsetPart.Bottom] : null) ?? page.Contents.GetLength(1);

      for (int x = page.Contents.GetLength(0) - xStartDiff; x < xEnd; x++)
      {
        for (int y = page.Contents.GetLength(1) - yStartDiff; y < yEnd; y++)
        {
          if (page.Contents[x, y] == null || page.Contents[x, y].Count == 0)
            continue;

          var pageIndex = new PageIndex(x, y);
          pageTraverser = new PageTraverser(page, pageIndex);

          try
          {
            base.VisitPattern(context);

            if (onlyFirstCaptured)
            {
              x = page.Contents.GetLength(0);
              break;
            }
          }
          catch (ParseCanceledException) { }
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

      results.Add(Tuple.Create(Direction.Right, rightPageTraverser.Traverse(Direction.Right)));
      results.Add(Tuple.Create(Direction.Down, downPageTraverser.Traverse(Direction.Down)));

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
        Result.AddResult(queryLabel, propertyName, capturedValue);
      }

      return new Symbol(SymbolType.Success);
    }
  }
}
