namespace DocumentLab.TextAnalyzerUnitTest
{
  using System.Linq;
  using Contracts.Ocr;
  using Castle.Windsor;
  using Castle.Windsor.Installer;
  using DocumentLab.TextAnalyzer.Interfaces;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class TextAnalyzer_Various
  {
    private WindsorContainer container = new WindsorContainer();

    [TestInitialize]
    public void Initialize()
    {
      container.Install(FromAssembly.Named("DocumentLab.TextAnalyzer"));
    }

    [TestMethod]
    public void Can_Fix_Bad_OCR_Number()
    {
      var analyzer = container.Resolve<ITextAnalyzer>();

      var fakeString = "l50, OO";

      var result = analyzer.AnalyzeOcrResult(new OcrResult() { Result = new string[] { fakeString } });

      Assert.IsNotNull(result);
      Assert.IsTrue(result.Any(x => x.TextType == "Text"));
      Assert.IsTrue(result.Any(x => x.TextType == "Number" && x.Text == "150"));
      Assert.IsTrue(result.Any(x => x.TextType == "AmountOrNumber"));
      Assert.IsTrue(result.Any(x => x.TextType == "Amount" && x.Text == "150.00"));
    }

    [TestMethod]
    public void Can_Analyze_Text()
    {
      var analyzer = container.Resolve<ITextAnalyzer>();

      var fakeString = "dfgdfgldfg retertertert";

      var result = analyzer.AnalyzeOcrResult(new OcrResult() { Result = new string[] { fakeString } });

      Assert.IsNotNull(result);
      Assert.IsTrue(result.Any(x => x.TextType == "Text" && x.Text == "dfgdfgldfg retertertert"));

      fakeString = "2314897l asdasdlqwe qetqetqet, 234234234-234234234";

      result = analyzer.AnalyzeOcrResult(new OcrResult() { Result = new string[] { fakeString } });

      Assert.IsNotNull(result);
      Assert.IsTrue(result.Any(x => x.TextType == "Text" && x.Text == "2314897l asdasdlqwe qetqetqet, 234234234-234234234"));
    }

    [TestMethod]
    public void Can_Analyze_CustomType()
    {
      var analyzer = container.Resolve<ITextAnalyzer>();
      string fakeString = "F 9 00 91";
      var result = analyzer.AnalyzeOcrResult(new OcrResult() { Result = new string[] { fakeString } });
      Assert.IsTrue(result.Any(x => x.TextType == "InvoiceNumber"));
    }

    [TestMethod]
    public void Can_Analyze_Amount()
    {
      var analyzer = container.Resolve<ITextAnalyzer>();

      var fakeString = "1.402";

      var result = analyzer.AnalyzeOcrResult(new OcrResult() { Result = new string[] { fakeString } });

      Assert.IsNotNull(result);
      Assert.IsTrue(result.Any(x => x.TextType == "Amount" && x.Text == "1.402"));

      fakeString = "1,402";

      result = analyzer.AnalyzeOcrResult(new OcrResult() { Result = new string[] { fakeString } });

      Assert.IsNotNull(result);
      Assert.IsTrue(result.Any(x => x.TextType == "Amount" && x.Text == "1.402"));

      fakeString = "1. 402";

      result = analyzer.AnalyzeOcrResult(new OcrResult() { Result = new string[] { fakeString } });

      Assert.IsNotNull(result);
      Assert.IsTrue(result.Any(x => x.TextType == "Amount" && x.Text == "1.402"));
    }

    [TestMethod]
    public void Can_Analyze_Letters()
    {
      var analyzer = container.Resolve<ITextAnalyzer>();

      var fakeString = "1 dfg";

      var result = analyzer.AnalyzeOcrResult(new OcrResult() { Result = new string[] { fakeString } });

      Assert.IsNotNull(result);
      Assert.IsTrue(result.Any(x => x.TextType == "Letters" && x.Text == "dfg"));

      fakeString = "1";

      result = analyzer.AnalyzeOcrResult(new OcrResult() { Result = new string[] { fakeString } });

      Assert.IsTrue(!result.Any(x => x.TextType == "Letters"));
    }

    [TestMethod]
    public void Can_Analyze_StreetAddress()
    {
      var analyzer = container.Resolve<ITextAnalyzer>();

      var fakeString = "Box 550";

      var result = analyzer.AnalyzeOcrResult(new OcrResult() { Result = new string[] { fakeString } });

      Assert.IsNotNull(result);
      Assert.IsTrue(result.Any(x => x.TextType == "Text" && x.Text == "Box 550"));
      Assert.IsTrue(result.Any(x => x.TextType == "Number" && x.Text == "550"));
      Assert.IsTrue(result.Any(x => x.TextType == "AmountOrNumber" && x.Text == "550"));
      Assert.IsTrue(result.Any(x => x.TextType == "StreetAddress" && x.Text == "Box 550"));

      fakeString = "Box";

      result = analyzer.AnalyzeOcrResult(new OcrResult() { Result = new string[] { fakeString } });

      Assert.IsNotNull(result);
      Assert.IsTrue(result.Any(x => x.TextType == "Text" && x.Text == "Box"));
      Assert.IsTrue(result.Any(x => x.TextType == "StreetAddress" && x.Text == "Box"));

      fakeString = "Box l";

      result = analyzer.AnalyzeOcrResult(new OcrResult() { Result = new string[] { fakeString } });

      Assert.IsNotNull(result);
      Assert.IsTrue(result.Any(x => x.TextType == "Text" && x.Text == "Box l"));
      Assert.IsTrue(result.Any(x => x.TextType == "StreetAddress" && x.Text == "Box"));
    }
  }
}
