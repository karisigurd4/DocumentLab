namespace DocumentLab.PageInterpreterUnitTest
{
  using DocumentLab.Contracts.Utils;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using System.Linq;

  [TestClass]
  public class Query_Tests : InterpreterTestBase
  {
    [TestMethod]
    public void Does_Ignore_Comments()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(4, 4, "Text", "Hello");
      pageCreator.SetValue(4, 5, "Amount", "234.3");
      pageCreator.SetValue(4, 6, "Percentage", "5%");
      pageCreator.SetValue(4, 7, "Text", "Correct");

      string query =
@"
/*
Block comment at top
*/

// This is a comment 
TestLabel: // Comment on a label
/* 
Block comment below label
*/
[StreetAddress]; // Comment on a text type
[Text]; 
// Comment above text type
[Email];
//Comment at bottom

/* 
Block comment at eof
*/
";

      var result = Visit(query, pageCreator.Page);

      Assert.AreEqual("TestLabel", result.Results.First().Key);
    }

    [TestMethod]
    public void Can_Assign_Query_Label()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(4, 4, "Text", "Hello");
      pageCreator.SetValue(4, 5, "Amount", "234.3");
      pageCreator.SetValue(4, 6, "Percentage", "5%");
      pageCreator.SetValue(4, 7, "Text", "Correct");

      string query = "TestLabel: [Text];";

      var result = Visit(query, pageCreator.Page);

      Assert.AreEqual("TestLabel", result.Results.First().Key);
    }

    [TestMethod]
    public void Does_Prioritize_Patterns()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(4, 4, "Text", "Hello");
      pageCreator.SetValue(4, 5, "Amount", "234.3");
      pageCreator.SetValue(4, 6, "Percentage", "5%");
      pageCreator.SetValue(4, 7, "Text", "Correct");

      string query =
@"
TestLabel: 
Text(Wrong) Up Left [Text];
Text Down Amount Down [Percentage] Down [Text];
Percentage Down [Text];
";

      var result = Visit(query, pageCreator.Page);

      Assert.AreEqual("5%", result.Results["TestLabel"].GetResultAt(0));
      Assert.AreEqual("Correct", result.Results["TestLabel"].GetResultAt(1));
    }

    [TestMethod]
    public void Can_Assign_Query_Multiple_Result_Labels()
    {
      var pageCreator = new TestPageCreator(20, 20);

      pageCreator.SetValue(4, 4, "Text", "First");
      pageCreator.SetValue(8, 4, "Text", "Value1");
      pageCreator.SetValue(4, 5, "Text", "Second");
      pageCreator.SetValue(8, 5, "Text", "Value2");

      string query = @"
Label1: 
Text(First) Right [Text];

Label2:
Text(Second) Right [Text];
";

      var result = Visit(query, pageCreator.Page);

      Assert.AreEqual("Value1", result.Results["Label1"].GetResultAt(0));
      Assert.AreEqual("Value2", result.Results["Label2"].GetResultAt(0));
    }
  }
}
