# Fluent C# interface to DocumentLab

If scripts aren't your thing and you'd prefer to use a C# interface to DocumentLab when extracting data from document images you can use the fluent query extensions. 

Data is always returned contained in a string object. It is up to the caller to do any necessary type conversions.

**Header**
```C#
using DocumentLab;
using System.Drawing;
```

**Implementation**
```C#
using (var dl = new Document((Bitmap)Image.FromFile("pathToSomeImage.png")))
{
  // We know our document has the customer number to the right of the label, we can be very specific
  string customerNumber = dl.Query().GetValueForLabel("Customer number", Direction.Right);

  // We can ask DocumentLab to find the closest to the label. The text type of the value to match is by default "Text".
  string invoiceNumber = dl.Query().FindValueForLabel("Invoice number");

  // Here we ask DocumentLab to specifically find a date value for the specified label
  string dueDate = dl.Query().FindValueForLabel("Due date", TextType.Date);

  // We can build patterns using predicates, directions and capture operations that return the value matched in the document
  string receiverName = dl
    .Query()
    .Match("PostCode") // All methods with text type parameters offer the TextType enum as well as a string variant of the method, this is because dynamically loaded contexgtual data files aren't statically defined'
    .Up()
    .Match("Town")
    .Up()
    .Match("StreetAddress")
    .Up()
    .CaptureSingle(TextType.Text)

  // We can build patterns that yield multiple results, the results need to be named and the response is a Dictionary<string, string>
  Dictionary<string, string> receiverInformation = dl
    .Query()
    .MultiCapture("PostCode")
    .Up()
    .MultiCapture("Town")
    .Up()
    .MultiCapture("StreetAddress")
    .Up()
    .MultiCapture("TextType.Text")
    .ExecuteMultiCapture(); // This method executes the query built up so far and returns the dictionary response.

  // We can ask for all dates in a document by using the GetAny method
  string[] dates = dl.Query().GetAny(TextType.Date);
}
``` 
