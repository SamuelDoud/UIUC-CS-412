using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace SequentialPatternMining
{
    public class Pattern : IEquatable<Pattern>
    {
        public List<string> Patterns { get; set; }
        public int Length => Patterns.Count;
        public int Support { get; set; }
        public Pattern(string pattern)
        {
            Patterns = pattern.Split(',').ToList();
            Support = 1;
        }

        public Pattern(string[] pattern)
        {
            Patterns = pattern.ToList();
            Support = 1;
        }

        /// <summary>
        /// Computes the patterns of length N + 1 that could exist based on the input of length N patterns.
        /// </summary>
        /// <param name="patterns">The length N patterns</param>
        /// <returns>The possible length N + 1 patterns</returns>
        public static List<Pattern> CandidatePatterns(List<Pattern> patterns)
        {
            HashSet<Pattern> patternsHashSet = new HashSet<Pattern>();
            var skip = 0;
            foreach (var topPattern in patterns)
            {
                skip++;
                foreach (var innerPattern in patterns.Skip(skip))
                {
                    var offCount = 0;
                    foreach (var word in innerPattern.Patterns)
                    {
                        if (!topPattern.Patterns.Contains(word))
                        {
                            offCount++;
                        }
                        if (offCount > 1)
                        {
                            break;
                        }
                    }
                    if (offCount == 1)
                    {
                        patternsHashSet.Add(Merge(topPattern, innerPattern));
                        patternsHashSet.Add(Merge(innerPattern, topPattern));
                    }
                }
            } 
            return patternsHashSet.ToList();
        }


        /// <summary>
        /// Computes the one combination of almost possible patterns given two almost equal patterns.
        /// A B C / A B D -> A B D C/ A B C D (need to run this twice with reversed params to work)
        /// </summary>
        /// <param name="topPattern">One, almost equal, pattern.</param>
        /// <param name="lowerPattern">The other, almost equal, pattern.</param>
        /// <returns>A merged pattern.</returns>
        private static Pattern Merge(Pattern topPattern, Pattern lowerPattern)
        {
            List<string> strings = new List<string>();
            for (int i = 0; i < topPattern.Patterns.Count; i++)
            {
                strings.Add(topPattern.Patterns[i]);
                if (lowerPattern.Patterns[i] != topPattern.Patterns[i])
                {
                    strings.Add(lowerPattern.Patterns[i]);
                }
            }
            return new Pattern(strings.ToArray());
        }

        public static bool operator ==(Pattern pattern1, Pattern pattern2)
        {
            if (((object)pattern1) == null || ((object)pattern2) == null)
                return Equals(pattern1, pattern2);

            return pattern1.Equals(pattern2);
        }

        public static bool operator !=(Pattern pattern1, Pattern pattern2)
        {
            return !(pattern1 == pattern2);
        }

        public bool Equals([AllowNull] Pattern other)
        {
            return this.Patterns.SequenceEqual(other.Patterns);
        }

        public override bool Equals(object obj)
        {
            return this == obj as Pattern;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            foreach (var pattern in Patterns)
            {
                hashCode ^= pattern.GetHashCode();
            }
            return hashCode;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.Support, string.Join(';', this.Patterns));
        }

    }
}
