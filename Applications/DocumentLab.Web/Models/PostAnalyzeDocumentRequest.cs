namespace DocumentLab.Web.Models
{
  public class PostAnalyzeDocumentRequest
  {
    /// <summary>
    /// The image, serialized as base64 to interpret using the associated provided script
    /// </summary>
    public string ImageAsBase64 { get; set; }
  }
}