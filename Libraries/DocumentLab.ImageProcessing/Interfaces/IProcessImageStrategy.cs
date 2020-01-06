namespace DocumentLab.ImageProcessor.Interfaces
{
  using System.Drawing;
  using System.Collections.Generic;

  public interface IProcessImageStrategy
  {
    IEnumerable<byte> PreProcess(IEnumerable<byte> bitmap); 
  }
}
