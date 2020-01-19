namespace DocumentLab.CoreTest
{
  using DocumentLab.Core.Utils;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
 
  [TestClass]
  public class FuzzyTextComparerTest
  {
    [TestMethod]
    public void Can_Match_Partial_Script()
    {
      string scriptLabel = "Customer Num";
      string realLabel = "Customer Number";

      Assert.IsTrue(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Can_Match_Partial_Label()
    {
      string scriptLabel = "Customer Num";
      string realLabel = "ustomer Number";

      Assert.IsTrue(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Rejects_Different()
    {
      string scriptLabel = "Banana";
      string realLabel = "Apple";

      Assert.IsFalse(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }
  }
}
