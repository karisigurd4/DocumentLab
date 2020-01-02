namespace DocumentLab
{
  using System;
  using System.Drawing;
  using Contracts;
  using Interfaces;

  /// <summary>
  /// A component that exposes the DocumentLab fluent query interface that provides an alternative to the DocumentLab scripting language.
  /// </summary>
  public class Document : DocumentLabBase, IDocument, IDisposable
  {
    private readonly Bitmap documentBitmap;
    private readonly Page page;

    /// <summary>
    /// Initialize a Document component and assign a bitmap of the document to analyze. 
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
      return new FluentQuery(this.page, this.documentInterpreter) 
      { 
        Script = FluentQueryConstants.GeneratedScriptQuery + ":"
      };
    }

    public new void Dispose()
    {
      base.Dispose();
      documentBitmap.Dispose();
    }
  }
}
