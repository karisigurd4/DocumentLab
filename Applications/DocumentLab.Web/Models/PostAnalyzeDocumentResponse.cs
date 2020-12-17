namespace DocumentLab.Web.Models
{
  using DocumentLab.Contracts;
  using System;
  
  public enum PostAnalyzeDocumentResult
  {
    Success,
    Failed_InternalError
  }
  
  public class PostAnalyzeDocumentResponse
  {
    public Page AnalyzedDocument { get; set; }
    public PostAnalyzeDocumentResult Result { get; set; }
    public string Message { get; set; }
  }
}