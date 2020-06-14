
## DocumentLab

Back in 2017 I encountered a problem which for the subsequent 3 years I would obsess on and off over. It was one of those problems that had an air of "many have tried, none have succeeded" around it. Immediately my mind started spinning, trying to imagine possible methods for solving this problem. The more I thought about it, the more I wanted to go for it.

This was a personal research project for myself. I never knew what eventual goal I wanted to reach. I still don't, however at this time I believe the solution which I have created is sufficient enough to have a more high-level discussion about. Hopefully what I've learned can help others in a similar situation in the future or to provide useful insight for further research. 

##  The problem

I'll present the problem by giving a hypothetical example, the example will be limited to the domain of invoices however the example could just as well have picked any other domain in which information exchange on paper is applicable. Let's say we're building an app that allows us to keep track of our finances, one of the features of the app is that we should be able to take a picture of an invoice, let the app scan the information contained in the invoice and then store the information in a database in order to give us statistics about what we're paying for. Maybe we want to track increase or decrease in price from a given sender for some service. 

Sounds simple enough right? 

Well here comes the tricky part.

How can we teach a computer to scan invoice contents in such an intelligent way that it can organize the information automatically for us? Keep in mind that all we have to work with is an image of an invoice which contains no additional information other than pixel information. To begin with, we'll need some method for extracting text from an image. That's where OCR or Optical Character Recognition comes in. What if the photograph was taken in sub-optimal lighting conditions, or at a slight angle? That's where computer vision and image manipulation techniques can be used.

Using the techniques mentioned above a common solution to our problem has historically been to define pixel boundaries where information on invoices commonly can be found. There exists an entire industry around providing solutions that do exactly that from very specific to very general cases. But taking that approach to solving the problem has some very unfortunate drawbacks. The structure of an invoice varies so incredibly between origin that even if we know that an invoice from sender A has some piece of information at some pixel coordinates x and y there's no guarantee that an invoice from sender B will have that piece of info anywhere close by. How do we solve that? We would need to keep a database of every possible invoice structure to refer to when we want to scan information from them. 

## The idea

Given how incredibly varied the structure of an invoice can be, how do I even interpret and extract the information I need from an invoice? This was my initial thought when I heard about this problem. But it seems relatively easy to conceptualize, while invoices might not have exact recurring placement of information there are patterns at play which allow me to identify what I'm looking at. Sometimes these patterns are simple and even give hints such as with a label "Total amount" and an amount next to it. More complex patterns, such as the invoice receiver information can be inferred by the knowledge that it follows a variation of a commonly accepted pattern such as Receiver's Name -> Address -> Postal Code -> Town  -> Country.

I was interested in modeling this process and evaluating how the output would perform. My hypothesis was that if a model was sufficiently robust, all we would need to do was to define what kind of patterns to look for. An algorithm could parse the model with the patterns as input and the output should be the actual information contained within the document. Given that documents across a whole range of domains have patterns which we can intuitively interpret and use to extract information, I had a hunch that such a method could be generalized across all those domains.

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
 -- State machines
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

![Photohraph to computer data process](https://lh3.googleusercontent.com/1gxu9im-XCqSF2Sn8S7X5qOOcCicSUaZmMJQNqrMQ8_6zGyYNPgt-DLK2XyMcnjWVGjXXeEuGCNHJoPnxbCWFR95Y3AkPddDiBTZISqy-8ivV7L93mgBGzpzHfUuIopN6eToCG3jGuZhW3Q80OFlTq_1EZj8_sYGwTutvLbaoK8y69GRK4EItLBv6A6MdLamFr0hCwdqfUrgCESRcEVPEh64Kh_odZVAr1oR3v1lkW93fm8KuU-T91lQs3RRIkQOeKvHvoS_83TSrPi7GB83RXuxWGm3Kc-gxMqyi2Lgy9mTxqIrtU2yeQ50A_yk5hZigNDs8IFTCS-QCpQ2WMVSMOJPO9fUkJqQtGhQBEGbP6Grnbjav-iVPmzLfdrTiozA-7xrbFvJZscx2tJHTvT_Wvz4x3Rf_xFjNR1GIqLGQ__8_euTpYqXdU6VSZG_tdVu9Cf9sOU7KFYzACThBwXssUjwc4OmlGCtqb4m15XJcKtaQx2l9zrjiC3ayHqqBvcxYeCDJWqdmTrX2G64S6cOWGwoDPasKKwSFLnPw8Yv1bQeUn_-Kq75BYvaSHKfB-b_R8O9d5R0ZFamBrs-Zujhj7ULCf8dYOn7sByi1EpLaHygkL0mSY7PcKi-gwoYlASS7LIHIRLBTo0HCpADx_5QjDqThWH0uVgXVztb4lN1q2bafy3pNyr71-7WfX6QDg=w841-h126-no?authuser=0)

A lot of the choices made for this component of the solution were motivated by technical necessity. Performing optical character recognition on an image is a computationally heavy operation. The feature analysis part of the process was added in order to optimize for performance, namely, it allows us to perform OCR on sections of the image in parallel which provides better throughput. 

### Transform the data into a representational model of the original photo

By this point we have a set of data which consists of the OCR result for each subsection and their associated positional coordinates. The primary requirement I had for the model in which to transform these results into was that it should be easy to intuitively understand and work with. 

The solution I created for this purpose starts by classifying the text contained in the OCR results. The set of possible classifications is arbitrary and depends on configuration. The text classifier component of the solution uses regular expressions. 


![The model construction process](https://lh3.googleusercontent.com/M6xcppjQnB7GI4huLorYaipA9m7Wna5yZ-yAXppxDm_uw9fjrS6BBqHblXLhFGuUejgcmaPcsFMZHOpe0y2soJ9cVugrLJrVrWSw3kWFOLB83dL4MBBT_27G-kf9wRufEnTsJ1jATRaE6204lyhbb-rDwsKZAdyHjFMZO20m4RseLvBHg7j_oc4DIlQZjYVXHWWgnlx_5uYvo-mdY3dG4Ti7tnSDi9rpdrtE4A7c8gIL2Hd9CIacmm19KDQyVfYPl5_iDBkQwFjjU2JEgOssmoB2OYOfQcgmfXsmJZDOW2YCIllxu_7btfUWx4oN7hEkTOgEgkV9XprhYqGsVQfvkTJCFYSzFkW_15s9mt2mzC0kIsn7tBaLTmyQkVpvhJhBj3FkSr_0SdDYqSsN0FhYMsyz-I8YGWRa42rogbUk9HdoyT5V3TrG6rncJUVK-xzC77aelciiYhhwl3ZBosQjHpP62vqnk5xewsFAf4mujp3WlZa4M02Ns7ml1TSlevq4rWrbEE992CZhcpuV-9LISx4D7jmo_F9BZ5_-7wgz8_0u2edJTU1zgyed6M5p0c79jnRXGl61uxcJYIF9PDrF4Y4eDtjI6RMsndsdigtNq2irVDkZbYBFDAmTI4x_QYKj07pySGykwRXjKBfp9LDLscQLsrX9PNEIrUJZeL5ELjEnMNBw1V4CfbsTNpltlQ=w786-h94-no?authuser=0)
