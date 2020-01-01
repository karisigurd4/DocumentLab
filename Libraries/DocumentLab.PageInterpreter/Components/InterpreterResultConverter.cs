namespace DocumentLab.PageInterpreter
{
  using Antlr4.Runtime;
  using DocumentLab.Contracts.Contracts.PageInterpreter;
  using DocumentLab.PageInterpreter.Components;
  using DocumentLab.PageInterpreter.Grammar;
  using DocumentLab.PageInterpreter.Interpreter;
  using Newtonsoft.Json;
  using System.Collections.Generic;
  using System.Linq;

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
            .FirstOrDefault(x => x.Key == analyzedQuery.Key)
            .Value.Result
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
            .FirstOrDefault()
            .Value
            .Result
            .FirstOrDefault().Value ?? string.Empty
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
