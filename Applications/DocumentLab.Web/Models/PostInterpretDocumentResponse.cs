namespace DocumentLab.Web.Models
{
  public enum PostInterpretDocumentResult
  {
    Success,
    Fault_BadRequest,
    Failed_InternalError
  }

  public class PostInterpretDocumentResponse
  {
    public string InterpretedDocument { get; set; }
    public PostInterpretDocumentResult Result { get; set; }
    public string Message { get; set; }
  }
}