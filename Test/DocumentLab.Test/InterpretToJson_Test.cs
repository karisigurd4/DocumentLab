namespace DocumentLab.Test
{
  using DocumenLab;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using PdfiumViewer;
  using System.Drawing;

  [TestClass]
  public class InterpretToJson_Test
  {
    [TestMethod]
    public void Can_Interpret_FromFile()
    {
      string script =
@"
Sender:
Text(From:) Down 'SenderName': [Text] Down 'SenderApartment': [Text] Down 'SenderStreetAddress': [Text] Down 'SenderCity': [Text] Down 'SenderEmail': [Email];

Receiver:
Text(To:) Down 'ReceiverName': [Text] Down 'ReceiverStreetAddress': [Text] Down 'ReceiverCity': [Text] Down 'ReceiverEmail': [Email];

TotalAmount:
Text(Total Due) Right [Amount];
";

      var documentLab = new DocumentInterpreter();

      PdfDocument pdfDocument = PdfDocument.Load("Data\\SomeDemoInvoice.pdf");

      var firstPageResults = documentLab.InterpretToJson(script, (Bitmap)pdfDocument.Render(0, 250, 250, PdfRenderFlags.CorrectFromDpi));

      //Assert.AreEqual("DEMO - Sliced Invoices".ToLower(), firstPageResults["Sender"].GetResultByKey("SenderName").ToLower());
      //Assert.AreEqual("Suite 5A-1204".ToLower(), firstPageResults["Sender"].GetResultByKey("SenderApartment").ToLower());
      //Assert.AreEqual("123 Somewhere Street".ToLower(), firstPageResults["Sender"].GetResultByKey("SenderStreetAddress").ToLower());
      //Assert.AreEqual("Your City AZ 12345".ToLower(), firstPageResults["Sender"].GetResultByKey("SenderCity").ToLower());
      //Assert.AreEqual("admin@slicedinvoices.com".ToLower(), firstPageResults["Sender"].GetResultByKey("SenderEmail").ToLower());

      //Assert.AreEqual("Test Business".ToLower(), firstPageResults["Receiver"].GetResultByKey("ReceiverName").ToLower());
      //Assert.AreEqual("123 Somewhere st".ToLower(), firstPageResults["Receiver"].GetResultByKey("ReceiverStreetAddress").ToLower());
      //Assert.AreEqual("Melbourne, VIC 3000".ToLower(), firstPageResults["Receiver"].GetResultByKey("ReceiverCity").ToLower());
      //Assert.AreEqual("test@test.com".ToLower(), firstPageResults["Receiver"].GetResultByKey("ReceiverEmail").ToLower());

      //Assert.AreEqual("93.50", firstPageResults["TotalAmount"].GetResultAt(0));
    }
  }
}
