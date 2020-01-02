namespace DocumentLab.Test
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using System.Drawing;

  [TestClass]
  public class FluentQuery_Test
  {
    [TestMethod]
    public void Can_FluentQuery_Handle_SingleCapture_Pattern()
    {
      using (var document = new Document((Bitmap)Image.FromFile("Data\\Example1.png")))
      {
        var result = document
          .Query()
          .Match("Email")
          .Down()
          .Match("WebAddress")
          .Down()
          .Match("Number")
          .Down()
          .CaptureSingle("Text");

        Assert.AreEqual("Example", result);
      }
    }

    [TestMethod]
    public void Can_FluentQuery_Handle_MultiCapture_Pattern()
    {
      using (var document = new Document((Bitmap)Image.FromFile("Data\\Example1.png")))
      {
        var result = document
          .Query()
          .MultiCapture("Email", "Email")
          .Down()
          .MultiCapture("WebAddress", "WebAddress")
          .Down()
          .MultiCapture("Number", "Number")
          .Down()
          .MultiCapture("Text", "Text")
          .ExecuteMultiCapture();

        Assert.IsTrue(result.ContainsKey("Email"));
        Assert.IsTrue(result.ContainsKey("WebAddress"));
        Assert.IsTrue(result.ContainsKey("Number"));
        Assert.IsTrue(result.ContainsKey("Text"));

        Assert.AreEqual("email@email.com", result["Email"]);
        Assert.AreEqual("http://www.website.com/", result["WebAddress"]);
        Assert.AreEqual("50", result["Number"]);
        Assert.AreEqual("Example", result["Text"]);
      }
    }

    [TestMethod]
    public void Can_FluentQuery_Handle_Any_Pattern()
    {
      using (var document = new Document((Bitmap)Image.FromFile("Data\\Example1.png")))
      {
        var result = document
          .Query()
          .GetAny("Date");

        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("2018-01-01", result[0]);
        Assert.AreEqual("2019-12-26", result[1]);
      }
    }

    [TestMethod]
    public void Can_FluentQuery_Handle_GetValueAtLabel()
    {
      using (var document = new Document((Bitmap)Image.FromFile("Data\\Example1.png")))
      {
        var result = document
          .Query()
          .GetValueAtLabel("Label 1", Direction.Right, TextType.Amount);

        Assert.AreEqual("500.50", result);
      }
    }

    [TestMethod]
    public void Can_FluentQuery_Handle_FindValueAtLabel()
    {
      using (var document = new Document((Bitmap)Image.FromFile("Data\\Example1.png")))
      {
        var result = document
          .Query()
          .FindValueForLabel("Label 1", TextType.AmountOrNumber);

        Assert.AreEqual("500.50", result);
      }
    }
  }
}
