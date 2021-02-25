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

        public bool MergeIfEqual(Pattern other, bool addSupport=true)
        {
            var match = other == this;
            if (match && addSupport)
            {
                this.Support += other.Support;
            }
            return match;
        }

        public static List<Pattern> OffByOne(List<Pattern> patterns)
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

        private static Pattern Merge(Pattern topPattern, Pattern pattern)
        {
            List<string> strings = new List<string>();
            for (int i = 0; i < topPattern.Patterns.Count; i++)
            {
                strings.Add(topPattern.Patterns[i]);
                if (pattern.Patterns[i] != topPattern.Patterns[i])
                {
                    strings.Add(pattern.Patterns[i]);
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

        public override int GetHashCode()
        {
            return Patterns.GetHashCode();
        }

        public bool IsSequentialSubsetOf(Pattern possibleSuperset, bool isProper = false)
        {
            if (possibleSuperset.Length < this.Length)
            if (!(isProper && possibleSuperset.Length == this.Length))
            {
                return false;
            }
            for (int i = 0; i < this.Length; i++)
            {
                if (this.Patterns[i] != possibleSuperset.Patterns[i])
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsSequentialSupersetOf(Pattern possibleSubset, bool isProper = false)
        {
            return possibleSubset.IsSequentialSubsetOf(this, isProper);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.Support, string.Join(';', this.Patterns));
        }

    }
}
