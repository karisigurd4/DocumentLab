namespace DocumentLab.Test
{
  using DocumenLab;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using System.Drawing;

  [TestClass]
  public class FullInvoice_Test
  {
    [TestMethod]
    public void Can_Interpret_FakeInvoice()
    {
      string script = @"
InvoiceNumber: 
Text(Invoice Number) Right [Text];

InvoiceDate:
Text(Invoice date) Right [Date];

DueDate:
Text(Due date) Right [Date];

Receiver:
'Name': [Text] Down 
'Address': [StreetAddress] Down 
'PostalCode': [Number] 
'Town': [Town] Down 
'Country': [Country];

PaymentTerms:
Text(Payment term) Right [Number];

InterestRate: 
Text(Interest) Right [Percentage];

OrganizationNumber: 
Text(Organization number) Right [Text];

Items:
Table 
'ItemNo': [Text(ItemNumber)] 
'Description': [Text(Description)] 
'Quantity': [Number(Quantity)] 
'UnitPrice': [AmountOrNumber(Unit price)] 
'VatPercentage': [Percentage(VAT)] 
'Total': [AmountOrNumber(Total)];

TotalAmount:
Text(Total amount) Right [Amount];
";

      var documentLab = new DocumentInterpreter();

      //var page = documentLab.GetAnalyzedPage((Bitmap)Image.FromFile("Data\\fakeinvoice.png"));
      //string csv = string.Empty;
      //for (int i = 0; i < page.Contents.GetLength(1); i++)
      //{
      //  for (int x = 0; x < page.Contents.GetLength(0); x++)
      //  {
      //    csv = csv + JsonConvert.SerializeObject(page.Contents[x, i]).Replace(",", ";") + ", ";
      //  }
      //  csv += "\n";
      //}
      //csv = csv.Substring(0, csv.LastIndexOf(',') - 1);
      //File.WriteAllText("test.csv", csv);

      var result = documentLab.InterpretToJson(script, (Bitmap)Image.FromFile("Data\\fakeinvoice.png"));

      Assert.IsNotNull(result);
    }
  }
}
