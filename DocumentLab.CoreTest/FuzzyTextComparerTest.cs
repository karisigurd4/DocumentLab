namespace DocumentLab.CoreTest
{
  using DocumentLab.Core.Utils;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
 
  [TestClass]
  public class FuzzyTextComparerTest
  {
    [TestMethod]
    public void Test_1()
    {
      string scriptLabel = "Customer Num";
      string realLabel = "Customer Number";

      Assert.IsTrue(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Test_2()
    {
      string scriptLabel = "Customer Num";
      string realLabel = "ustomer Number";

      Assert.IsTrue(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Test_3()
    {
      string scriptLabel = "Banana";
      string realLabel = "Apple";

      Assert.IsFalse(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }
  }
}
