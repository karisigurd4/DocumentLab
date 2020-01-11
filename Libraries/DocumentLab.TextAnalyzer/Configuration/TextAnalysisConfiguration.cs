namespace DocumentLab.TextAnalyzer.Configuration
{
  public class TextAnalysisConfiguration
  {
    public string ConvertDatesToFormat { get; set; }
    public string EmailAddressRegex { get; set; }
    public string BadOcrEmailAddressRegex { get; set; }
    public string WebAddressRegex { get; set; }
    public string BadOcrWebAddressRegex { get; set; }
    public string TextRegex { get; set; }
    public string AmountIgnoreRegex { get; set; }
    public string[] TryParseDateTimeFormats { get; set; }
    public string[] StreetAddressRegexes { get; set; }
    public string[] PostalCodeRegexes { get; set; }
    public string[] KnownDateDelimiters { get; set; }
    public string[] AmountRegexes { get; set; }
    public string[] PercentageRegexes { get; set; }
    public string[] NumberRegexes { get; set; }
    public string[] DateRegexes { get; set; }
    public string[] FirstPassPageNumberRexeges { get; set; }
    public string[] SecondPassPageNumberRegexes { get; set; }
    public string[] InvoiceNumberRegexes { get; set; }
    public string[] LettersRegexes { get; set; }
  }
}
