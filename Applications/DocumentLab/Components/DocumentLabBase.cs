namespace DocumentLab
{
  using System;
  using System.Drawing;
  using Castle.Windsor;
  using Castle.Windsor.Installer;
  using Contracts;
  using Contracts.Enums.Operations;
  using ImageProcessor.Extensions;
  using ImageProcessor.Interfaces;
  using PageAnalyzer.Interfaces;
  using Ocr.Interfaces;
  using PageInterpreter.Components;
  using PageInterpreter.Interfaces;

  public class DocumentLabBase : IDisposable
  {
    private readonly IOcr ocr;
    private readonly IPageAnalyzer pageAnalyzer;
    private readonly IPageTrimmer pageTrimmer;
    private readonly IImageProcessor imageProcessor;
    private readonly IWindsorContainer windsorContainer;
    protected readonly IInterpreter documentInterpreter;

    public DocumentLabBase()
    {
      this.windsorContainer = new WindsorContainer();
      this.windsorContainer.Install(FromAssembly.This());

      this.ocr = windsorContainer.Resolve<IOcr>();
      this.imageProcessor = windsorContainer.Resolve<IImageProcessor>();
      this.pageAnalyzer = windsorContainer.Resolve<IPageAnalyzer>();
      this.pageTrimmer = windsorContainer.Resolve<IPageTrimmer>();
      this.documentInterpreter = new Interpreter();
    }

    public Page GetAnalyzedPage(Bitmap documentBitmap)
    {
      var downscaledDocumentBitmap = imageProcessor.Process
      (
        ProcessImageOperation.Downsample, 
        documentBitmap.ToByteArray(Ocr.Constants.ConvertBetween)
      );

      var ocrResults = ocr.PerformOcr(documentBitmap, downscaledDocumentBitmap);
      var analyzedPage = pageAnalyzer.PerformPageAnalysis(ocrResults);
      var trimmedPage = pageTrimmer.TrimPage(analyzedPage);

      return trimmedPage;
    }

    public void Dispose()
    {
      windsorContainer.Release(ocr);
      windsorContainer.Release(pageAnalyzer);
      windsorContainer.Release(pageTrimmer);
      windsorContainer.Release(documentInterpreter);
      windsorContainer.Release(imageProcessor);
      windsorContainer.Dispose();
    }
  }
}
