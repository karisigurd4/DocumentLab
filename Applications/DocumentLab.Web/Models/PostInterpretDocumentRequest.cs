using DocumentLab.Contracts;

namespace DocumentLab.Web.Models
{
  public class PostInterpretDocumentRequest
  {
    /// <summary>
    /// Analyzed document retrieved from [Post] /analyze
    /// </summary>
    public Page AnalyzedDocument { get; set; }

    /// <summary>
    /// DocumentLab query language script to use when interpreting the document
    /// </summary>
    public string Script { get; set; }
  }
}