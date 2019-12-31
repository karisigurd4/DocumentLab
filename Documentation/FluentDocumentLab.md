# Fluent C# interface to DocumentLab

N. (Soon in v1.2) 

An alternative to writing scripts via the query language. The library contains a fluent interface that makes it easier to get up and running quickly. 

```C#
using (var dl = new FluentDocumentLab(documentImage))
{
  var customerNumber = dl.GetValueForLabel<string>("Customer number", Direction.Right);

  var invoiceNumber = dl.FindValueForLabel<string>("Invoice number");

  var dueDate = dl.FindValueForLabel<DateTime>("Due date");

  var receiverName = dl
    .Capture(TextType.Text)
    .Down()
    .Match("StreetAddress")
    .Down()
    .Match("Town")
    .Down()
    .Match("PostCode");

  var dates = dl.GetAny<DateTime>(TextType.Date);
}
``` 
