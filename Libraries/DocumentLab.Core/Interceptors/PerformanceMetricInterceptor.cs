namespace DocumentLab.Core.Interceptors
{
  using Castle.DynamicProxy;
  using System.Diagnostics;

  public class PerformanceMetricInterceptor : IInterceptor
  {
    public void Intercept(IInvocation invocation)
    {
      Debug.WriteLine($"Started measuring performance of {invocation.TargetType.Name}.{invocation.Method.Name}");
      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();
      invocation.Proceed();
      stopWatch.Stop();
      Debug.WriteLine($"Measured performance of {invocation.TargetType.Name}.{invocation.Method.Name}, took {stopWatch.ElapsedMilliseconds} milliseconds");
    }
  }
}
