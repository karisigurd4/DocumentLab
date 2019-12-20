# DocumentLab
This is a solution for optical character recognition and information retrieval from documents with textual information. The entire process is implemented so that you only need to specify in terms of *queries* what data you would like to retrieve. You send in a bitmap of a document and a *script* and the rest is taken care of for you.

One of the goals of the project was to be able to query textual data from images based solely on contextual information that would not depend on any previously determined localization. In order to achieve that goal, the process implementation in this library builds up a grid datastructure whose cells are analogous to the actual locations of text from the image. Each cell will contain the corresponding text from the document as well as the text classifications that were identified. The query language introduces a *pattern* concept which allows us to define a context and what we want to capture.

## Overview

The basic idea is, imagine you have the following top half of an invoice as an image file,
![Imgur](https://i.imgur.com/t6KHwN7.png)

We'll perform OCR on the invoice, text analysis and then place that information in a grid that reflects the original image such as the following,
![Imgur](https://imgur.com/xRDr8t3.png)

Let's say we're out to find out the receiver name, normally the structure of how they're presented on an invoice follows a pattern of name -> address -> city -> postal code. We can define a query such as the following,
```
ReceiverName: PostalCode Up City Up StreetAddress Up [Text];
```

And then we have an interpreter which can traverse the grid and verify if any actual pattern in the grid matches the pattern we define in our query. The [Text] item in the query indicates this is the one we want to capture, i.e., the receiver name.

...or if we want to capture all receiver information in one go,
```
Receiver:
'Name': [Text] Down 'Address': [StreetAddress] Down 'City': [Town] Down 'PostalCode': [PostalCode];
```

Then we get the following output,
```
"Receiver": {
  "Name": "John Adams",
  "Address": "SomeStreet 13",
  "City": "ImagineCity",
  "PostalCode": "553322"
}
```

Or if we want to be able to interpret two distinct types of invoices, one which follows the previous pattern and another which might have a pattern like name -> receiver address -> co address -> city -> postal code. Then we can add a new pattern below the first one like, 
```
Receiver:
'Name': [Text] Down 'Address': [StreetAddress] Down 'City': [Town] Down 'PostalCode': [PostalCode];
'Name': [Text] Down 'Address': [StreetAddress] Down Address Down 'City': [Town] Down 'PostalCode': [PostalCode];
```

The priority of which patterns to use if they match anything in the grid is from top to bottom. So if the first one doesn't match, we'd still get a match on the second one for the different type of invoice. Allowing a lot of flexibility for differing document types within one query definition.

We can extend our queries by adding,
```
TotalAmount: Text(TotalAmount) [Amount];
Dates: Any [Date];
```

Which would result in,
```
...
"TotalAmount": { "0": "500.00" },
"Dates": [
  "2019-12-30",
  "2020-01-10"
]
```

... Just to provide some example

The library consists of distinct components that play a distinct part in the process. The following figure represents a high-level view of how they interact and the direction of data throughout them,
![Imgur](https://i.imgur.com/UiJhaVi.png)

The following sections will explain the parts each component plays more extensively.

## Image processing & OCR

... 

## Text classification
The TextAnalyzer component in this solution is able to identify the following types of text out of the box,
* AmountOrNumber
* Amount
* Date
* Email
* InvoiceNumber
* Letter
* Number
* PageNumber
* Percentage
* Text 
* WebAddress

These standard text types are defined by regular expressions in *Data\Configuration\TextAnalysis.json* that can be edited and refined without recompilation. 

Moreover, the TextAnalyzer also contains implementation and configuration options for defining custom text types by providing a list of items separated by newline. For example, the provided file StreetAddress.txt under *Data\Context\** contains a list of street addresses in Sweden. The TextAnalyzer will automatically load the contents of this file into memory and any text matching an item in the file will get classified as a *StreetAddress*.

For example, if we pass the string "Storgatan 15" through the TextAnalyzer, it will return a set of results such as the following,
```
[
  { Text: "Storgatan 15", TextType: "StreetAddress" },
  { Text: "15", TextType: "AmountOrNumber" },
  { Text: "15", TextType: "Number" },
  { Text: "Storgatan 15", TextType: "Text" },
]
```
Note that no additional configuration is necessarily required for adding these context files to the solution. They are loaded and introduced into the process automatically. Some additional configuration is possible in *Data\Configuration\FromFileConfiguration.json*.

### Page analysis
Once the OCR and text classification step of the process is finished, we've got a set of analyzed text and their coordinate positions. The process that comes next is referred to as *Page Analysis* whereupon we take this information and build up a grid datastructure whose cells try to reflect the position of text extracted from the document as accurately as possible. Each cell in the grid is an N-dimensional array where N is the number of text classifications that were found for each text. The grid is essentially a three-dimensional array where the X and Y reflect position and the Z axis the possible matching text classifications.

### Interpreter: Querying the page
#### Page traversal
Querying the page datastructure described above is made possible by defining a conceptual entity called *Page Traverser*. The page traverser has the following mode of behavior,
* It can be initialized on any cell in the page
* It can move Up, Down, Left or Right
* Upon moving, it will traverse in the specified direction until it encounters a nonempty cell and stops
* It reports erroneous states when it moves beyond the boundaries of the page

#### Patterns
The query language implementation defines a conecpt of a *pattern*. A pattern can be thought of as a set of commands which the interpreter and page traverser must execute. A pattern definition is executed left to right. A pattern yields a result from its capture token declarations if the interpreter encounters no error in the execution of the whole pattern. The interpreter implementation provided in this solution works by iterating through each cell in the page grid, initializing a page traverser and testing whether the whole pattern can be executed without error. 

A pattern consists of a set of predicates, directions for the page traverser to move, and a set of one or more captures. A predicate in a pattern can be any text type classified in the TextAnalyzer component, moreover, a predicate can be made more definite by stating what explicit text should be matched within it (**note:** fuzzy matching using Levenshtein distance 2 is implicit by default).

A pattern in the query language is defined in the following ANTLR4 syntax,
```
Any? ( capture | traverse | textType | rightDownSearch )* SemiColon
```

An explanation of each token follows,
* **Any**: (Optional) Normally the interpreter will stop after finding a single match, specifying **Any** at the start of a pattern indicates to the interpeter to find **All** instances matching the pattern in the document.
* **TextType**: This is a predicate in the pattern. This can be any text classification resulting from the TextAnalyzer. The pattern can be made more definite by specifying for example *Text("CustomerNumber")* meaning that only cell with a TextType: Text and a Value: "CustomerNumber* is a match. Furthermore, definite value matches can be separated by || if for example "CustNo" should also be considered. If the interpreter determines that a page traverser is not on a cell matching the text type specified in the pattern it will not continue. 
* **Traverse**: Direction for the PageTraverser to move at any step in the pattern, can be "Up", "Down", "Left", "Right"
* **Capture**: A capture token is defined with the following syntax: [TextType], for example: [Text]. When a PageTraverser reaches a capture token the interpreter will check if the specified text type in it matches the cell the traverser is on. If it is a match the interpreter will add the value of the current cell to its result set. A pattern can contain any number of capture tokens, they can be further differentiated in the result set with the following syntax: 'ResultPropertyName': [Text], for example: 'CustomerNumber': [Text]
* **RightDownSearch**: This is a more advanced search than just the normal set of directions that the PageTraverser offers. This can best be explained by example. Say we have invoice A that has a label 'Customer number' and a value 'C001' below it, then we have another invoice B that has a label 'Customer number' and a value 'C002' to the right of it. A *Right Down Search* will split the current page traverser into two at its current location and traverse down with one and right with the other. The traverser which makes a correct match with the rest of the pattern first will be chosen as the one to use for a result. The syntax for this token is *RD* and a numerical value follows designating the number of steps we'll allow the traverser to step through before we stop the search. The reason being that we can define how far away the next step in the pattern can be.

#### Query language
The query language interpretation output depends on pattern defintiions described above. A set of queries is referred to as a *Script*. A script can define individual query labels and then a set of prioritized patterns associated to that label. An example in case of invoices would be,
```
InvoiceNumber:
Text(Invoice number||Invoice no) Right [InvoiceNumber];
Text(Invoice number||Invoice no) Down [InvoiceNumber];
```

The first pattern is considered highest priority, if a match cannot be found for it the interpreter will continue onto the next until it finds a match. If no patterns match then the result is empty. The result will contain the captured [InvoiceNumber] cell value. A script can contain any number of query output labels + prioritized pattern definitions. A single script can define all query labels and patterns to interpret the contents of an entire invoice, the use of prioritization in patterns means that a script can potentially be defined to handle countless different types of invoices. 
