namespace DocumentLab.PageInterpreter
{
  using Contracts.PageInterpreter;
  using Grammar;
  using Components;
  using Interpreter;
  using Antlr4.Runtime;
  using Newtonsoft.Json;
  using System.Linq;
  using System.Collections.Generic;

  public static class InterpreterResultConverter
  {
    public static string ConvertToJson(this InterpreterResult interpreterResult, string script)
    {
      var responseObject = new Dictionary<string, object>();

      foreach (var analyzedQuery in AnalyzeScript(script))
      {
        if (analyzedQuery.Value.NumberOfCaptures > 1 && analyzedQuery.Value.IsArray)
        {
          responseObject.Add(analyzedQuery.Key,
            interpreterResult.Results
            .Where(x => x.Key == analyzedQuery.Key)
            .Select(x => x.Value.Result)
          );
        }
        else if (analyzedQuery.Value.NumberOfCaptures > 1 && !analyzedQuery.Value.IsArray)
        {
          responseObject.Add(analyzedQuery.Key,
            interpreterResult.Results
            ?.FirstOrDefault(x => x.Key == analyzedQuery.Key)
            .Value?.Result ?? new Dictionary<string, string>() { }
          );
        }
        else if (analyzedQuery.Value.IsArray)
        {
          responseObject.Add(analyzedQuery.Key,
            interpreterResult.Results
            .Where(x => x.Key == analyzedQuery.Key)
            .SelectMany(x => x.Value.Result.Select(y => y.Value)) ?? new string[] { }
          );
        }
        else
        {
          responseObject.Add(analyzedQuery.Key,
            interpreterResult.Results
            .Where(x => x.Key == analyzedQuery.Key)
            ?.FirstOrDefault()
            .Value
            ?.Result
            ?.FirstOrDefault().Value ?? string.Empty
          );
        }
      }

      return JsonConvert.SerializeObject(responseObject, Formatting.Indented);
    }

    private static Dictionary<string, AnalyzedQuery> AnalyzeScript(string script)
    {
      var patternInterpreterLexer = new PageInterpreterLexer(new AntlrInputStream(script));
      var patternInterpreterParser = new PageInterpreterParser(new CommonTokenStream(patternInterpreterLexer));
      patternInterpreterParser.AddErrorListener(new ErrorListener());

      var visitor = new ScriptAnalyzer();

      visitor.Visit(patternInterpreterParser.compileUnit());
      return visitor.ResultCountByQuery;
    }
  }
}
