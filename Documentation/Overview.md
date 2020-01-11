# Technical overview

The basic idea is, you have the following image of an invoice,
<img src="https://i.imgur.com/t6KHwN7.png" width="80%" />

The solution performs OCR on the invoice, text analysis and then places that information in a grid that reflects the original image such as the following,
<img src="https://imgur.com/xRDr8t3.png" width="80%" />

Let's say we're out to find out the receiver name, normally the structure of how they're presented on an invoice follows a pattern of name -> address -> city -> postal code. If we look at just the original invoice, intuitively, it would make sense for us to define a query such as the following,
```
ReceiverName: PostalCode Up City Up StreetAddress Up [Text];
```

DocumentLab implements a process making this a possibility. This section provides more of a technical overview of the components that participate in the process. 

# Caching and optimization 

Upon instantiation and first use DocumentLab might take a few seconds more to respond. This is because it needs to allocate resources related to Tesseract. Upon further uses on the same instance, DocumentLab will respond faster. 

DocumentLab provides configuration options for allocation and parallelization of the Tesseract OCR resources in Ocr Configuration.json
* Tesseract OCR is performed in parallel
* We have a "pool" of tesseract engines
  * OCR jobs are batched and assigned to engines in the pool
  * Each engine can only be used by one thread at a time
  * Each engine tracks how many threads are waiting for it
  * When a new OCR request comes in, we assign it to the engine in the pool with the fewest awaiters
* Configuration options include
  * Pool size
  * Thread limit
  * Batch size

# Components

The library consists of several components that play a part in the process. The following figure represents a high-level view of how they interact and the direction of data throughout them,

![Imgur](https://i.imgur.com/UiJhaVi.png)

## Image processing & OCR
Before we perform any actual OCR the bitmap is run through filters and analyzed in order to identify sections in the bitmap which contain cohesive information. The bitmap is split and cropped into several smaller bitmaps of those sections. This allows us to OCR sections in parallel and it appears that the OCR performance is not linear with respect ro image size. This increases performance quite a bit. 

The OCR processing makes sure to maintain positional information for lines of text that belong together. This information is preserved all throughout and used by the PageAnalyzer when building a grid of the data. 

The OCR is handled by Tesseract and the image processing by Imagemagick and EmguCV.

## Text classification
The TextAnalyzer component in this solution is able to identify the following types of text out of the box,
* AmountOrNumber
* Amount
* Date
* Email
* InvoiceNumber
* Letters
* Number
* PageNumber
* Percentage
* Text 
* WebAddress
* Additional definitions from context files
* Additional custom definitions from configuration file

The text analysis and classification happens when the grid datastructure representing the page is built. Each cell containing a piece of text will have its text divided into the available classifications. When a piece of text contains a number the classification result will include an entry with just the number and an associated TextType property with a value of number. This is useful later on when we define patterns and extracting data. We can tell DocumentLab for instance to follow a pattern and then extract the number at the end of it, regardless of whether the original ocr result contained more text in that specific location. 

### Page analysis
Once the OCR and text classification step of the process is finished, we've got a set of analyzed text and their coordinate positions. The process that comes next is referred to as *Page Analysis* whereupon we take this information and build up a grid datastructure whose cells try to reflect the position of text extracted from the document as accurately as possible. Each cell in the grid is an N-dimensional array where N is the number of text classifications that were found for each text. The grid is essentially a three-dimensional array where the X and Y reflect position and the Z axis the possible matching text classifications.

Additionally: since we're dealing with images we don't trust the exact pixel positions completely in order to build the grid. So we use a lot of rounding there to begin with. A Page Trimmer component on top of that provides an algorithm to fix any issues that might occur because of rounding. Resulting in a very reliable grid. 

### Interpreter
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
