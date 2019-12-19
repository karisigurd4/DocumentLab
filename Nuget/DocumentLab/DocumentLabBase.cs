namespace DocumentLab
{
  using Castle.Facilities.TypedFactory;
  using Castle.Windsor;
  using Castle.Windsor.Installer;
  using global::DocumentLab.ImageProcessor.Interfaces;
  using global::DocumentLab.Ocr.Interfaces;
  using global::DocumentLab.PageAnalyzer.Interfaces;
  using global::DocumentLab.PageInterpreter.Components;
  using global::DocumentLab.PageInterpreter.Interfaces;

  public class DocumentLabBase
  {
    protected readonly IOcr ocr;
    protected readonly IPageAnalyzer pageAnalyzer;
    protected readonly IPageTrimmer pageTrimmer;
    protected readonly IInterpreter interpreter;
    protected readonly IImageProcessor imageProcessor;
    protected readonly IWindsorContainer windsorContainer;

    public DocumentLabBase()
    {
      this.windsorContainer = new WindsorContainer();
      this.windsorContainer.Install(FromAssembly.This());
      this.windsorContainer.AddFacility<TypedFactoryFacility>();
      this.windsorContainer.Install(FromAssembly.Named("DocumentLab.Core"));
      this.windsorContainer.Install(FromAssembly.Named("DocumentLab.ImageProcessor"));
      this.windsorContainer.Install(FromAssembly.Named("DocumentLab.TextAnalyzer"));
      this.windsorContainer.Install(FromAssembly.Named("DocumentLab.PageAnalyzer"));
      this.windsorContainer.Install(FromAssembly.Named("DocumentLab.Ocr"));

      this.ocr = windsorContainer.Resolve<IOcr>();
      this.imageProcessor = windsorContainer.Resolve<IImageProcessor>();
      this.pageAnalyzer = windsorContainer.Resolve<IPageAnalyzer>();
      this.pageTrimmer = windsorContainer.Resolve<IPageTrimmer>();
      this.interpreter = new Interpreter();
    }
  }
}
