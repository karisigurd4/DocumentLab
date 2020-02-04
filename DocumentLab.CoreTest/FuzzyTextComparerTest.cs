namespace DocumentLab.CoreTest
{
  using DocumentLab.Core.Utils;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
 
  [TestClass]
  public class FuzzyTextComparerTest
  {
    [TestMethod]
    public void Can_Match_1()
    {
      string scriptLabel = "Invoice";
      string realLabel = "Invoice";

      Assert.IsTrue(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Can_Match_2()
    {
      string scriptLabel = "Address";
      string realLabel = "Address";

      Assert.IsTrue(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Can_Match_3()
    {
      string scriptLabel = "Banana Factory";
      string realLabel = "Banana Factory";

      Assert.IsTrue(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Rejects_1()
    {
      string scriptLabel = "Invoice";
      string realLabel = "Address";

      Assert.IsFalse(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Rejects_2()
    {
      string scriptLabel = "To";
      string realLabel = "From";

      Assert.IsFalse(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Rejects_3()
    {
      string scriptLabel = "Banana Factory";
      string realLabel = "Apple Factory";

      Assert.IsFalse(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Can_Match_Partial_Script()
    {
      string scriptLabel = "ustomer Number";
      string realLabel = "Customer Number";

      Assert.IsTrue(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Can_Match_Partial_Script_2()
    {
      string scriptLabel = "stomer Number";
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
    public void Can_Match_Partial_Label_2()
    {
      string scriptLabel = "Customer Number";
      string realLabel = "stomer Number";

      Assert.IsTrue(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Rejects_Short_Script_Short_Label()
    {
      string scriptLabel = "C";
      string realLabel = "D";

      Assert.IsFalse(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Rejects_Short_Script()
    {
      string scriptLabel = "C";
      string realLabel = "DamnLongLabel";

      Assert.IsFalse(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Rejects_Short_Label()
    {
      string scriptLabel = "CoolLongScript";
      string realLabel = "D";

      Assert.IsFalse(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Matches_Short()
    {
      string scriptLabel = "AB";
      string realLabel = "AB";

      Assert.IsTrue(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }

    [TestMethod]
    public void Rejects_Short()
    {
      string scriptLabel = "BA";
      string realLabel = "AB";

      Assert.IsFalse(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
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
