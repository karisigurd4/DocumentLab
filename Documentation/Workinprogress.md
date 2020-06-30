

## DocumentLab

Back in 2017 I encountered a problem which for the subsequent 2 years I would obsess on and off over. It was one of those problems that had an air of "many have tried, none have succeeded" around it. Immediately my mind started spinning, trying to imagine possible methods for solving this problem. The more I thought about it, the more I wanted to go for it.

This was at its core a personal research project. I never knew what eventual goal I wanted to reach. I still don't, however at this time I believe the solution which I have created is sufficient enough to have a more high-level discussion about. Hopefully what I've learned can help others in a similar situation in the future or to provide useful insight for further research. 

##  The problem
How do we convert images of documents to structured json formats which we can easily integrate with our systems?
![Idea](https://i.imgur.com/2Sko0it.png)

I'll present the problem by giving a hypothetical example, the example will be limited to the domain of invoices however the example could just as well have picked any other domain in which information exchange on paper is applicable. Let's say we're building an app that allows us to keep track of our finances, one of the features of the app is that we should be able to take a picture of an invoice, let the app scan the information contained in the invoice and then store the information in a database in order to give us statistics about what we're paying for. Maybe we want to track increase or decrease in price from a given sender for some service. 

Sounds simple enough right? 

Well here comes the tricky part.

How can we teach a computer to scan invoice contents in such an intelligent way that it can organize the information automatically for us? Keep in mind that all we have to work with is an image of an invoice which contains no additional information other than pixel information. To begin with, we'll need some method for extracting text from an image. That's where OCR or Optical Character Recognition comes in. What if the photograph was taken in sub-optimal lighting conditions, or at a slight angle? That's where computer vision and image manipulation techniques can be used.

Using the techniques mentioned above a common solution to our problem has historically been to define pixel boundaries where information on invoices commonly can be found. There exists an entire industry around providing solutions that do exactly that from very specific to very general cases. But taking that approach to solving the problem has some very unfortunate drawbacks. The structure of an invoice varies so incredibly between origin that even if we know that an invoice from sender A has some piece of information at some pixel coordinates x and y there's no guarantee that an invoice from sender B will have that piece of info anywhere close by. How do we solve that? We would need to keep a database of every possible invoice structure to refer to when we want to scan information from them. 

## The idea

Given how incredibly varied the structure of an invoice can be, how do I even interpret and extract the information I need from an invoice? This was my initial thought when I heard about this problem. But it seems relatively easy to conceptualize, while invoices might not have exact recurring placement of information there are patterns at play which allow me to identify what I'm looking at. Sometimes these patterns are simple and even give hints such as with a label "Total amount" and an amount next to it. More complex patterns, such as the invoice receiver information can be inferred by the knowledge that it follows a variation of a commonly accepted pattern such as Receiver's Name -> Address -> Postal Code -> Town  -> Country.

I was interested in modeling this process and evaluating how the output would perform. My hypothesis was that if a model was sufficiently robust, all we would need to do was to define what kind of patterns to look for. An algorithm could parse the model with the patterns as input and the output should be the actual information contained within the document. Given that documents across a whole range of domains have patterns which we can intuitively interpret and use to extract information, I had a hunch that such a method could be generalized across all those domains.

I had this image in my mind for the entire process,

![Idea](https://i.imgur.com/Vaq6vFY.png)

If we can sufficiently model the document in a grid where elements would be placed intuitively enough we could define simple queries to extract whatever data we want to extract. The realization of this became my obsession and I had to tackle every part of the technical process with the entire goal in mind.

## The solution

In order to provide a thoroughly comprehensive overview of the solution. I believe a merely theoretical discussion wouldn't convey the insight a more technically oriented reader would appreciate. Therefore in this section I'll combine discussions about the idea as well as the technical aspects.  

The idea proposed in the previous section involves processes that can be distinctly separated. Implementing the idea started by designing the processes involved as components can be defined in terms of input output and connecting those components in order to design the entire information pipeline. On a high level these were the processes involved that I could identify, 

 1. Turn a photograph into data which a computer can process
 2. Transform the data into a representational model of the original photo
 3. Interpret the model and generate desired output

On a practical level these categories have high-level subcategories that are substantially notable. Those can essentially be separated into the following

 - Turn a photograph into data which a computer can process
 -- Computer vision / Image manipulation
 -- Optical character recognition
 - Transform the data into a representational model of the original photo
 -- Text analysis
 -- Inferential analysis
 - Interpret the model and generate desired output
 -- Language theory

The solution will be discussed in the sequence presented above. 

### Turn a photograph into data which a computer can process

It is kind of amazing that the notion of extracting text from images has been a prevalent topic for the past 30+ years yet it can be frustratingly difficult to find information related to techniques and available tools. The places I crawled among my path through the information jungle included a lot of filtering ad-hoc from actual knowledgeable advice, digging into forum posts from years ago, websites that looks like relics of the past 90's era internet. Ultimately however, with enough trial and error, progress.

Through trial and error, the eventual process is summarized in the following list,

 1. Process the input image and optimize for OCR quality
 -- Essentially a process of input normalization
 -- Eliminates transient noise 
 2. Identify cohesive sections of information in the image
 -- Split each cohesive sections into individual images
 -- Achieved feature analysis of a filter that blends together nearby text 
 3. Perform OCR on each section in parallel
 -- Split results by subsections of the input image
 -- Maintain positional coordinate information within each result
 4. Combine the OCR results into one result set

The following figure presents the sequence from input on the left to output on the right,

![Photohraph to computer data process](https://i.imgur.com/KWXyC2L.png)

A lot of the choices made for this component of the solution were motivated by technical necessity. Performing optical character recognition on an image is a computationally heavy operation. The feature analysis part of the process was added in order to optimize for performance, namely, it allows us to perform OCR on sections of the image in parallel which provides better throughput. 

### Transform the data into a representational model of the original photo

***Intro***

In order to be able to parse the textual data generated from the previous OCR process we'll need to transform the currently disjunct result sets into a cohesive data structure. The basic idea from the start was to *map* the data thus far onto a grid that would represent the original document in terms of placement of elements. With such a data structure we could easily reason and build algorithms analogous to our own mode of thinking when extracting information.

The following figure describes a rough summary of the eventual process,
![Rough summary](https://i.imgur.com/oDvpCi0.png)

Each step will be discussed within its own following subsection.

***Text classification***

The transformation process starts by classifying the text contained in the OCR results. The set of possible classifications is arbitrary and depends on configuration. The text classifier component of the solution uses regular expressions primarily. For example, when it encounters a piece of text like "abc 123.0" it will extract the entire text with and without the numbers, the numbers [ "123", "0" ] and the decimal separated number as an amount [ "123.0" ]. 

***Mapping data onto a grid***

After classification, we construct a three dimensional grid in which the X and Y axis represents the corresponding two dimensions of the photograph of the document. The length of the Z axis is dynamic and contains every classification identified in that part of the document. Let's say we have a label in the document "Total amount" and an amount value below it, ideally, we want the text label to be placed in the grid exactly above the cell containing the total amount so that we can define intuitive patterns to represent and query that piece of information. 

This proved to be a more difficult problem to solve than initially thought and required a lot of trial and error. The biggest problem that is that real world documents don't really place elements according to a predefined grid. Furthermore, documents don't seem to care so much about precise pixel coordinates either, so a label and a value might be offset by a tiny number of pixels. How do we then map the data onto a grid which can be sensibly evaluated? 

***Normalization algorithm***

This required the design and implementation of a *normalization algorithm* that takes the input data and finds out a good grid approximation derived from all of the element positions. 

The normalization process involves selecting the set of all pixel indices of an axis, rounding each index by an arbitrary constant, then selecting the distinct resulting integers. The size of the resulting set will denote the length of that axis in the model, then we use the set to infer the closest X or Y model coordinate by finding the closest actual positional value from the set of rounded indices.

The following pseudo-code presents the algorithm that takes care of finding where to place elements in the model grid using the actual index coordinate values.

     let rc: integer rounding constant
     let si: set of all indices of an axis
     let sr: distinct set of all indices in si rounded down by rc

	 func findModelCoordinate(bx: axis coordinate of OCR result bounding box):
	     for i in si:
			 if si[i] > bx:
				 if ABS(si[i] - bx) > ABS(si[i - 1] - bx): 
					 return i - 1
				 else: 
					 return i
				 
		 return i - 1
     
     
This algorithm doesn't yield the most optimal results due to the rounding mechanism involved. For instance, let's assume a rounding constant of 15, two elements that represent cohesive information in a document together are placed at X coordinates 14 and 16 with some difference in the Y axis. One element would be rounded down to 0 and the other to 15, leaving what was supposed to be cohesive offset from each other in the final result.

***Trimming algorithm***

Due to the nature of the previous algorithm, cells which ideally should exist on the same X coordinate might end up offset from each other due to the rounding. To compensate for that error, a component implementing a *trimming* algorithm follows. The trimming algorithm scans the model three columns and three rows at a time, comparing the left and the right columns with the middle one and determining whether the cells have been erroneously offset due to the rounding. The algorithm determines that by comparing the absolute distance between the actual coordinates between the two and if they fall shorter than an arbitrary constant, the algorithm moves the cell to the appropriate position.

![Trimming](https://i.imgur.com/fLrKmos.png)

After trimming, we have a grid in which element placements are both intuitive to us and extremely easy to interact with via traversal algorithms with the intention to extract information.

### Interpret the model and generate desired output

The model generated by the process of the previous section represents a document as a three dimensional matrix where the X and Y axis correspond to approximate positional values where careful adjustments have been made that it can be intuitively understood by an interpreter. Down the Z axis we find possible interpretations and specific extracted data based on the text that was present in the corresponding cell.

The basis of the original idea was to be able to specify in terms of simple intuitive patterns what data we want to extract from a document. The reasoning for that being that even information that is difficult to determine in isolation by pure inference alone tend to follow patterns which we are familiar to intuitively. Replicating that intuitive process with an algorithm would allow for the complicated inferential analysis that would facilitate data extraction.

This part of the problem was solved by implementing a *custom query language* that directs the interpretation of the model representation. I could find no existing languages that are oriented towards interpretation of documents in such a manner so it seems to be a novel approach to the problem. 

The interpreter implementation utilizes a component called a *PageTraverser* which is an iterator for the matrix representation of the document. The *PageTraverser* is extremely simple in its design, it can be instantiated on any X and Y coordinate in the matrix. It exposes methods to traverse in four directions, up, down, left and right. When asked to traverse in a direction, the *PageTraverser* will move into the specified direction until it lands on a cell in the matrix which contains data. If it goes out of bounds it will report back to the interpreter that an error occurred.

Using the language to match a pattern in a document is intended to be very intuitive and flexible, let's say we want to find the receiver name for an invoice. We can do that by specifying the following pattern which represents a great deal of invoice formats,

    ReceiverName: [Text] Down Address Down PostalCode Town Down Country

In the example above the *"ReceiverName"* is tokenized as a *Pattern*, what follows is the definition of the pattern which in the example above consists of a *Capture* token indicated by the brackets, *TextType* tokens and *Direction* tokens.

The capture token is a special one and encapsulates a *TextType* token of its own. It denotes in this case that what we're interested in capturing is the name of the invoice receiver. When an entire pattern of the above definition can be matched in a document, we get back the value from the cell that we specified in the [Text] token.

The language itself is far more versatile and we could even have captured the entirety receiver information structure in one pattern,

    Receiver: 'Name': [Text] Down 'Address': Address Down 'PostalCode': PostalCode 'Town': Town Down 'Country': Country

Which instead of just yielding a key-value response would return an entire object construct with the properties specified in the pattern and associated values.

The interpreter will try executing the pattern on each of its cells until a successful pattern execution is yielded. Upon which the interpretation stops and the value captured from the [Text] token in the pattern is returned associated with the "ReceiverName" key. Pseudo-code for the interpreter follows,

	visitPattern:
	    for every x coordinate in matrix:
		    for every y coordinate in matrix:
			    let traverser: initialize a page traverser at (x, y)
			    let result: visit next token in pattern

				if result: successful
					return data contained in captured cell
				else
					continue
	
The visit token implementations themselves are incredibly simple as well, pseudo-code for the text type token visitor follows,

	visitTextType: 
		let valid: is traverser on a cell which contains the same text type as the current token?
		let matchesText: if specified, does the actual cell contents match the specified token value?

		if valid && matchesText:
			cast error back to visitPattern
		else
			visit next token
			
And finally the direction token is executed as follows,

	visitDirection:
		let result: request traversal in the specified direction
		
		if result: failed
			cast error back to visitPattern
		else
			visit next token

Visual example follows,
![Interpreter](https://i.imgur.com/d4eAgQS.png)

The language proved to be simple yet incredibly versatile in practice. It includes a good deal more operations than discussed here such as filtering by fuzzy value matching, specifying a limiting subset of the matrix, logical OR between text types, multiple prioritized (top to bottom) pattern definitions and a special *"Right-Down"* search algorithm implemented directly in the language. 

***Additional components***

In the context of the implemented processes, a fun additional experiment was to evaluate whether identifying structures such as tables and automatic extraction of data from them would be viable. That is, can we define a table by telling the interpreter what text is contained in column labels and what type of data is contained in the corresponding rows. The resulting component called *TableAnalyzer* can be directly interacted with in the query language with pattern definitions such as the following,

	Items: 
		Table 
		'ItemNo': [Text(ItemNumber)] 
		'Description': [Text(Description)] 
		'Quantity': [Number(Quantity)] 
		'UnitPrice': [AmountOrNumber(Unit price)] 
		'VatPercentage': [Percentage(VAT)] 
		'Total': [AmountOrNumber(Total)];

Note the table token at the start of the pattern indicating to the parser that whatever follows is a definition of a table's columns to identify. The table analysis algorithm won't be discussed in any further detail in this article however the simplicity of the implementation and the data extraction capabilities of the component are incredible.

Finally, DocumentLab outputs results in Json format whose structure depends on what the input patterns look like. I.e., either as a set of key-value objects or as a set of more complex objects for more complex patterns. This allows easy external integration with DocumentLab for any system that knows how to parse Json.

## Evaluation

Running some images through DocumentLab with a set of queries to extract the desired information is relatively easy enough. The following table presents the results of one such experiment,

![BoringEvaluation](https://i.imgur.com/iehS5Eo.png)

It gets the imagination going a bit more to see how DocumentLab performs on actual *real world* data. Well, data that looks *real world* more like since the image in the following example was created in Photoshop and doesn't contain any real information.

Let's say we're in the situation where we want to extract a variety of information from the following invoice.

![TheExample](https://i.imgur.com/BOQur27.png)

We create the following set of patterns,

![Patterns](https://i.imgur.com/OTTGEF1.png)

When the invoice is evaluated by DocumentLab, the following image showcases the resulting JSON, split at some points for presentational purposes,

![Invoice](https://i.imgur.com/ahAN27D.png)

There is nothing unique or provides any additional help for DocumentLab contained within the fake invoice created for the purpose of this evaluation presentation. The choice of input document is arbitrary and should yield the same result given that the input is of comparable quality.

## Final thoughts

I don't know if I'll obsess over this project again in such a way that I have previously. For now I'd like to sit down one day and continue cleaning up the github repository, the interface to the DocumentLab library and providing it in a package that is easy to integrate with. 

For now, you can check out the [github repository](https://github.com/karisigurd4/DocumentLab) and hopefully the provided documentation is comprehensive engough to get you started,
* Query language is designed for flexibility, using the prioritization of pattern executions or the logical or operators you can encompass a great deal of different document formats in one query, see [query langugage documentation](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md)
* DocumentLab is extensively configurable on all levels, see [configuration documentation](https://github.com/karisigurd4/DocumentLab/blob/master/Documentation/QueryLanguage.md)
  * Optimize performance according to your system requirements
  * Configure the text analyzer to match your unique data


