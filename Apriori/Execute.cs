using System;
using System.Diagnostics;
using System.IO;

public class Execute
{
    public static void Main(string[] Args)
    {
        Debug.WriteLine("Started");
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Categories.txt");
        var text = System.IO.File.ReadAllText(path);

        Debug.WriteLine("Text Loaded");
        new Apriori(text, .01);
    }
}