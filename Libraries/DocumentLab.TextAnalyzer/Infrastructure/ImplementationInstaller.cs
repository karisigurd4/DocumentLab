namespace DocumentLab.TextAnalyzer.Infrastructure
{
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
  using Castle.Windsor;
  using DocumentLab.TextAnalyzer.Interfaces;
  using DocumentLab.TextAnalyzer.Implementation;
  using Castle.MicroKernel.Resolvers.SpecializedResolvers;

  public class ImplementationInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container.Register(
        Component.For<ITextAnalyzer>()
          .ImplementedBy<TextAnalyzer>()
          .LifeStyle
          .Singleton,
        Classes.FromThisAssembly()
        .BasedOn<IAnalyzeTextStrategy>()
        .WithService.FromInterface()
      );
      container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
    }
  }
}
