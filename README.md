<img src="https://raw.githubusercontent.com/karisigurd4/DocumentLab/master/Documentation/logo.png" width="70%" />

[![NuGet version (DocumentLab-x64)](https://img.shields.io/nuget/v/DocumentLab-x64.svg?style=flat-square)](https://www.nuget.org/packages/DocumentLab-x64/1.1.0) ![License)](https://img.shields.io/github/license/karisigurd4/DocumentLab) ![Platform](https://img.shields.io/badge/platform-win--64-green)

---
This is a solution for data extraction from images of documents. You send in a bitmap, a set of queries and get extracted data in structured json. 

* DocumentLab takes care of image processing/recognition, optical character recognition, [text classification](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Configuration.md#Language-configuration-and-text-analysis) and data analysis
* DocumentLab does not rely on pre-determined manual *mapping/localization* of sections 
  * You'll never have to specify pixel coordinates
  * DocumentLab is well versed for extracting data from documents with layouts it has never seen before
* It provides a [query language](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md) created specifically for document data extraction 
  * Defining documents by [queries and patterns](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md#building-patterns) allows DocumentLab to understand documents *intelligently*
  * Patterns in DocumentLab are designed to be analogous to how we'd read information with human intuition 
  * A [C# interface](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/FluentDocumentLab.md) to the query language is also supported
* **Production ready** for **real-world** data extraction
  * Fits seamlessly into a scalable architecture 
  * Speed and efficiency are among the primary design goals
  * [Optimizable via configuration](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Configuration.md#Host-system-resource-optimization) for maximum performance on a single host

**Quick intro**

You can write scripts in the query language or use the C# api.

The C# fluent interface is easier to get started quickly. The raw text scripting interface allows more versatility and configurability in a production context. 

* *Pattern*: A description of how information is presented in a document as well as which data to capture
  * *e.g*: ```Text(Total amount) Right [Amount]```
* *Query*: A named set of patterns prioritized first to last
  * *e.g*: ```IncoiceNumber: *pattern 1*; *pattern 2*; ... *pattern n*;```
* *Script*: A collection of queries to execute in one go. Output properties will have the query name

**Getting started**

* [Configure C# build settings](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Configuration.md) 
* [Query language documentation](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md) 
  * [C# Example: Executing scripts](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md#using-the-library) 
* [Fluent C# interface documentation](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/FluentDocumentLab.md) 

**Quick examples**

Below are a few select examples with comments on how DocumentLab can be used. 

**C# Fluent Query Example**
```C#
using (var dl = new Document((Bitmap)Image.FromFile("pathToSomeImage.png")))
{
  // Here we ask DocumentLab to specifically find a date value for the specified possible labels
  string dueDate = dl.Query().FindValueForLabel(TextType.Date, "Due date", "Payment date");

  // Here we ask DocumentLab to specifically find a date value for the specified label in a specific direction 
  string customerNumber = dl.Query().GetValueForLabel(Direction.Right, "Customer number");

  // We can build patterns using predicates, directions and capture operations that return the value matched in the document
  // Patterns allow us to recognize and capture data by contextual information, i.e., how we'd read for example receiver information from an invoice
  string receiverName = dl
    .Query()
    .Match("PostCode") // Text classification using contextual data files can be referenced by string
    .Up()
    .Match("Town")
    .Up()
    .Match("City")
    .Up()
    .Capture(TextType.Text); // All text type operations can also use the statically defined text type enum
} 
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
* NuGet - [Link](https://www.nuget.org/packages/DocumentLab-x64/)
  
# Documentation
The case with any OCR process is that the quality of the output depends entirely on the quality of source image you pass into it. If the original image quality is very low then you can expect very low quality OCR results. 

Depending on your image source, you may want to upsample low dpi images to a range between 220 - 300. This often also becomes a question of quality vs. time in terms of execution time and should be adjusted to your requirements.

* [Release notes](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/RealeaseNotes.md)
* [Simple scripting example](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md)
  * [Integrating scripts in C#](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md#using-the-library)
* [C# Fluent API](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/FluentDocumentLab.md) 
* [Query language](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md)
  * [Building patterns](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md#building-patterns)
    * [Priority](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md#priority)
  * [Text matching](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md#text-matching)
  * [Right-Down search](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md#right-down-search)
  * Extracting table data (soon)
  * Query a subset of the page (soon)
* [Configuration](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Configuration.md)
  * [Host system resource optimization](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Configuration.md#host-system-resource-optimization) 
  * [Language configuration and text analysis](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Configuration.md#language-configuration-and-text-analysis)
    * [Amounts and dates](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Configuration.md#amounts-and-dates)
    * [Defining custom text types](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Configuration.md#defining-custom-text-types)
  * [Contextual data files](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Configuration.md#contextual-data-files)
    * [Adding contextual information files](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Configuration.md#adding-contextual-information-files)
