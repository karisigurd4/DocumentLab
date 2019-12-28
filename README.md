
[![NuGet version (DocumentLab-x64)](https://img.shields.io/nuget/v/DocumentLab-x64.svg?style=flat-square)](https://www.nuget.org/packages/DocumentLab-x64/1.1.0) ![License)](https://img.shields.io/github/license/karisigurd4/DocumentLab) ![Platform](https://img.shields.io/badge/platform-win--64-green)

# DocumentLab
This is a solution for data extraction from documents. The entire process is implemented so that you only need to specify in terms of *queries* what data you would like to retrieve. The queries look like the following,
* Your document has a label "Customer number:" and a value to the right of it
  * Query: ```CustomerNumber: Text(Customer number) Right [Text];```
  * Match text labels with implicit *starts with* and Levensthein distance 2 comparison
* Your document has a label "Invoice date" and a date below it
  * Query: ```InvoiceDate: Text(Invoice date) Down [Date];```
* Want to capture invoice receiver info in one query?
  * Query: ```Receiver: 'Name': [Text] Down 'Address': [StreetAddress] Down 'City': [Town] Down 'PostalCode': [PostalCode];```
  * Control output structure by naming capture properties
* You want to capture all amounts in a document?
  * Query: ```AllAmounts: Any [Amount];```

You send in a bitmap of a document and a *query script* and the rest is taken care of for you. The results are returned in raw json format.

# Documentation
* [Example](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md)
  * [Image](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md#image)
  * [Script](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md#script)
  * [Output](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md#output)
  * [Using the library](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Examples.md#using-the-library)
* [Techical overview](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md)
  * [Image processing & OCR](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#image-processing-&-ocr)
  * [Text classification](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#text-classification)
  * [Page analysis](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#page-analysis)
  * [Interpreter](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#interpreter)
    * [Page traversal](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#page-traversal)
    * [Patterns](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#patterns)
    * [Query language](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/Overview.md#query-language)
