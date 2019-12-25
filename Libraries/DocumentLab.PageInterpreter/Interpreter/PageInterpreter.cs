namespace DocumentLab.PageInterpreter.Interpreter
{
  using Antlr4.Runtime.Misc;
  using Antlr4.Runtime.Tree;
  using DocumentLab.Contracts;
  using DocumentLab.Contracts.Contracts.PageInterpreter;
  using DocumentLab.Contracts.Enums.Types;
  using DocumentLab.Core.Utils;
  using DocumentLab.PageInterpreter.DataModel;
  using DocumentLab.PageInterpreter.Enum;
  using DocumentLab.PageInterpreter.Grammar;
  using DocumentLab.PageInterpreter.Components;
  using DocumentLab.PageInterpreter.Interfaces;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class PageInterpreter : PageInterpreterBaseVisitor<Symbol>
  {
    public InterpreterResult Result { get; private set; } = new InterpreterResult();

    private string queryLabel;
    private IPageTraverser pageTraverser;
    private readonly Page page;

    private PageUnit lastPageUnit = null;

    public PageInterpreter(Page page)
    {
      this.page = page;
    }

    private bool ValidateTextTypeMatch(PageUnit pageUnit, string textType, string[] matchContent) => pageUnit != null && (matchContent?.Any(x => FuzzyMatch(x, pageUnit.Value)) ?? true);

    private string[] GetTextTypeParameters(PageInterpreterParser.TextTypeContext context) => context.textTypeParameters() != null ? context.textTypeParameters().Accept(this)?.GetValue<string[]>() : null;

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
      var textType = context.Text().GetText();
      var pageUnit = pageTraverser.GetMatchingPageUnit(textType);

      if (pageUnit != null)
      {
        lastPageUnit = pageUnit;
      }

      string[] matchContent = GetTextTypeParameters(context);

      if (!ValidateTextTypeMatch(pageUnit, textType, matchContent))
      {
        lastPageUnit = null;
        throw new ParseCanceledException();
      }

      return new Symbol(SymbolType.Success);
    }

    public override Symbol VisitTextTypeParameters([NotNull] PageInterpreterParser.TextTypeParametersContext context)
    {
      return new Symbol(SymbolType.Array, context.Parameters().GetText().Substring(1, context.Parameters().GetText().Length - 2).Replace("||", "~").Split('~'));
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
      var matchResult = context.textType().Accept(this);
      var propertyName = context.propertyName()?.Accept(this).GetValue<string>();
      var capturedValue = pageTraverser.GetMatchingPageUnit(context.Match.GetText())?.Value;

      if(!string.IsNullOrWhiteSpace(capturedValue))
      {
        Result.AddResult(queryLabel, propertyName, capturedValue);
      }

      return new Symbol(SymbolType.Success);
    }

    private bool FuzzyMatch(string first, string second)
    {
      first = first.Replace(" ", "");
      second = second.Replace(" ", "");

      return second.IndexOf(first, StringComparison.InvariantCultureIgnoreCase) >= 0 || LevenshteinDistance.Compute(first, second) < 2;
    }
  }
}
