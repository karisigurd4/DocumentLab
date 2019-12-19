namespace DocumentLab.PageInterpreterUnitTest
{
  using DocumentLab.Contracts.Utils;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class RightDownSearch_Test : InterpreterTestBase
  {
    [TestMethod]
    public void Can_Do_RightDown_Down_Search()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(0, 0, "Text", "Find");
      pageCreator.SetValue(0, 5, "Text", "Hello");
      pageCreator.SetValue(6, 0, "Text", "World");

      string query = "Text(Find) RD 6 [Text];";

      var result = Visit(query, pageCreator.Page);

      Assert.AreEqual("Hello", result.Results["0"].Result["0"]);
    }

    [TestMethod]
    public void Can_Do_RightDown_Right_Search()
    {
      var pageCreator = new TestPageCreator(10, 10);

      pageCreator.SetValue(0, 0, "Text", "Find");
      pageCreator.SetValue(0, 5, "Text", "Hello");
      pageCreator.SetValue(3, 0, "Text", "World");

      string query = "Text(Find) RD 6 [Text];";

      var result = Visit(query, pageCreator.Page);

      Assert.AreEqual("World", result.Results["0"].Result["0"]);
    }

    [TestMethod]
    public void Can_Do_RightDown_Search_Middle_Of_Page()
    {
      var pageCreator = new TestPageCreator(30, 30);

      pageCreator.SetValue(15, 15, "Text", "Find");
      pageCreator.SetValue(18, 15, "Text", "Hello");
      pageCreator.SetValue(15, 19, "Text", "World");

      string query = "Text(Find) RD 6 [Text];";

      var result = Visit(query, pageCreator.Page);

      Assert.AreEqual("Hello", result.Results["0"].Result["0"]);
    }

    [TestMethod]
    public void Can_Do_RightDown_Search_And_Continue_From_It()
    {
      var pageCreator = new TestPageCreator(30, 30);

      pageCreator.SetValue(15, 15, "Text", "Find");
      pageCreator.SetValue(18, 15, "Text", "Hello");
      pageCreator.SetValue(15, 19, "Text", "World");
      pageCreator.SetValue(18, 21, "Amount", "3.0");

      string query = "Text(Find) RD 6 Text Down 'CoolStuff': [Amount];";

      var result = Visit(query, pageCreator.Page);

      Assert.AreEqual("3.0", result.Results["0"].Result["CoolStuff"]);
    }
  }
}
