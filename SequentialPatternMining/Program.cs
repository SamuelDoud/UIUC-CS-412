using System;
using System.Diagnostics;
using System.IO;

namespace SequentialPatternMining
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.WriteLine("Started");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "reviews_sample.txt");
            var text = System.IO.File.ReadAllText(path);

            Debug.WriteLine("Text Loaded");
            var minerResult = new SequentialPatternMiner(text, 0.01);
            Console.WriteLine(minerResult.ToString());
        }
    }
}
