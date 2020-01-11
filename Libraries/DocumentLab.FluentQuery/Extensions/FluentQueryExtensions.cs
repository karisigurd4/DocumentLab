namespace DocumentLab
{
  using PageInterpreter;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using System;
  using System.Linq;
  using System.Collections.Generic;

  public static class FluentQueryExtensions
  {
    internal static Dictionary<QueryType, QueryType> queryTypeProgression = new Dictionary<QueryType, QueryType>()
    {
      { QueryType.None, QueryType.SingleCapture },
      { QueryType.SingleCapture, QueryType.MultiCapture },
      { QueryType.MultiCapture, QueryType.MultiCapture },
      { QueryType.Any, QueryType.Any }
    };

    internal static FluentQuery AppendToScript(this FluentQuery query, string operation)
    {
      query.Script += " " + operation;

      return query;
    }

    /// <summary>
    /// Following a pattern predicate or a capture, specify to move *Up* from there to look for the next element in the document for the next operation.
    /// </summary>
    /// <returns>A DocumentLab FluentQuery with a script extension that performs the traversal.</returns>
    public static FluentQuery Up(this FluentQuery response)
    {
      return response.AppendToScript("Up");
    }

    /// <summary>
    /// Following a pattern predicate or a capture, specify to move *Down* from there to look for the next element in the document for the next operation.
    /// </summary>
    /// <returns>A DocumentLab FluentQuery with a script extension that performs the traversal.</returns>
    public static FluentQuery Down(this FluentQuery response)
    {
      return response.AppendToScript("Down");
    }

    /// <summary>
    /// Following a pattern predicate or a capture, specify to move *Left* from there to look for the next element in the document for the next operation.
    /// </summary>
    /// <returns>A DocumentLab FluentQuery with a script extension that performs the traversal.</returns>
    public static FluentQuery Left(this FluentQuery response)
    {
      return response.AppendToScript("Left");
    }

    /// <summary>
    /// Following a pattern predicate or a capture, specify to move *Right* from there to look for the next element in the document for the next operation.
    /// </summary>
    /// <returns>A DocumentLab FluentQuery with a script extension that performs the traversal.</returns>
    public static FluentQuery Right(this FluentQuery response)
    {
      return response.AppendToScript("Right");
    }

    /// <summary>
    /// Adds a text type and optional text match predicate to the pattern we want to match in a document.
    /// </summary>
    /// <param name="textType">The text type we want to match in the pattern</param>
    /// <param name="matchText">*Optional* Adds that the text type match also needs to match the text specified. This works by checking if the string we're evaluating with from the document contains the text we specify here + a Levensthein distance 2 (by default) check. Therefore the text here can be an abbreviation of common terms or at least in some simplified form in order to make it more durable to differences that might occur in OCR results.</param>
    /// <returns>Returns a DocumentLab FluentQuery with a script extension that performs the match.</returns>
    public static FluentQuery Match(this FluentQuery response, TextType textType, params string[] matchText)
    {
      return response.AppendToScript(matchText.Count() > 0 ? $"{textType.ToString()}({string.Join("||", matchText)})" : textType.ToString());
    }

    /// <summary>
    /// Adds a text type and optional text match predicate to the pattern we want to match in a document.
    /// </summary>
    /// <param name="textType">The text type we want to match in the pattern</param>
    /// <param name="matchText">*Optional* Adds that the text type match also needs to match the text specified. This works by checking if the string we're evaluating with from the document contains the text we specify here + a Levensthein distance 2 (by default) check. Therefore the text here can be an abbreviation of common terms or at least in some simplified form in order to make it more durable to differences that might occur in OCR results.</param>
    /// <returns>Returns a DocumentLab FluentQuery with a script extension that performs the match.</returns>
    public static FluentQuery Match(this FluentQuery response, string textType, params string[] matchText)
    {
      return response.AppendToScript(matchText.Count() > 0 ? $"{textType.ToString()}({string.Join("||", matchText)})" : textType.ToString());
    }

    /// <summary>
    /// At the end of a query, calling this method performs a capture operation on the document using the specified text type as a match predicate. The value returned is the value that corresponds to the pattern on the document.
    /// </summary>
    /// <param name="captureTextType">The text type predicate the capture must match for a result to be considered valid</param>
    /// <returns>Value extracted from document.</returns>
    public static string Capture(this FluentQuery response, TextType captureTextType)
    {
      response.QueryType = queryTypeProgression[response.QueryType];

      if (response.QueryType != QueryType.SingleCapture)
      {
        throw new FluentQueryException("The specified pattern has another capture already. CaptureSingle executes the current query but can only return a single value.");
      }

      response.AppendToScript($"[{captureTextType.ToString()}]");

      return ExecuteSingleCapture(response);
    }

    /// <summary>
    /// At the end of a query, calling this method performs a capture operation on the document using the specified text type as a match predicate. The value returned is the value that corresponds to the pattern on the document.
    /// </summary>
    /// <param name="captureTextType">The text type predicate the capture must match for a result to be considered valid</param>
    /// <returns>Value extracted from document.</returns>
    public static string Capture(this FluentQuery response, string captureTextType)
    {
      response.QueryType = queryTypeProgression[response.QueryType];

      if (response.QueryType != QueryType.SingleCapture)
      {
        throw new FluentQueryException("The specified pattern has another capture already. CaptureSingle executes the current query but can only return a single value.");
      }

      response.AppendToScript($"[{captureTextType}]");

      return ExecuteSingleCapture(response);
    }

    /// <summary>
    /// Executes a multiple capture query
    /// </summary>
    /// <param name="fluentQuery">A FluentQuery object containing the script built so far.</param>
    /// <returns>The data specified for capture in the query in a dictionary.</returns>
    public static Dictionary<string, string> Capture(this FluentQuery fluentQuery, Func<FluentQuery, FluentQuery> query)
    {
      fluentQuery = query(fluentQuery);

      if (fluentQuery.QueryType != QueryType.MultiCapture)
      {
        throw new FluentQueryException("A multi capture query needs to have multiple captures specified");
      }

      return fluentQuery.ExecuteQuery();
    }

    /// <summary>
    /// Captures the value of the text matched in the document and includes it in the extracted output.
    /// </summary>
    /// <param name="captureTextType">The text type we want to capture in the document. In a pattern the text type specified in a pattern must yield a positive match in order for the capture to be valid.</param>
    /// <param name="propertyName">*Optional* Specify a name for the property associated with the capture. This is only applicable for multi-capture patterns.</param>
    /// <returns>Returns a DocumentLab FluentQuery with a script extension that performs the capture.</returns>
    public static FluentQuery Capture(this FluentQuery response, TextType captureTextType, string propertyName = "")
    {
      response.QueryType = QueryType.MultiCapture;

      if (string.IsNullOrWhiteSpace(propertyName))
      {
        throw new FluentQueryException("The specified pattern has multiple captures, a property name must be specified when capturing more than one value.");
      }

      return response.AppendToScript((!string.IsNullOrWhiteSpace(propertyName) ? $"'{propertyName}': " : string.Empty) + $"[{captureTextType.ToString()}]");
    }

    /// <summary>
    /// Captures the value of the text matched in the document and includes it in the extracted output.
    /// </summary>
    /// <param name="captureTextType">The text type we want to capture in the document. In a pattern the text type specified in a pattern must yield a positive match in order for the capture to be valid.</param>
    /// <param name="propertyName">*Optional* Specify a name for the property associated with the capture. This is only applicable for multi-capture patterns.</param>
    /// <returns>Returns a DocumentLab FluentQuery with a script extension that performs the capture.</returns>
    public static FluentQuery Capture(this FluentQuery response, string captureTextType, string propertyName)
    {
      response.QueryType = QueryType.MultiCapture;

      if (string.IsNullOrWhiteSpace(propertyName))
      {
        throw new FluentQueryException("The specified pattern has multiple captures, a property name must be specified when capturing more than one value.");
      }

      return response.AppendToScript((!string.IsNullOrWhiteSpace(propertyName) ? $"'{propertyName}': " : string.Empty) + $"[{captureTextType}]");
    }

    /// <summary>
    /// Performs a Right-Down search from the previous predcate. A predicate or capture should follow. This is implicitly already used in the FindValueForLabel and GetValueForLabel methods. Use this method if you want to create more detailed patterns using RD.
    /// </summary>
    /// <param name="maxSteps">The maximum distance in cells between the previous predicate and the next predicate for the pattern to be valid.</param>
    /// <returns></returns>
    public static FluentQuery RightDownSearch(this FluentQuery response, int maxSteps)
    {
      return response.AppendToScript($"RD {maxSteps}");
    }

    /// <summary>
    /// Finds a value by a label's text using the Right-Down search algorithm. The value closest to the label's text in the right or down direction in the document will be chosen as the result.
    /// </summary>
    /// <param name="labelInDocument">The label we expect to find in the document, valid matches can be separated by ||</param>
    /// <returns>Returns a DocumentLab FluentQuery with a script that performs the value extraction.</returns>
    public static string FindValueForLabel(this FluentQuery response, params string[] labelInDocument)
    {
      response.QueryType = QueryType.SingleCapture;
      response.AppendToScript($"Text({string.Join("||", labelInDocument)}) RD 6 [{TextType.Text}]");

      return ExecuteSingleCapture(response);
    }

    /// <summary>
    /// Finds a value by a label's text using the Right-Down search algorithm. The value closest to the label's text in the right or down direction in the document will be chosen as the result.
    /// </summary>
    /// <param name="labelInDocument">The label we expect to find in the document, valid matches can be separated by ||</param>
    /// <param name="textTypeOfValue">Specifies the text type the capture operation needs to match</param>
    /// <param name="maxSteps">The maximum distance in terms of DocumentLab grid cells the label-value in the document can be. This is by default 6 which is sufficient for close-by elements but can be made longer or shorter depending on the type of the document.</param>
    /// <returns>Returns a DocumentLab FluentQuery with a script that performs the value extraction.</returns>
    public static string FindValueForLabel(this FluentQuery response, TextType textTypeOfValue = TextType.Text, int maxSteps = 6, params string[] labelInDocument)
    {
      response.QueryType = QueryType.SingleCapture;
      response.AppendToScript($"Text({string.Join("||", labelInDocument)}) RD {maxSteps} [{textTypeOfValue.ToString()}]");

      return ExecuteSingleCapture(response);
    }

    /// <summary>
    /// Finds a value by a label's text using the Right-Down search algorithm. The value closest to the label's text in the right or down direction in the document will be chosen as the result.
    /// </summary>
    /// <param name="textTypeOfValue">Specifies the text type the capture operation needs to match</param>
    /// <param name="maxSteps">The maximum distance in terms of DocumentLab grid cells the label-value in the document can be. This is by default 6 which is sufficient for close-by elements but can be made longer or shorter depending on the type of the document.</param>
    /// <param name="labelInDocument">The label we expect to find in the document</param>
    /// <returns>Returns a DocumentLab FluentQuery with a script that performs the value extraction.</returns>
    public static string FindValueForLabel(this FluentQuery response, string textTypeOfValue = "Text", int maxSteps = 6, params string[] labelInDocument)
    {
      response.QueryType = QueryType.SingleCapture;
      response.AppendToScript($"Text({string.Join("||", labelInDocument)}) RD {maxSteps} [{textTypeOfValue}]");

      return ExecuteSingleCapture(response);
    }

    /// <summary>
    /// Gets value by label's text given that we know in which direction relative to the label the value is.
    /// </summary>
    /// <param name="labelInDocument">The label we expect to find in the document, valid matches can be separated by ||</param>
    /// <returns>Returns a DocumentLab FluentQuery with a script that performs the value extraction.</returns>
    public static string GetValueAtLabel(this FluentQuery response, params string[] labelInDocument)
    {
      response.QueryType = QueryType.SingleCapture;
      response.AppendToScript($"Text({string.Join("||", labelInDocument)}) {Direction.Right.ToString()} [{TextType.Text.ToString()}]");

      return ExecuteSingleCapture(response);
    }

    /// <summary>
    /// Gets value by label's text given that we know in which direction relative to the label the value is.
    /// </summary>
    /// <param name="labelInDocument">The label we expect to find in the document, valid matches can be separated by ||</param>
    /// <param name="textTypeOfValue">Specifies the text type the capture operation needs to match</param>
    /// <param name="direction">Direction relative to the label the value should be located at in the document</param>
    /// <returns>Returns a DocumentLab FluentQuery with a script that performs the value extraction.</returns>
    public static string GetValueAtLabel(this FluentQuery response, Direction direction, TextType textTypeOfValue = TextType.Text, params string[] labelInDocument)
    {
      response.QueryType = QueryType.SingleCapture;
      response.AppendToScript($"Text({string.Join("||", labelInDocument)}) {direction} [{textTypeOfValue.ToString()}]");

      return ExecuteSingleCapture(response);
    }

    /// <summary>
    /// Gets value by label's text given that we know in which direction relative to the label the value is.
    /// </summary>
    /// <param name="labelInDocument">The label we expect to find in the document, valid matches can be separated by ||</param>
    /// <param name="direction">Direction relative to the label the value should be located at in the document</param>
    /// <returns>Returns a DocumentLab FluentQuery with a script that performs the value extraction.</returns>
    public static string GetValueAtLabel(this FluentQuery response, Direction direction, string textTypeOfValue = "Text", params string[] labelInDocument)
    {
      response.QueryType = QueryType.SingleCapture;
      response.AppendToScript($"Text({string.Join("||", labelInDocument)}) {direction} [{textTypeOfValue}]");

      return ExecuteSingleCapture(response);
    }

    /// <summary>
    /// Gets all values of the specified text type in a document.
    /// </summary>
    /// <param name="textType">The text type to capture all instances of in a document.</param>
    /// <returns>A DocumentLab FluentQuery with a script extension that performs the Any operation.</returns>
    public static string[] GetAny(this FluentQuery response, TextType textType)
    {
      response.QueryType = QueryType.Any;
      response.AppendToScript($"Any [{textType.ToString()}]");

      return response.ExecuteQuery().Select(x => x.Value).ToArray();
    }

    /// <summary>
    /// Gets all values of the specified text type in a document.
    /// </summary>
    /// <param name="textType">The text type to capture all instances of in a document.</param>
    /// <returns>A DocumentLab FluentQuery with a script extension that performs the Any operation.</returns>
    public static string[] GetAny(this FluentQuery response, string textType)
    {
      response.QueryType = QueryType.Any;
      response.AppendToScript($"Any [{textType}]");

      return response.ExecuteQuery().Select(x => x.Value).ToArray();
    }

    /// <summary>
    /// Helper method for queries with only a single capture token. Executes the query and returns the first result's value. 
    /// </summary>
    /// <param name="fluentQuery">A FluentQuery object containing the script built so far.</param>
    /// <returns>The data specified for capture as a string.</returns>
    private static string ExecuteSingleCapture(this FluentQuery fluentQuery)
    {
      return fluentQuery.ExecuteQuery().FirstOrDefault().Value;
    }

    private static Dictionary<string, string> ExecuteQuery(this FluentQuery fluentQuery)
    {
      if (fluentQuery.QueryType == QueryType.None)
      {
        throw new FluentQueryException("Query includes no capture tokens. Nothing to query for.");
      }

      fluentQuery.AppendToScript(";");

      var interpretationResultJson = fluentQuery
        .Interpreter
        .Interpret(fluentQuery.AnalyzedPage, fluentQuery.Script)
        .ConvertToJson(fluentQuery.Script);

      if (string.IsNullOrWhiteSpace(interpretationResultJson))
      {
        return null;
      }

      var json = JObject.Parse(interpretationResultJson)[FluentQueryConstants.GeneratedScriptQuery];
      Dictionary<string, string> deserializedResult = null;
      switch (fluentQuery.QueryType)
      {
        case QueryType.SingleCapture:
          deserializedResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(interpretationResultJson);
          break;
        case QueryType.MultiCapture:
          deserializedResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(json.ToString());
          break;
        case QueryType.Any:
          deserializedResult = JsonConvert.DeserializeObject<string[]>(json.ToString())
            .Select((x, i) => new KeyValuePair<int, string>(i, x))
            .ToDictionary(x => x.Key.ToString(), x => x.Value);
          break;
        default: throw new FluentQueryException("Query type is invalid");
      }

      if (deserializedResult == null || deserializedResult.Count == 0)
      {
        return null;
      }

      return deserializedResult;
    }
  }
}
