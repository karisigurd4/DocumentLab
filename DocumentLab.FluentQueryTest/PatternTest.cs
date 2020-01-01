namespace DocumentLab.FluentQueryTest
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class PatternTest
  {
    [TestMethod]
    public void Can_Generate_Pattern_With_All_Tokens_And_Text_Matching()
    {
      var expected = " Text(MatchThis) Down Date Right Town Up Text Left [Number]";

      var query = new FluentQuery()
        .Match("Text", "MatchThis")
        .Down()
        .Match("Date")
        .Right()
        .Match("Town")
        .Up()
        .Match("Text")
        .Left()
        .Capture("Number");

      Assert.AreEqual(expected, query.Script);
    }
  }
}
