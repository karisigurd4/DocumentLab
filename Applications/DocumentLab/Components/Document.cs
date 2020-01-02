namespace DocumentLab
{
  using global::DocumentLab.Contracts;
  using global::DocumentLab.Interfaces;
  using System;
  using System.Drawing;

  /// <summary>
  /// A component that exposes the DocumentLab fluent query interface that provides an alternative to the DocumentLab scripting language.
  /// </summary>
  public class Document : DocumentLabBase, IDocument, IDisposable
  {
    private readonly Bitmap documentBitmap;
    private readonly Page page;

    /// <summary>
    /// Initialize a Document component and assign a bitmap of the document to analyze. This component initializes a new instance of DocumentLab and keeps the bitmap in memory until it is explicitly disposed. For fast-multi file processing the initalization of a new DocumentLab instance for every document creates considerable overhead that should not be neglected.
    /// </summary>
    /// <param name="documentBitmap">A Bitmap of the document to analyze</param>
    public Document(Bitmap documentBitmap)
    {
      this.documentBitmap = documentBitmap;
      this.page = GetAnalyzedPage(documentBitmap);
    }

    /// <summary>
    /// Initializes a query to perform on the document. The fluent query api allows you to build up DocumentLab patterns via its method interface.
    /// 
    /// In order to execute the pattern on the document the .Execute() method must be called.
    /// </summary>
    /// <returns>A new DocumentLab FluentQuery object with an implicit query name</returns>
    public FluentQuery Query()
    {
      return new FluentQuery()
      {
        Script = FluentQueryConstants.GeneratedScriptQuery + ":",
        AnalyzedPage = this.page,
        Interpreter = this.documentInterpreter
      };
    }

    public new void Dispose()
    {
      base.Dispose();
      documentBitmap.Dispose();
    }
  }
}
