namespace DocumentLab.PageInterpreter.DataModel
{
  using Enum;

  public class Symbol
  {
    public SymbolType SymbolType { get; }
    public object Value { get; }

    public Symbol(SymbolType symbolType, object value = null)
    {
      this.SymbolType = symbolType;
      this.Value = value;
    }

    public T GetValue<T>() {
      return (T)this.Value;
    }
  }
}
