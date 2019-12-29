# Query language

## Building patterns

### Priority
Each query in a script can include many patterns. DocumentLab will try the patterns for each query in sequential order until one yields a successful match.

Let's say we have the following query to extract customer numbers from invoices,
```
CustomerNumber: Text(Customer number) Right [Text];
```

Then we have an invoice from another sender which has different placement of elements, the customer number might be below a *Customer Number* label, then we can extend the query with another pattern,
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

We can simplify *Cust No* to *Cust N* för the same durability reason above, 
```
CustomerNumber: Text(Customer nu || Cust n) Right [Text]
```

Now we'll be able to match the customer number labels from both document types from the same query. A text match can include as many || operators as necessary to handle the varieties of labels documents might have for the same information.

## Right-Down search

## Extracting table data (soon)

## Query a subset of the page (soon)
