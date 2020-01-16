namespace DocumentLab.PageInterpreterUnitTest
{
  using DocumentLab.Contracts.PageInterpreter;
  using DocumentLab.PageInterpreter;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Newtonsoft.Json;
  using System.Collections.Generic;

  [TestClass]
  public class InterpreterResultConverter_ConvertToJson
  {
    [TestMethod]
    public void Can_Convert_Array_To_Json()
    {
      var fakeResult = new InterpreterResult();

      var script = "Stuff: Any [Text];";

      fakeResult.AddResult("Stuff", "0", "1");
      fakeResult.AddResult("Stuff", "1", "4");

      var json = fakeResult.ConvertToJson(script);

      Assert.IsNotNull(json);

      var result = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);

      Assert.AreEqual(2, result["Stuff"].Length);
      Assert.AreEqual("1", result["Stuff"][0]);
      Assert.AreEqual("4", result["Stuff"][1]);
    }

    [TestMethod]
    public void Can_Convert_Empty_Array_To_Json()
    {
      var fakeResult = new InterpreterResult();

      var script = "Stuff: Any [Text];";

      var json = fakeResult.ConvertToJson(script);

      Assert.IsNotNull(json);

      var result = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);

      Assert.AreEqual(0, result["Stuff"].Length);
    }
    [TestMethod]
    public void Can_Convert_Single_Capture_To_Json()
    {
      var fakeResult = new InterpreterResult();

      var script = "Stuff: [Text];";

      fakeResult.AddResult("Stuff", "0", "1");

      var json = fakeResult.ConvertToJson(script);

      Assert.IsNotNull(json);

      var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

      Assert.AreEqual(1, result["Stuff"].Length);
      Assert.AreEqual("1", result["Stuff"]);
    }

    [TestMethod]
    public void Can_Convert_Empty_Single_Capture_To_Json()
    {
      var fakeResult = new InterpreterResult();

      var script = "Stuff: [Text];";

      var json = fakeResult.ConvertToJson(script);

      Assert.IsNotNull(json);

      var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

      Assert.AreEqual(0, result["Stuff"].Length);
    }

    [TestMethod]
    public void Can_Convert_Single_Multi_Capture()
    {
      var fakeResult = new InterpreterResult();

      var script = "Stuff: 'A': [Text] 'B': [Text];";

      fakeResult.AddResult("Stuff", "A", "1");
      fakeResult.AddResult("Stuff", "B", "2");

      var json = fakeResult.ConvertToJson(script);

      Assert.IsNotNull(json);

      var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);

      Assert.AreEqual("1", result["Stuff"]["A"]);
      Assert.AreEqual("2", result["Stuff"]["B"]);
    }

    [TestMethod]
    public void Can_Convert_Empty_Single_Multi_Capture()
    {
      var fakeResult = new InterpreterResult();

      var script = "Stuff: 'A': [Text] 'B': [Text];";

      var json = fakeResult.ConvertToJson(script);

      Assert.IsNotNull(json);

      var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);

      Assert.AreEqual(0, result["Stuff"].Count);
    }
  }
}
