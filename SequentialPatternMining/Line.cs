using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SequentialPatternMining
{
    public class Line
    {
        private string[] _Words { get; set; }
        public string[] Words {
            get
            {
                return _Words;
            }
            set 
            {
                WordsHashset = new HashSet<string>(value);
                _Words = value;
            }
        }
        public HashSet<string> WordsHashset { get; set; }
        public Line(string lineText)
        {
            Words = lineText.Split(" ").Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();
            WordsHashset = new HashSet<string>(Words);
        }

        public bool ContainsPattern(Pattern pattern)
        {
            if (pattern.Patterns.Any(p => !WordsHashset.Contains(p))) 
                return false;
            for (int i = 0; i < Words.Length - pattern.Length + 1; i++)
            {
                if (Words[i] == pattern.Patterns[0] && Words[i + 1] == pattern.Patterns[1])
                {
                    if (pattern.Length == 2)
                    {
                        return true;
                    }
                    if (Words.Skip(i).Take(pattern.Length).SequenceEqual(pattern.Patterns))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<Pattern> GetAllPatternsLength1()
        {
            return new HashSet<Pattern>(this.Words.Select(w => new Pattern(w))).ToList();
        }
    }
}
