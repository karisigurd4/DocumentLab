namespace DocumentLab.PageAnalyzerUnitTest
{
  using DocumentLab.Contracts;
  using DocumentLab.Contracts.PageInterpreter;
  using DocumentLab.Contracts.Utils;
  using DocumentLab.PageAnalyzer.Implementation;
  using DocumentLab.PageAnalyzer.Interfaces;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using System;
  using System.Linq;

  [TestClass]
  public class PageTrimmer_MovePageUnit
  {
    private readonly IPageTrimmer pageTrimmer;

    public PageTrimmer_MovePageUnit()
    {
      pageTrimmer = new PageTrimmer();
    }

    [TestMethod]
    public void Can_Move_PageUnit()
    {
      TestPageCreator testPageCreator = new TestPageCreator(2, 3);
      testPageCreator.SetValue(0, 0, "Text", "Text 1", new PageUnitBoundingBox(0, 0, 1, 1));
      testPageCreator.SetValue(1, 1, "Text", "Text 2", new PageUnitBoundingBox(5, 5, 1, 1));
      testPageCreator.SetValue(0, 2, "Text", "Text 2", new PageUnitBoundingBox(0, 10, 1, 1));

      var test = pageTrimmer.MovePageUnit(testPageCreator.Page, new Coordinate(1, 1), new Coordinate(0, 1));

      Assert.IsNotNull(test);
      Assert.IsNotNull(testPageCreator.Page.Contents[0, 1].FirstOrDefault());
      Assert.IsNull(testPageCreator.Page.Contents?[1, 1]?.FirstOrDefault());
    }
  }
}
