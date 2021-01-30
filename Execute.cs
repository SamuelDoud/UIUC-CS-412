using System; 
using System.IO;

public class Execute
{
    public static void Main(string[] Args)
    {
        Console.WriteLine("Started");
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Categories.txt");
        var text = System.IO.File.ReadAllText(path);

        Console.WriteLine("Text Loaded");
        new Apriori(text, 1);
    }
}