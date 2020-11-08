namespace DocumentLab.Application
{
  using DocumenLab;
  using System;
  using System.Drawing;
  using System.IO;

  public class Program
  {
    public static void Main(string[] args)
    {
      if (args == null || args.Length < 2)
      {
        Console.WriteLine($"First argument needs to be a path to a picture, second argument needs to be a path to a text file containing queries. Third argument is optional, but specifies a file path to output the results to.");
        return;
      }

      var pictureFilePath = args[0];
      var scriptFilePath = args[1];
      var outputFilePath = string.Empty;
      if (args.Length == 3)
      {
        outputFilePath = args[2];
      }

      if (!File.Exists(pictureFilePath))
      {
        Console.WriteLine($"Specified picture file path does not exist");
      }

      if (!File.Exists(scriptFilePath))
      {
        Console.WriteLine($"Specified script file path does not exist");
      }

      var script = File.ReadAllText(scriptFilePath);
      var patternInterpreter = new DocumentInterpreter();

      var result = patternInterpreter.InterpretToJson(script, (Bitmap)Image.FromFile(pictureFilePath));

      Console.WriteLine(result);

      if (!string.IsNullOrWhiteSpace(outputFilePath))
      {
        File.WriteAllText(outputFilePath, result);
      }
    }
  }
}
