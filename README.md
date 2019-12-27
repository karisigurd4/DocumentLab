
[![NuGet version (DocumentLab-x64)](https://img.shields.io/nuget/v/DocumentLab-x64.svg?style=flat-square)](https://www.nuget.org/packages/DocumentLab-x64/1.1.0) ![License)](https://img.shields.io/github/license/karisigurd4/DocumentLab) ![Platform](https://img.shields.io/badge/platform-win--64-green)

# DocumentLab
This is a solution for optical character recognition and information retrieval from documents with textual information. The entire process is implemented so that you only need to specify in terms of *queries* what data you would like to retrieve. You send in a bitmap of a document and a *query script* and the rest is taken care of for you.

One of the goals of the project was to be able to query textual data from images based solely on contextual information that would not depend on any previously determined localization. In order to achieve that goal, the process implementation in this library builds up a grid datastructure whose cells are analogous to the actual locations of text from the image. Each cell will contain the corresponding text from the document as well as the text classifications that were identified. The query language introduces a *pattern* concept which allows us to define a context and what we want to capture.

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
