# Query language

DocumentLab takes an image of a document and builds a grid datastructure using the ocr'd text and text classifications. This datastructure can be interacted with via the query language provided in order to extract desired information. A query is essentially a container for patterns. We declare a query by a name label followed by a colon. 

Interacting with DocumentLab is also possible via a C# interface, take a look at [the fluent query documentation page](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/FluentDocumentLab.md) for information. DocumentLab scripts have the advantage of being decoupled from compiled C# code and can vary and be maintained independently. 

## Building patterns
Building DocumentLab patterns is intended to be as intuitive as reading over the information in a document. It provides a method analogous to how we interpret the information from a document using human intuition. When we want to find an invoice number or a customer number in an invoice, how do we do it? We start by finding a label correlative to the information we want to find and then we find the value in some direction next to it, same intuitive process occurs when we want to find the receiver information, it follows a pattern of information and direction to follow that we're familiar with even when there's no label next to it. The patterns we define in DocumentLab follow the same logic.

We tell DocumentLab to match patterns by indicating what data is inclusive in the pattern, it may look like,
```
SomePattern: Date Down Number Down Amount Down [Text];
```

In the pattern above, we're telling DocumentLab that there's a section of information in a document where we expect to find a date followed by a number below followed by an amount below followed by a piece of text that we want to capture using the square bracket notation around the text type.

Depending on the situation, if we want to capture more information from the same pattern, we can put square brackets around any text type to include it in the json output, 
```
SomePattern: Date Down Number Down [Amount] Down [Text];
```

Sometimes it is desirable to offload analysis of information to the host program which calls DocumentLab, for example when we have an invoice and we're out to find the total due amount. Then searching for a label -> value approach might be insufficiently reliable enough to always find a correct match, or perhaps we'd like to provide a user with a drop-down of all amounts in an invoice so that manual input can be made easier, in that case we can use the *Any* operator in a pattern,

```
AllAmounts: Any [Amount];
```

The output json result will contain an array named *'AllAmounts'* that contains every amount identified in the document. This is similarly useful for the *Date* text type when we want to find for example the *due* and *issued* dates of an invoice.

### Priority
Each query in a script can include many patterns. DocumentLab will try the patterns for each query in sequential order until one yields a successful match.

Let's say we have the following query to extract customer numbers from invoices,
```
CustomerNumber: Text(Customer number) Right [Text];
```

Then we have an invoice from another sender which has different placement of elements, the customer number might be below a *Customer Number* label instead of to the right of it, then we can extend the query with another pattern,
```
CustomerNumber: 
Text(Customer number) Right [Text];
Text(Customer number) Down [Text];
```

This extension to our query would yield the customer number from both documents. As many patterns as necessary can be added to queries. They'll always be prioritized from top to bottom.

## Text matching
Oftentimes, we'll find ourselves in a situation where the documents we want to extract information from have their data structured in a label -> value way. For example, an invoice which has the label *Customer number:* and then the value representing the customer number to the right or below it. 

In such a case, we can define a pattern starting with the following,
```
CustomerNumber: Text(Customer number) ...
```

Then depending on if the expected value is below or to the right of the label (let's assume to the right for now) we extend our pattern with 
```
CustomerNumber: Text(Customer number) Right [Text]
```

Since we have implicit Levenshtein distance 2 text matching, as well as *starts with* matching on text we can make our pattern more durable to lower quality documents (such as when a picture is taken with a phone or a low quality scanner is used) by simplifying the text matching in the following way
```
CustomerNumber: Text(Customer nu) Right [Text]
```

When we have invoices from two different senders, the label they use for *Customer Number* might be entirely different, but we want to extract the customer number from both senders without needing to know beforehand which document type we're dealing with, then we can extend the list of text we can match for this pattern with the logical or || operator, let's assume the other sender uses *Cust No* as a label.

We can simplify *Cust No* to *Cust N* for the same durability reason above, 
```
CustomerNumber: Text(Customer nu || Cust n) Right [Text]
```

Now we'll be able to match the customer number labels from both document types from the same query. A text match can include as many || operators as necessary to handle the varieties of labels documents might have for the same information.

## OR operator on text types and captures. 

Sometimes you'll find that a pattern's predicates aren't sufficient to define across varying documents. You might want to extend those using the logical or operator. 

You can do that on text type predicates as well as on captures. Following example assumes we've defined a more specific custom customer number text type. 

```
CustomerNumber: Text(Customer number) || Letters(CustNo) Right [CustomerNumber || Text]
```

## Right-Down search
Right-Down search is specifically intended for the *find a label and then a value below or to the right of the label* use case. 

In the [priority](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md#priority) example above, we explored the possibility to extend queries with more patterns in order to solve the situation when a customer number may be on the right of a label in some cases and below a label in other cases. DocumentLab provides another operation specifically for this situation referred to as Right-Down search. 

A Right-Down search pattern can be expressed with the following syntax
```
CustomerNumber: RD 6 Text(Customer nu || Cust n) [Text];
```

The numeric value following RD indicates the maximum number of distance in cells in the DocumentLab page grid the following predicate can be in order to consider the pattern matched or not. This helps when there is actually information to the right or below the label but it's too far away to be considered relevant.

## Extracting table data (soon)

## Query a subset of the page

Querying a SubSet of a document might be necessary if the same pattern of information repeats on different locations and pattern definitions cannot distinguish between them. 

For example. An invoice has the receiver commonly at the top right of the page. We can explicitly limit the part of the document we query with the following syntax. 

```
QueryName: Subset(Top 30, Right 50) Postal Code Up ... 
```

The example above limits the query to the top 30% of the page and the right 50% of the page.

We can use Top, Bottom, Left and right to limit. 
