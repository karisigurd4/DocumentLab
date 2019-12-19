# DocumentLab
This is a solution for optical character recognition and information retrieval from documents with textual information. The entire process is implemented so that for you the end user you only need to specify in terms of *queries* and *patterns* what data you would like to retrieve. You send in a bitmap of a document and a *script* and the rest is taken care of for you.

Examples:

Want to capture all dates in a document?
```
AllDates:
Any [Date];
```

Want to capture invoice receiver information in one go? 
```
Receiver:
'Name': [Text] Down 'Address': [StreetAddress] Down 'City': [Town] Down 'PostalCode': [PostalCode];
```

Want to capture by a label -> value?
```
YourData:
Text(LabelToMatch) RD 10 'MyData': [Text];
```

Let's say you have another document, similar in structure but the label is different, you can make the same script capture that one by adding to the pattern predicate parameters like so,
```
YourData:
Text(LabelToMatch||AnotherLabelToMatch) RD 10 'MyData': [Text];
```

One of the goals of the project was to be able to query textual data from images based solely on contextual information that would not depend on any previously determined localization. In order to achieve that goal, the process implementation in this library builds up a grid datastructure whose cells are analogous to the actual locations of text from the image. Each cell will contain the corresponding text from the document as well as the text classifications that were identified. The query language introduces a *pattern* concept which allows us to define a context and what we want to capture.

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

### Querying the page
#### Page traversal
Querying the page datastructure described above is made possible by defining a conceptual entity called *Page Traverser*. The page traverser has the following mode of behavior,
* It can be initialized on any cell in the page
* It can move Up, Down, Left or Right
* Upon moving, it will traverse in the specified direction until it encounters a nonempty cell and stops
* It reports erroneous states when it moves beyond the boundaries of the page

#### Patterns
The query language implementation defines a conecpt of a *pattern*. A pattern consists of a set of predicates, directions for the page traverser to move, and a set of one or more captures. A predicate in a pattern can be any text type classified in the TextAnalyzer component, moreover, a predicate can be made more definite by stating what explicit text should be matched within it (**note:** fuzzy matching using Levenshtein distance 2 is implicit by default).

A pattern in the query language is defined in the following ANTLR4 syntax,
```
Any? ( capture | traverse | textType | rightDownSearch )* SemiColon
```

An explanation of each token follows,
* **Any**: (Optional) Normally the interpreter will stop after finding a single match, specifying **Any** at the start of a pattern indicates to the interpeter to find **All** instances matching the pattern in the document.
* **TextType**: This is a predicate in the pattern. This can be any text classification resulting from the TextAnalyzer. The pattern can be made more definite by specifying for example *Text("CustomerNumber")* meaning that only cell with a TextType: Text and a Value: "CustomerNumber* is a match. Furthermore, definite value matches can be separated by || if for example "CustNo" should also be considered. If the interpreter determines that a page traverser is not on a cell matching the text type specified in the pattern it will not continue. 
* **Traverse**: Direction for the PageTraverser to move at any step in the pattern, can be "Up", "Down", "Left", "Right"
* **Captue**: A capture token is defined with the following syntax: [TextType], for example: [Text]. When a PageTraverser reaches a capture token the interpreter will check if the specified text type in it matches the cell the traverser is on. If it is a match the interpreter will add the value of the current cell to its result set. A pattern can contain any number of capture tokens, they can be further differentiated in the result set with the following syntax: 'ResultPropertyName': [Text], for example: 'CustomerNumber': [Text]
* **RightDownSearch**: This is a more advanced search than just the normal set of directions that the PageTraverser offers. This can best be explained by example. Say we have invoice A that has a label 'Customer number' and a value 'C001' below it, then we have another invoice B that has a label 'Customer number' and a value 'C002' to the right of it. A *Right Down Search* will split the current page traverser into two at its current location and traverse down with one and right with the other. The traverser which makes a correct match with the rest of the pattern first will be chosen as the one to use for a result. The syntax for this token is *RD* and a numerical value follows designating the number of steps we'll allow the traverser to step through before we stop the search. The reason being that we can define how far away the next step in the pattern can be.

#### Query language
The query language interpretation output depends on pattern defintiions described above. A set of queries is referred to as a *Script*. A script can define individual query labels and then a set of prioritized patterns associated to that label. An example in case of invoices would be,
```
InvoiceNumber:
Text(Invoice number||Invoice no) Right [InvoiceNumber];
Text(Invoice number||Invoice no) Down [InvoiceNumber];
```

The first pattern is considered highest priority, if a match cannot be found for it the interpreter will continue onto the next until it finds a match. If no patterns match then the result is empty. The result will contain the captured [InvoiceNumber] cell value. A script can contain any number of query output labels + prioritized pattern definitions. A single script can define all query labels and patterns to interpret the contents of an entire invoice, the use of prioritization in patterns means that a script can potentially be defined to handle countless different types of invoices. 
