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

        public List<string[]> GetAllPatternsAsText(int length, int startIndex = 0, string[] existingPattern = null, List<Pattern> hintPatterns=null)
        {
            List<string[]> patterns = new List<string[]>();
            string[] pattern;
            if (existingPattern != null)
            {
                pattern = new string[existingPattern.Length + 1];
                existingPattern.CopyTo(pattern, 0);
                if (hintPatterns != null)
                {
                    foreach (var patternTest in existingPattern.Select(p => new Pattern(p)))
                    {
                        if (!hintPatterns.Any(hp => hp.IsSequentialSupersetOf(patternTest)))
                        {
                            return new List<string[]>();
                        }
                    }
                }
            }
            else
            {
                pattern = new string[1];
            }
            // we dont want to look for patterns that cannot be completed
            for (int i = startIndex; i < Words.Length - (length - pattern.Length); i++)
            {
                var tempArray = new string[pattern.Length];
                if (hintPatterns == null || hintPatterns.Any(hp => hp.IsSequentialSupersetOf(new Pattern(Words[i]))))
                    pattern[pattern.Length - 1] = Words[i];
                else
                    continue;
                pattern.CopyTo(tempArray, 0);
                if (pattern.Length == length)
                {
                    if (hintPatterns == null || hintPatterns.Any(hp => hp.IsSequentialSubsetOf(new Pattern(tempArray), true)))
                        patterns.Add(tempArray);
                }
                else
                {
                    patterns.AddRange(GetAllPatternsAsText(length, i + 1, tempArray, hintPatterns));
                }
            }
            return patterns;

        }

        public List<Pattern> GetAllPatternsLength1()
        {
            return new HashSet<Pattern>(this.Words.Select(w => new Pattern(w))).ToList();
        }
    }
}
