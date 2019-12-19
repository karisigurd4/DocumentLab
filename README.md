# DocumentLab
A two year research project culminating in a generalized method for querying textual information from images. Involving a mix of optical character recognition, text classification, data structuring and a query language implementation

What does it do?

1. You send in a bitmap (Of a document or whatever data you'd like to query)
2. It analyzes the structure of the bitmap and performs optical character recognition on it
3. It performs text classification on the results
4. It builds a three dimensional matrix of the results
5. We send in a script to query the results

... More documentation pending

## How to use

The DocumentLab component exposes a simple to use method called 'Interpret' for interacting with the underlying libraries. The method accepts a script containing queries as well as a bitmap to perform the queries upon.

The bitmap sent in must be at least > 250 DPI for the OCR to yield any usable results.

For a query script example, if we query an invoice we can get the receiver's name, address and city as with the following query,
```
Receiver:
'ReceiverName': [Text] Down 'ReceiverStreetAddress': [StreetAddress] Down 'ReceiverCity': [City] Down 'ReceiverEmail': [Email];
```
