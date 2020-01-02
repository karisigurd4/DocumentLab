
[![NuGet version (DocumentLab-x64)](https://img.shields.io/nuget/v/DocumentLab-x64.svg?style=flat-square)](https://www.nuget.org/packages/DocumentLab-x64/1.1.0) ![License)](https://img.shields.io/github/license/karisigurd4/DocumentLab) ![Platform](https://img.shields.io/badge/platform-win--64-green)

# DocumentLab
This is a solution for data extraction from documents. You pass in an bitmap of a document and a set of queries and you get back your extracted data in structured json. 

Queries are patterns of information in documents that you want to match. If DocumentLab can find a match, you can capture any data from a pattern. You can write scripts in the query language or use the C# api.

**C# API**
```C#
using (var dl = new Document((Bitmap)Image.FromFile("pathToSomeImage.png")))
{
  // Here we ask DocumentLab to specifically find a date value for the specified label
  string dueDate = dl.FindValueForLabel("Due date", TextType.Date);

    // We can build patterns using predicates, directions and capture operations that return the value matched in the document
  string receiverName = dl
    .Match("PostCode") // All methods with text type parameters offer the TextType enum as well as a string variant of the method, this is because dynamically loaded contextual data files aren't statically defined'
    .Up()
    .Match("Town")
    ...
    .CaptureSingle(TextType.Text)
```

**Script example**
* Your document has a label "Customer number:" and a value to the right of it
  * Query: ```CustomerNumber: Text(Customer number) Right [Text];```
  * Match text labels with implicit *starts with* and Levensthein distance 2 comparison
* Your document has a label "Invoice date" and a date below it
  * Query: ```InvoiceDate: Text(Invoice date) Down [Date];```
  * You can capture a variety of *text types*. Even if the document contains additional text at the capture you'll only get back a standardized ISO date. 
* Want to capture invoice receiver info in one query?
  * Query: ```Receiver: 'Name': [Text] Down 'Address': [StreetAddress] Down 'City': [Town] Down 'PostalCode': [PostalCode];```
  * Json output will name properties according to the query predicate naming parameters
* You want to capture all amounts in a document?
  * Query: ```AllAmounts: Any [Amount];```
  * When we use *any*, results are returned in a json array

# Download 

* Win64-CLI (1.1.1) - [Download](https://github.com/karisigurd4/DocumentLab/raw/master/bin/DocumentLabCL-Win64.zip)
  * *Arguments: DocumentLabCL.exe "path\Script.txt" "path\Image.png" (optional) "path\Output.json"*
  * Easy to use tool for evaluation or testing scripts
  * See *Examples* folder
  
# Documentation
The case with any OCR process is that the quality of the output depends entirely on the quality of source image you pass into it. If the original image quality is very low then you can expect very low quality OCR results. 

Depending on your image source, you may want to upsample low dpi images to a range between 220 - 300. This often also becomes a question of quality vs. time in terms of execution time and should be adjusted to your requirements.

* [Example](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md)
  * [Image](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md#image)
  * [Script](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md#script)
  * [Output](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md#output)
  * [Using the library (C# example)](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md#using-the-library)
* [C# Fluent API](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/FluentDocumentLab.MD) 
* [Query language](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md)
  * [Building patterns](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md#building-patterns)
    * [Priority](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md#priority)
  * [Text matching](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md#text-matching)
  * [Right-Down search](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md#right-down-search)
  * Extracting table data (soon)
  * Query a subset of the page (soon)
* [Configuration](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Configuration.md)
  * [Host system resource optimization](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Configuration.md#host-system-resource-optimization) 
  * [Language configuration](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Configuration.md#language-configuration)
    * [Adding contextual information files](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Configuration.md#adding-contextual-information-files)
* [Techical overview](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md)
  * [Caching and optimization](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#coaching-and-optimization)
  * [Components](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#components)
    * [Image processing & OCR](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#image-processing-&-ocr)
    * [Text classification](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#text-classification)
    * [Page analysis](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#page-analysis)
    * [Interpreter](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#interpreter)
      * [Page traversal](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#page-traversal)
      * [Patterns](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#patterns)
      * [Query language](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#query-language)
