namespace DocumentLab.PageInterpreter.Interpreter
{
  using Antlr4.Runtime.Misc;
  using Contracts.PageInterpreter;
  using Exceptions;
  using Grammar;
  using System.Collections.Generic;

  public class ScriptAnalyzer : PageInterpreterBaseVisitor<object>
  {
    public Dictionary<string, AnalyzedQuery> ResultCountByQuery { get; private set; } = new Dictionary<string, AnalyzedQuery>();
    private List<string> visitedTables = new List<string>();

    private string currentQuery;
    private int currentCount;

    public override object VisitQuery([NotNull] PageInterpreterParser.QueryContext context)
    {
      var queryLabel = context?.Text()?.GetText();
      if (string.IsNullOrWhiteSpace(queryLabel))
      {
        throw new SyntaxException("Queries need to be labelled");
      }

      currentQuery = queryLabel;

      base.VisitQuery(context);

      return null;
    }

    public override object VisitTable([NotNull] PageInterpreterParser.TableContext context)
    {
      if (visitedTables.Contains(currentQuery))
      {
        return null;
      }

      ResultCountByQuery.Add(currentQuery, new AnalyzedQuery()
      {
        NumberOfCaptures = context.tableColumn().Length,
        IsArray = false
      });

      visitedTables.Add(currentQuery);

      return null;
    }

    public override object VisitPattern([NotNull] PageInterpreterParser.PatternContext context)
    {
      var onlyFirstCaptured = context.Any() == null ? true : false;

      currentCount = 0;

      base.VisitPattern(context);

      if (!ResultCountByQuery.ContainsKey(currentQuery))
      {
        ResultCountByQuery.Add(currentQuery, new AnalyzedQuery()
        {
          IsArray = !onlyFirstCaptured,
          NumberOfCaptures = currentCount
        });
      }
      else
      {
        if (!onlyFirstCaptured && !ResultCountByQuery[currentQuery].IsArray)
        {
          ResultCountByQuery[currentQuery].IsArray = true;
        }

        if (currentCount > ResultCountByQuery[currentQuery].NumberOfCaptures)
        {
          ResultCountByQuery[currentQuery].NumberOfCaptures = currentCount;
        }
      }

      return null;
    }

    public override object VisitCapture([NotNull] PageInterpreterParser.CaptureContext context)
    {
      currentCount += 1;

      return base.VisitCapture(context);
    }
  }
}
