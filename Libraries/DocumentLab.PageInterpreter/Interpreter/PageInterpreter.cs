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
        && (matchContent?.Any(x => FuzzyMatch(x, pageUnit.Value)) 
        ?? true);

    private string[] GetTextTypeParameters(PageInterpreterParser.TextTypeContext context, int index) 
      =>  context.textTypeParameters(index) != null 
        ? context?.textTypeParameters(index).Accept(this)?.GetValue<string[]>() ?? null
        : null;

    private bool FuzzyMatch(string first, string second)
    {
      first = first.Replace(" ", "");
      second = second.Replace(" ", "");
      return second.IndexOf(first, StringComparison.InvariantCultureIgnoreCase) >= 0 || LevenshteinDistance.Compute(first, second) < 2;
    }

    public override Symbol VisitQuery([NotNull] PageInterpreterParser.QueryContext context)
    {
      queryLabel = context?.Text()?.GetText() ?? Result.Results.Count.ToString();

      base.VisitQuery(context);

      return new Symbol(SymbolType.Success);
    }

    public override Symbol VisitPattern([NotNull] PageInterpreterParser.PatternContext context)
    {
      var onlyFirstCaptured = context.Any() == null ? true : false;

      for (int x = 0; x < page.Contents.GetLength(0); x++)
      {
        for (int y = 0; y < page.Contents.GetLength(1); y++)
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
      var maxSteps = (int)int.Parse(context.Steps.Text);
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
