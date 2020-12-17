namespace DocumentLab.Web.Controllers
{
  using DocumenLab;
  using DocumentLab.Web.Models;
  using PdfiumViewer;
  using System;
  using System.Drawing;
  using System.IO;
  using System.Text.RegularExpressions;
  using System.Web.Http;
  using System.Web.Http.Cors;

  [RoutePrefix("api/document")]
  public class DocumentController : ApiController
  {
    private readonly DocumentInterpreter interpreter;

    public DocumentController()
    {
      interpreter = new DocumentInterpreter();
    }

    [Route("")]
    [HttpGet]
    public string GetRoot()
    {
      return "Hello";
    }

    [Route("interpret")]
    [HttpPost]
    public PostInterpretDocumentResponse PostInterpretDocument(PostInterpretDocumentRequest request)
    {
      try
      {
        return new PostInterpretDocumentResponse()
        {
          InterpretedDocument = interpreter.InterpretToJson(request.AnalyzedDocument, request.Script),
          Message = string.Empty,
          Result = PostInterpretDocumentResult.Success
        };
      }
      catch (Exception e)
      {
        return new PostInterpretDocumentResponse()
        {
          Message = e.Message,
          Result = PostInterpretDocumentResult.Failed_InternalError
        };
      }
    }

    [Route("analyze")]
    [HttpPost]
    public PostAnalyzeDocumentResponse PostAnalyzeDocument(PostAnalyzeDocumentRequest request)
    {
      try
      {
        var replaceRegex = "\\w+\\:\\w+\\/(\\w+)\\;base64\\,";
        var fileTypeRegex = "(?!<=\\w+\\:\\w+\\/)(\\w+)(?=\\;base64\\,)";
        var fileType = Regex.Match(request.ImageAsBase64, fileTypeRegex).Value;

        var base64 = request.ImageAsBase64.Replace(Regex.Match(request.ImageAsBase64, replaceRegex).Value, "");

        using (var ms = new MemoryStream(Convert.FromBase64String(base64)))
        {

          Bitmap bitmap = null;
          switch (fileType)
          {
            case "png": bitmap = (Bitmap)Image.FromStream(ms); break;
            case "pdf": bitmap = (Bitmap)PdfDocument.Load(ms).Render(0, 300, 300, PdfRenderFlags.CorrectFromDpi); break;
          }

          return new PostAnalyzeDocumentResponse()
          {
            AnalyzedDocument = interpreter.AnalyzePage(bitmap),
            Message = string.Empty,
            Result = PostAnalyzeDocumentResult.Success
          };
        }
      }
      catch (System.Exception e)
      {
        return new PostAnalyzeDocumentResponse()
        {
          Message = e.Message,
          Result = PostAnalyzeDocumentResult.Failed_InternalError
        };
      }
    }
  }
}