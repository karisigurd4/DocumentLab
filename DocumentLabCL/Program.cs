namespace DocumentLabCL
{
  using System;
  using System.Drawing;
  using System.IO;
  using DocumentLab;

  public class Program
  {
    public static void Main(string[] args)
    {
      if (args == null || args.Length < 2)
      {
        PrintHelp();
        return;
      }

      var scriptFilePath = args[0];
      var imagePath = args[1];

      if (!File.Exists(scriptFilePath))
      {
        Console.WriteLine("Script file path does not exist");
      }

      if (!File.Exists(imagePath))
      {
        Console.WriteLine("Image file path does not exist");
      }

      var script = File.ReadAllText(scriptFilePath);
      var image = Image.FromFile(imagePath);

      var jsonOut = new DocumentLab().InterpretToJson(script, (Bitmap)image);

      if (args.Length > 2 && !string.IsNullOrWhiteSpace(args[2]))
      {
        File.WriteAllText(args[2], jsonOut);
      }

      Console.WriteLine(jsonOut);
      Console.WriteLine("Press enter to exit");
      Console.Read();
    }

    public static void PrintHelp()
    {
      Console.WriteLine(
@"
Two arguments,

[argument 1]: script file path
[argument 2]: image file path
[argument 3]: (optional) output path

Ex: DocumentLabCL.Exe ""Script.txt"" ""Bitmap.png"" ""output.json""

");
    }
  }
}
