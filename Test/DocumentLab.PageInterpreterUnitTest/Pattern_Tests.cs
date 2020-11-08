namespace DocumentLab.PageInterpreterUnitTest
{
  using DocumentLab.Contracts.Utils;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using System.Linq;

  [TestClass]
  public class Pattern_Tests : InterpreterTestBase
  {
    [TestMethod]
    public void Can_Match_Multiple_Captures()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(4, 4, "Text", "Hello");
      pageCreator.SetValue(4, 5, "Amount", "234.3");
      pageCreator.SetValue(4, 6, "Percentage", "5%");
      pageCreator.SetValue(4, 7, "Text", "Correct");

      string query = "Text Down [Amount] Down Percentage Down [Text];";

      var result = Visit(query, pageCreator.Page);

      Assert.IsTrue(result.Results.Count == 1);
      Assert.AreEqual("0", result.Results["0"].Result.First().Key);
      Assert.AreEqual("234.3", result.Results["0"].GetResultAt(0));
      Assert.AreEqual("Correct", result.Results["0"].GetResultAt(1));
    }

    [TestMethod]
    public void Using_Any_Takes_All_Matching_Patterns()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(4, 4, "Text", "Hello");
      pageCreator.SetValue(4, 5, "Amount", "234.3");
      pageCreator.SetValue(4, 6, "Percentage", "5%");
      pageCreator.SetValue(4, 7, "Text", "Correct");

      pageCreator.SetValue(6, 4, "Text", "Hello");
      pageCreator.SetValue(6, 5, "Amount", "234.3");
      pageCreator.SetValue(6, 6, "Percentage", "5%");
      pageCreator.SetValue(6, 7, "Text", "Also correct");

      string query = "Any Text Down Amount Down Percentage Down [Text];";

      var result = Visit(query, pageCreator.Page);

      Assert.IsTrue(result.Results["0"].Result.Count == 2);
      Assert.AreEqual("0", result.Results["0"].Result.ToArray()[0].Key);
      Assert.AreEqual("Correct", result.Results["0"].GetResultAt(0));
      Assert.AreEqual("00", result.Results["0"].Result.ToArray()[1].Key);
      Assert.AreEqual("Also correct", result.Results["0"].Result["00"]);
    }

    [TestMethod]
    public void Takes_First_Matching_Pattern()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(4, 4, "Text", "Hello");
      pageCreator.SetValue(4, 5, "Amount", "234.3");
      pageCreator.SetValue(4, 6, "Percentage", "5%");
      pageCreator.SetValue(4, 7, "Text", "Correct");

      pageCreator.SetValue(6, 4, "Text", "Hello");
      pageCreator.SetValue(6, 5, "Amount", "234.3");
      pageCreator.SetValue(6, 6, "Percentage", "5%");
      pageCreator.SetValue(6, 7, "Text", "Incorrect");

      string query = "Text Down Amount Down Percentage Down [Text];";

      var result = Visit(query, pageCreator.Page);

      Assert.IsTrue(result.Results.Count == 1);
      Assert.IsTrue(result.Results["0"].Result.Count == 1);
      Assert.AreEqual("0", result.Results["0"].Result.ToArray()[0].Key);
      Assert.AreEqual("Correct", result.Results["0"].GetResultAt(0));
    }

    [TestMethod]
    public void Can_Interpret_Continuous_Page_Pattern()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(4, 4, "Text", "Hello");
      pageCreator.SetValue(4, 5, "Amount", "234.3");
      pageCreator.SetValue(4, 6, "Percentage", "5%");
      pageCreator.SetValue(4, 7, "Date", "2019-06-05");

      string query = "Text Down Amount Down Percentage Down 'CaptureThis': [Date];";

      var result = Visit(query, pageCreator.Page);

      Assert.IsTrue(result.Results.Count == 1);
      Assert.IsTrue(result.Results["0"].Result.Count == 1);
      Assert.AreEqual("CaptureThis", result.Results["0"].Result.ToArray()[0].Key);
      Assert.AreEqual("2019-06-05", result.Results["0"].GetResultAt(0));
    }

    [TestMethod]
    public void Can_Match_Content()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(4, 4, "Text", "Hello");
      pageCreator.SetValue(4, 5, "Text", "Test string");
      pageCreator.SetValue(4, 6, "Percentage", "5%");
      pageCreator.SetValue(4, 7, "Date", "2019-06-05");

      string query = "Text(Hello) Down Text(Test string) Down Percentage Down 'CaptureThis': [Date];";

      var result = Visit(query, pageCreator.Page);

      Assert.IsTrue(result.Results.Count == 1);
      Assert.IsTrue(result.Results["0"].Result.Count == 1);
      Assert.AreEqual("CaptureThis", result.Results["0"].Result.ToArray()[0].Key);
      Assert.AreEqual("2019-06-05", result.Results["0"].GetResultAt(0));
    }

    [TestMethod]
    public void Can_Match_TextType_With_Or()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(4, 4, "Text", "Hello");
      pageCreator.SetValue(4, 7, "Date", "2019-06-05");
      pageCreator.SetValue(4, 9, "Number", "5");

      string firstQuery = "Date || Text(Hello) Down [Number || Date];";

      var firstResult = Visit(firstQuery, pageCreator.Page);

      Assert.IsTrue(firstResult.Results.Count == 1);
      Assert.IsTrue(firstResult.Results["0"].Result.Count == 1);
      Assert.AreEqual("2019-06-05", firstResult.Results["0"].GetResultAt(0));

      string secondQuery = "Date || Text(John) || Number Down [Number || Date];";

      var secondResult = Visit(secondQuery, pageCreator.Page);

      Assert.IsTrue(secondResult.Results.Count == 1);
      Assert.IsTrue(secondResult.Results["0"].Result.Count == 1);
      Assert.AreEqual("5", secondResult.Results["0"].GetResultAt(0));
    }
    [TestMethod]
    public void Stops_When_Cant_Match_Content()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(4, 4, "Text", "Hello");
      pageCreator.SetValue(4, 5, "Text", "Bad match");
      pageCreator.SetValue(4, 6, "Percentage", "5%");
      pageCreator.SetValue(4, 7, "Date", "2019-06-05");

      string query = "Text Down Text(Test string) Down Percentage Down 'CaptureThis': [Date];";

      var result = Visit(query, pageCreator.Page);

      Assert.IsTrue(result.Results.Count == 0);
    }

    [TestMethod]
    public void Stops_When_Pattern_Missing_Page_Units()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(4, 4, "Text", "Hello");
      pageCreator.SetValue(4, 7, "Date", "2019-06-05");

      string query = "Text Down Amount Down Percentage Down 'CaptureThis': [Date];";

      var result = Visit(query, pageCreator.Page);

      Assert.IsTrue(result.Results.Count == 0);
    }

    [TestMethod]
    public void Stops_When_Pattern_Does_Not_Match()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(4, 4, "Text", "Hello");
      pageCreator.SetValue(4, 5, "Text", "Hello");
      pageCreator.SetValue(4, 6, "Text", "Hello");
      pageCreator.SetValue(4, 7, "Date", "2019-06-05");

      string query = "Text Down Amount Down Percentage Down 'CaptureThis': [Date];";

      var result = Visit(query, pageCreator.Page);

      Assert.IsTrue(result.Results.Count == 0);
    }

    [TestMethod]
    public void Can_Interpret_Gap_Page_Pattern()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(4, 4, "Text", "Hello");
      pageCreator.SetValue(4, 6, "Amount", "234.3");
      pageCreator.SetValue(4, 7, "Percentage", "5%");
      pageCreator.SetValue(4, 9, "Date", "2019-06-05");

      string query = "Text Down Amount Down Percentage Down 'CaptureThis': [Date];";

      var result = Visit(query, pageCreator.Page);

      Assert.IsTrue(result.Results.Count == 1);
      Assert.IsTrue(result.Results["0"].Result.Count == 1);
      Assert.AreEqual("CaptureThis", result.Results["0"].Result.ToArray()[0].Key);
      Assert.AreEqual("2019-06-05", result.Results["0"].GetResultAt(0));
    }

    [TestMethod]
    public void Fails_With_ParseCancelledException_On_Mismatch()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(4, 4, "Text", "Hello");
      pageCreator.SetValue(4, 6, "Amount", "234.3");
      pageCreator.SetValue(4, 7, "Percentage", "5%");

      string query = "Text Down Amount Down Percentage Down 'CaptureThis': [Date];";

      var result = Visit(query, pageCreator.Page);

      Assert.IsTrue(result.Results.Count == 0);
    }
  }
}
