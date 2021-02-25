using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace SequentialPatternMining
{
    public class SequentialPatternMiner
    {
        List<Line> Lines { get; set; }
        Dictionary<int, List<Pattern>> PatternsOfLength { get; set; }
        public int MinimumSupport {get; set;}
        public SequentialPatternMiner(string patterns, double minSupport)
        {
            var resultFile = "results.txt";
            Lines = patterns.Split("\n").Select(p => new Line(p)).Where(l => l.Words.Count() > 0).ToList();
            MinimumSupport = (int) Math.Floor(minSupport * Lines.Count);
            PatternsOfLength = new Dictionary<int, List<Pattern>>();
            int length = 0;
            do
            {
                length++;
                GeneratePatternsOfLength(length);
            } while (PatternsOfLength[length - 1] != null && PatternsOfLength[length - 1].Count > 0);

            if (File.Exists(resultFile))
            {
                File.Delete(resultFile);
            }
            using (StreamWriter file = new StreamWriter(resultFile))
            {
                file.Write(this.ToString());
            }
        }

        private void CullLines(List<Pattern> patterns, int length)
        {
            var patternsLookup = new HashSet<string>(patterns.SelectMany(p => p.Patterns));
            var count = Lines.SelectMany(l => l.Words).Count();
            if (length == 1)
            {
                Parallel.ForEach(Lines, line =>
                {
                    var tempWords = new List<string>();
                    foreach (var word in line.Words)
                    {
                        if (patternsLookup.Contains(word))
                        {
                            tempWords.Add(word);
                        }
                    }
                    line.Words = tempWords.ToArray();
                });
            }
            var countDiff = count - Lines.SelectMany(l => l.Words).Count();
            // to do other lengths
        }

        private Dictionary<string, Pattern> CullPatterns(Dictionary<string, Pattern> passedPatternsDict, int length)
        {
            var cullPatterns = PatternsOfLength[length - 1];
            var patternsDict = new Dictionary<string, Pattern>();
            foreach (var kvp in passedPatternsDict)
            {
                if (cullPatterns.Any(cp => cp.IsSequentialSubsetOf(kvp.Value)))
                {
                    patternsDict[kvp.Key] = passedPatternsDict[kvp.Key];
                }
            }
            return patternsDict;
        }

        private void GeneratePatternsOfLength(int length)
        {
            if (length == 1)
            {
                var allPatterns = new List<Pattern>();
                Console.Write(string.Format("{0} at length {1} ", "Generate patterns parallel section", length));
                var stopWatch = Stopwatch.StartNew();
                Parallel.For(0, Lines.Count, i =>
                {
                    var tempPatterns = Lines[i].GetAllPatternsLength1();
                    lock ("a")
                    {
                        allPatterns.AddRange(tempPatterns);
                    }
                });
                Console.WriteLine(string.Format("took {0}ms.", stopWatch.ElapsedMilliseconds));
                var mergedPatterns = MergePatterns(allPatterns);
                var prunedPatterns = Prune(mergedPatterns);
                PatternsOfLength[length - 1] = prunedPatterns;
            }
            else
            {
                PatternsOfLength[length - 1] = new List<Pattern>();
                var candidatePatterns = Pattern.OffByOne(PatternsOfLength[length - 2]);
                Parallel.ForEach(candidatePatterns, candidatePattern =>
                {
                    candidatePattern.Support = Lines.Where(l => l.ContainsPattern(candidatePattern)).Count();
                });
                PatternsOfLength[length - 1] = candidatePatterns.Where(cp => cp.Support >= MinimumSupport).ToList();

            }
        }



        private List<Pattern> Prune(List<Pattern> patterns)
        {
            return patterns.Where(p => p.Support >= MinimumSupport).ToList();
        }

        private static List<Pattern> MergePatterns(List<Pattern> patterns)
        {
            var dictionary = patterns.Where(p => p != null).GroupBy(p => string.Join(',', p.Patterns)).ToDictionary(kvp => kvp.Key, kvp => kvp);
            patterns = new List<Pattern>();
            foreach (var kvp in dictionary)
            {
                patterns.Add(new Pattern(kvp.Key)
                {
                    Support = kvp.Value.Sum(pat => pat.Support)
                });
            }

            return patterns;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var kvp in PatternsOfLength)
            {
                foreach (var pattern in kvp.Value)
                {
                    stringBuilder.AppendFormat("{0}\n", pattern.ToString());
                }

            }
            return stringBuilder.ToString();
        }
    }
}
