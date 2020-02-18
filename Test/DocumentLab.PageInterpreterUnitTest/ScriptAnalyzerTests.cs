namespace DocumentLab.PageInterpreterUnitTest
{
  using Antlr4.Runtime;
  using Contracts.PageInterpreter;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using PageInterpreter.Components;
  using PageInterpreter.Grammar;
  using PageInterpreter.Interpreter;
  using System.Collections.Generic;

  [TestClass]
  public class ScriptAnalyzerTests
  {
    public Dictionary<string, AnalyzedQuery> Visit(string script)
    {
      var patternInterpreterLexer = new PageInterpreterLexer(new AntlrInputStream(script));
      var patternInterpreterParser = new PageInterpreterParser(new CommonTokenStream(patternInterpreterLexer));
      patternInterpreterParser.AddErrorListener(new ErrorListener());

      var visitor = new ScriptAnalyzer();

      visitor.Visit(patternInterpreterParser.compileUnit());
      return visitor.ResultCountByQuery;
    }

    [TestMethod]
    public void Can_Analyze_Simple_Query()
    {
      string script = @"
Dates: Any [Date];
Receivers: 
'A': [Text];
Any 'A': [Text] Down 'B': [Text] Down 'C': [Text];
'B': [Text];
TotalAmount: Text(Hello) Right [Amount];
";

      var result = Visit(script);

      Assert.AreEqual(1, result["Dates"].NumberOfCaptures);
      Assert.AreEqual(true, result["Dates"].IsArray);
      Assert.AreEqual(3, result["Receivers"].NumberOfCaptures);
      Assert.AreEqual(true, result["Receivers"].IsArray);
      Assert.AreEqual(1, result["TotalAmount"].NumberOfCaptures);
      Assert.AreEqual(false, result["TotalAmount"].IsArray);
    }

    [TestMethod]
    public void Can_Analyze_Table_Query()
    {
      string script = @"
 MyTable: Table 'First': [Text] 'Second': [Amount] 'Third': [Number];
";

      var result = Visit(script);

      Assert.AreEqual(3, result["MyTable"].NumberOfCaptures);
      Assert.AreEqual(false, result["MyTable"].IsArray);
    }
  }
}
