using System;
using System.Linq;
using DocumentLab.Contracts;
using DocumentLab.Contracts.Utils;
using DocumentLab.PageAnalyzer.Implementation;
using DocumentLab.PageAnalyzer.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DocumentLab.PageAnalyzerUnitTest
{
  [TestClass]
  public class PageTrimmer_TrimPage
  {
    private readonly IPageTrimmer pageTrimmer;

    public PageTrimmer_TrimPage()
    {
      pageTrimmer = new PageTrimmer();
    }

    [TestMethod]
    public void Can_Trim_To_Straight_Column()
    {
      TestPageCreator testPageCreator = new TestPageCreator(2, 3);
      testPageCreator.SetValue(0, 0, "Text", "Text 1", new PageUnitBoundingBox(0, 0, 1, 1));
      testPageCreator.SetValue(1, 1, "Text", "Text 2", new PageUnitBoundingBox(5, 20, 1, 1));
      testPageCreator.SetValue(0, 2, "Text", "Text 2", new PageUnitBoundingBox(0, 40, 1, 1));

      var test = pageTrimmer.TrimPage(testPageCreator.Page);

      Assert.IsNotNull(test.Contents[0, 1].First());
      Assert.AreEqual(test.Contents[0, 0].First().Coordinate.X, test.Contents[0, 1].First().Coordinate.X);
    }

    [TestMethod]
    public void Can_Trim_To_Straight_Row()
    {
      TestPageCreator testPageCreator = new TestPageCreator(3, 2);
      testPageCreator.SetValue(0, 0, "Text", "Text 1", new PageUnitBoundingBox(0, 0, 1, 1));
      testPageCreator.SetValue(1, 1, "Text", "Text 2", new PageUnitBoundingBox(20, 5, 1, 1));
      testPageCreator.SetValue(2, 0, "Text", "Text 2", new PageUnitBoundingBox(40, 0, 1, 1));

      var test = pageTrimmer.TrimPage(testPageCreator.Page);

      Assert.IsNotNull(test.Contents[1, 0].First());
      Assert.AreEqual(test.Contents[0, 0].First().Coordinate.Y, test.Contents[1, 0].First().Coordinate.Y);
    }
  }
}
