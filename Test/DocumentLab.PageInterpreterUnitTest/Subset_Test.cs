namespace DocumentLab.PageInterpreterUnitTest
{
  using DocumentLab.Contracts.Utils;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using System.Linq;

  [TestClass]
  public class Subset_Test : InterpreterTestBase
  {
    [TestMethod]
    public void Can_Subset_Right_Half()
    {
      var pageCreator = new TestPageCreator(2, 2);

      pageCreator.SetValue(0, 0, "Text", "Hello");
      pageCreator.SetValue(0, 1, "Number", "5");
      pageCreator.SetValue(1, 0, "Text", "Hello");
      pageCreator.SetValue(1, 1, "Number", "3");

      string query = "Subset(Right 50) Text(Hello) Down [Number];";

      var result = Visit(query, pageCreator.Page);

      Assert.AreEqual("3", result.Results.First().Value.Result.First().Value);
    }

    [TestMethod]
    public void Can_Subset_Left_Half()
    {
      var pageCreator = new TestPageCreator(2, 2);

      pageCreator.SetValue(0, 0, "Text", "Hello");
      pageCreator.SetValue(0, 1, "Number", "5");
      pageCreator.SetValue(1, 0, "Text", "Hello");
      pageCreator.SetValue(1, 1, "Number", "3");

      string query = "Subset(Left 50) Text(Hello) Down [Number];";

      var result = Visit(query, pageCreator.Page);

      Assert.AreEqual("5", result.Results.First().Value.Result.First().Value);
    }

    [TestMethod]
    public void Can_Subset_Top_Half()
    {
      var pageCreator = new TestPageCreator(2, 2);

      pageCreator.SetValue(0, 0, "Text", "Hello");
      pageCreator.SetValue(0, 1, "Number", "5");
      pageCreator.SetValue(1, 0, "Text", "Hello world!");
      pageCreator.SetValue(1, 1, "Number", "3");

      string query = "Subset(Top 50) Text(Hello) Right [Text];";

      var result = Visit(query, pageCreator.Page);

      Assert.AreEqual("Hello world!", result.Results.First().Value.Result.First().Value);
    }

    [TestMethod]
    public void Can_Subset_Bottom_Half()
    {
      var pageCreator = new TestPageCreator(2, 2);

      pageCreator.SetValue(0, 0, "Text", "Hello");
      pageCreator.SetValue(0, 1, "Number", "5");
      pageCreator.SetValue(1, 0, "Text", "Hello world!");
      pageCreator.SetValue(1, 1, "Number", "3");

      string query = "Subset(Bottom 50) Number Right [Number];";

      var result = Visit(query, pageCreator.Page);

      Assert.AreEqual("3", result.Results.First().Value.Result.First().Value);
    }
  }
}
