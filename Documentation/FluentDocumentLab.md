# Fluent C# interface to DocumentLab

If [scripts](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md) aren't your thing and you'd prefer to use a C# interface to DocumentLab when extracting data from document images you can use the fluent query extensions. Data is always returned contained in a string object or in a dictionary in the case of a multi capture. It is up to the caller to do any necessary type conversions.

#### Code example 
**Header**
```C#
using DocumentLab;
using System.Drawing;
```

**Implementation**
```C#
using (var dl = new Document((Bitmap)Image.FromFile("pathToSomeImage.png")))
{
  // We know our documents have the customer number to the right of the labels we know of, we can be very specific about the direction
  string customerNumber = dl.Query().GetValueForLabel(Direction.Right, "Customer number", "Cust no");

  // We can ask DocumentLab to find the closest to the labels. The text type of the value to match is by default "Text".
  string invoiceNumber = dl.Query().FindValueForLabel("Invoice number");

  // Here we ask DocumentLab to specifically find a date value for the specified label
  string dueDate = dl.Query().FindValueForLabel(TextType.Date, "Due date");

  // We can build patterns using predicates, directions and capture operations that return the value matched in the document
  string receiverName = dl
    .Query()
    .Match("PostCode")
    .Up()
    .Match("Town")
    .Up()
    .Match("StreetAddress")
    .Up()
    .Capture(TextType.Text);

  // We can build patterns that yield multiple results, the results need to be named and the response is a Dictionary<string, string>
  Dictionary<string, string> receiverInformation = dl
    .Query()
    .Capture(q => q
      .Capture("PostCode", "PostCode") // All methods with text type parameters offer the TextType enum as well as a string variant of the method, this is because dynamically loaded contexgtual data files aren't statically defined
      .Up()
      .Capture("Town", "Town")
      .Up()
      .Capture("StreetAddress", "StreetAddress")
      .Up()
      .Capture(TextType.Text, "ReceiverName")
    );

  // We can ask for all dates in a document by using the GetAny method
  string[] dates = dl.Query().GetAny(TextType.Date);
}
``` 
