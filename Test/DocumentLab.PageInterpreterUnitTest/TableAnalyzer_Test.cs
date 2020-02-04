namespace DocumentLab.PageInterpreterUnitTest
{
  using DocumentLab.Contracts.Utils;
    using DocumentLab.PageInterpreter;
    using DocumentLab.PageInterpreter.Components;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class TableAnalyzer_Test : InterpreterTestBase
  {
    [TestMethod]
    public void Can_Analyze_Basic_Table()
    {
      var fakePage = new TestPageCreator(5, 10);

      fakePage.SetValue(0, 0, "Number", "ItemNo");
      fakePage.SetValue(1, 0, "Text", "Description");
      fakePage.SetValue(2, 0, "Number", "Quantity");
      fakePage.SetValue(3, 0, "Amount", "Unit price");
      fakePage.SetValue(4, 0, "Amount", "Amount");

      fakePage.SetValue(0, 1, "Number", "22");
      fakePage.SetValue(0, 2, "Number", "33");
      fakePage.SetValue(0, 3, "Number", "44");

      fakePage.SetValue(1, 1, "Text", "A");
      fakePage.SetValue(1, 2, "Text", "B");
      fakePage.SetValue(1, 3, "Text", "C");

      fakePage.SetValue(2, 1, "Number", "3");
      fakePage.SetValue(2, 2, "Number", "2");
      fakePage.SetValue(2, 3, "Number", "5");

      fakePage.SetValue(3, 1, "Amount", "500.0");
      fakePage.SetValue(3, 2, "Amount", "245.5");
      fakePage.SetValue(3, 3, "Amount", "899.99");

      fakePage.SetValue(4, 1, "Amount", "1250.5");
      fakePage.SetValue(4, 2, "Amount", "1500.2");
      fakePage.SetValue(4, 3, "Amount", "1345.3");

      //      var script = @"
      //AnalyzedTable: 
      //Table 
      //'ItemNumber': [Number(ItemNo)] 
      //'ItemDescription': [Text(Description)] 
      //'Quantity': [Number(Quantity)] 
      //'ItemPrice': [Amount(Unit price)] 
      //'TotalAmount': [Amount(Amount)];
      //";

      //      var result = Visit(script, fakePage.Page);

      var tableAnalyzer = new TableAnalyzer();
      var analysis = tableAnalyzer.AnalyzeTable(fakePage.Page, "Test", new PageInterpreter.DataModel.TableColumn[]
      {
        new PageInterpreter.DataModel.TableColumn()
        {
          ColumnName = "ItemNumber",
          LabelParameters = new string[] { "ItemNo" },
          TextType = "Number"
        },
        new PageInterpreter.DataModel.TableColumn()
        {
          ColumnName = "Description",
          LabelParameters = new string[] { "Description" },
          TextType = "Text"
        },
        new PageInterpreter.DataModel.TableColumn()
        {
          ColumnName = "Quantity",
          LabelParameters = new string[] { "Quantity" },
          TextType = "Number"
        },
        new PageInterpreter.DataModel.TableColumn()
        {
          ColumnName = "UnitPrice",
          LabelParameters = new string[] { "Unit price" },
          TextType = "Amount"
        },
        new PageInterpreter.DataModel.TableColumn()
        {
          ColumnName = "Total",
          LabelParameters = new string[] { "Amount" },
          TextType = "Amount"
        }
      });

      Assert.IsNotNull(analysis);
    }
  }
}
