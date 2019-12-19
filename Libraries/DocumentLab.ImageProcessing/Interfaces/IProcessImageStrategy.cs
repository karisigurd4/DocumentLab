namespace DocumentLab.ImageProcessor.Interfaces
{
  using System.Collections.Generic;
  using System.Drawing;

  public interface IProcessImageStrategy
  {
    IEnumerable<byte> PreProcess(IEnumerable<byte> bitmap); 
  }
}
