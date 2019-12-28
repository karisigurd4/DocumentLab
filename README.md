
[![NuGet version (DocumentLab-x64)](https://img.shields.io/nuget/v/DocumentLab-x64.svg?style=flat-square)](https://www.nuget.org/packages/DocumentLab-x64/1.1.0) ![License)](https://img.shields.io/github/license/karisigurd4/DocumentLab) ![Platform](https://img.shields.io/badge/platform-win--64-green)

# DocumentLab
This is a solution for data extraction from documents. You pass in an bitmap of the document and a set of queries and you get back your extracted data in structured json. 

Queries are patterns of information in documents that you want to match. If DocumentLab can find a match, you can capture any data from a pattern. The queries look like the following,
* Your document has a label "Customer number:" and a value to the right of it
  * Query: ```CustomerNumber: Text(Customer number) Right [Text];```
  * Match text labels with implicit *starts with* and Levensthein distance 2 comparison
* Your document has a label "Invoice date" and a date below it
  * Query: ```InvoiceDate: Text(Invoice date) Down [Date];```
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
* [Example](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md)
  * [Image](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md#image)
  * [Script](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md#script)
  * [Output](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md#output)
  * [Using the library](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md#using-the-library)
* [Techical overview](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md)
  * [Components](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#components)
    * [Image processing & OCR](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#image-processing-&-ocr)
    * [Text classification](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#text-classification)
    * [Page analysis](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#page-analysis)
    * [Interpreter](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#interpreter)
      * [Page traversal](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#page-traversal)
      * [Patterns](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#patterns)
      * [Query language](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#query-language)
