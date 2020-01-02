namespace DocumentLab.FluentQueryTest
{
    using DocumentLab.PageInterpreter.Components;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class PatternTest
  {
    [TestMethod]
    public void Can_Generate_Pattern_With_All_Tokens_And_Text_Matching()
    {
      var expected = " Text(MatchThis) Down Date Right Town Up Text Left [Number]";

      var query = new FluentQuery(new Contracts.Page(), new Interpreter())
      { 
        Script = ""
      }
        .Match("Text", "MatchThis")
        .Down()
        .Match("Date")
        .Right()
        .Match("Town")
        .Up()
        .Match("Text")
        .Left()
        .MultiCapture("Number");

      Assert.AreEqual(expected, query.Script);
    }
  }
}
