namespace DocumentLab.TextAnalyzer.Infrastructure
{
  using TextAnalyzer.Interfaces;
  using TextAnalyzer.Implementation;
  using Castle.Windsor;
  using Castle.MicroKernel.Registration;
  using Castle.MicroKernel.SubSystems.Configuration;
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
