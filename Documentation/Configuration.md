# Configuration 

DocumentLab provides a number of configuration parameters, some more interesting than others. This section explains the most interesting parameters that are configurable.

## Host system resource optimization

The actual OCR process in DocumentLab is optimized for parallel processing, the following diagram represents the conceptual implementation of the load-balancing and parallelization. It's not necessarily a 1-1 representation of the implementation but it suffices to show the steps taken to distribute the load appropriately,

![TesseractPoolDiagram](https://raw.githubusercontent.com/karisigurd4/DocumentLab/master/Documentation/TesseractPoolDiagram.png)

In *Data\Configuration\OCR Configuration.json* we have a number of configuration parameters. The intent of which allow us to optimize OCR processing, namely, Tesseract specifically to our host system hardware .

* NumberOfThreads - Maximum number of parallel threads we'll allow DocumentLab to instantiate
  * This is independent of how many engines we have available, i.e., even if an engine becomes available we can't assign a new OCR job to it if we're exceeding maximum thread count
  * When adjusting this parameter, pay attention to how many cores/threads your host CPU provides
  * Setting this one too high can bottleneck your system, setting it too low will limit otherwise available excess computational power
* TesseractEnginePoolSize - How many Tesseract *engines* we want our threads to have access to
  * The *TesseractPool* in the diagram represents how OCR jobs are distributed between *Tesseract Engines*. This parameter sets the number of Tesseract Engines the TesseractPool has at its disposal when distributing jobs
  * Ideally, this parameter should be around half or slightly less than half of the maximum number of threads you allow *(according to my not-so scientific tests)*
  * Setting this one too high wastes memory but has little impact on overall performance, setting it too low will bottleneck your threads
* ResultChunkSize 
  * Each OCR *job* assigned to a tesseract engine includes a set of images, referred to as *chunks*. These chunks of images are processed sequentially by each TesseractEngine job
  * The reason for processing more than one image in a job sequentially is that it can offload the context switching/thread management overhead a bit
  * The deafault setting is to chunk together two images, playing with this parameter may yield different results on different hardware

## Language configuration

## Image analysis optimization

## Text analysis optimization
