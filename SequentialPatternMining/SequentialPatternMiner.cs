using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        /// <summary>
        /// Creates patterns of length N. Calls the Pruner internally and stores to the class Pattern Dictionary.
        /// This method runs in parallel for length > 1.
        /// </summary>
        /// <param name="length">The length of patterns being created.</param>
        private void GeneratePatternsOfLength(int length)
        {
            var candidatePatterns = new List<Pattern>();
            if (length == 1)
            {
                candidatePatterns = MergePatterns(Lines.SelectMany(l => l.GetAllPatternsLength1()));
            }
            else
            {
                candidatePatterns = Pattern.CandidatePatterns(PatternsOfLength[length - 2]);
                Parallel.ForEach(candidatePatterns, candidatePattern =>
                {
                    // counts the number of lines that contain this contiguous sequential pattern and adds that to the support.
                    candidatePattern.Support = Lines.Where(l => l.ContainsPattern(candidatePattern)).Count();
                });
            }
            PatternsOfLength[length - 1] = Prune(candidatePatterns);
        }


        /// <summary>
        /// Removes patterns that do not meet the minimum support.
        /// </summary>
        /// <param name="patterns">The patterns we are culling.</param>
        /// <returns>The patterns that were not culled</returns>
        private List<Pattern> Prune(List<Pattern> patterns)
        {
            return patterns.Where(p => p.Support >= MinimumSupport).ToList();
        }

        /// <summary>
        /// Compresses a list of patterns to a list of unique patterns with a support value reflecting the occurance.
        /// </summary>
        /// <param name="patterns">The uncompressed patterns</param>
        /// <returns>The compressed patterns</returns>
        private static List<Pattern> MergePatterns(IEnumerable<Pattern> patterns)
        {
            var PatternNamePatternListDict = patterns.Where(p => p != null).GroupBy(p => string.Join(',', p.Patterns)).ToDictionary(kvp => kvp.Key, kvp => kvp);
            var patternsList = new List<Pattern>();
            foreach (var kvp in PatternNamePatternListDict)
            {
                patternsList.Add(new Pattern(kvp.Key)
                {
                    Support = kvp.Value.Sum(pat => pat.Support)
                });
            }

            return patternsList;
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
