using System;
using DocumentLab.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DocumentLab.CoreTest
{
  [TestClass]
  public class FuzzyTextComparerTest
  {
    [TestMethod]
    public void Test()
    {
      string scriptLabel = "Cust Num";
      string realLabel = "Customer Number";

      Assert.IsTrue(FuzzyTextComparer.FuzzyEquals(scriptLabel, realLabel));
    }
  }
}
