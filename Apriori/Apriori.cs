using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class Apriori
{
    public List<Transaction> Transactions { get; set; }

    public HashSet<ItemSet> FrequencySets { get; set; }

    public int MinimumSupport { get; set; }

    public Apriori(string text, double minimumSupport)
    {
        var resultFile = "results.txt";
        Debug.WriteLine("Apriori");
        FrequencySets = new HashSet<ItemSet>();
        var lines = text.Split(Environment.NewLine, StringSplitOptions.None);
        Transactions = lines.Select(l => new Transaction(l)).ToList();
        MinimumSupport = (int)Math.Floor(minimumSupport * Transactions.Count);

        Debug.WriteLine("0");
        var oneFrequencyItems = Transactions.SelectMany(t => t.Items);
        var continueIteration = FrequencyCount(oneFrequencyItems);
        Prune();
        Debug.WriteLine("1");

        var level = 2;
        while (continueIteration && level < 12)
        {
            continueIteration = FrequencyCount(level);
            Prune();
            Debug.WriteLine(level);
            level++;
        }

        if (File.Exists(resultFile))
        {
            File.Delete(resultFile);
        }
        using (StreamWriter file = new StreamWriter(resultFile))
        {
            foreach (var itemSet in FrequencySets.OrderBy(fs => fs.Items.Count).ThenBy(fs => fs.Count).ThenBy(fs => fs.Items.First().Name))
            {
                Debug.WriteLine(itemSet.ToString());
                file.WriteLine(itemSet.ToString());
            }
        }
    }

    private bool FrequencyCount(IEnumerable<Item> items)
    {
        var any = false;
        foreach (var item in items)
        {
            if (CheckIfFrequencySetContains(new ItemSet(item)))
            {
                any = true;
                //Debug.Write("++" + item.Name);
            }
        }
        return any;
    }

    /// <summary>
    /// Scans for greater than 1 frequency
    /// </summary>
    /// <param name="targetCount">The count of items we are scanning for in transactions</param>
    /// <returns></returns>
    private bool FrequencyCount(int targetCount)
    {
        var removedFromConsiderationLock = new Object();
        var frequencySetsCheckLock = new Object();
        var any = true;

        var removedFromConsideration = new HashSet<ItemSet>();
        var candidateItemSets = FrequencySets.Where(iSet => iSet.Items.Count == targetCount - 1).ToList();
        for (int i = 0; i < candidateItemSets.Count; i++)
        {
            var candidateItemSet = candidateItemSets[i];
            var otherCandidateItemSets = candidateItemSets.Skip(i + 1);
            // Get nearly equal candidate item sets
            Parallel.ForEach(candidateItemSet.OffByOne(otherCandidateItemSets), otherCandidateItemSet =>
            {
                var alreadySeen = false;
                var items = new List<Item>(candidateItemSet.Items.ToList());
                items.AddRange(otherCandidateItemSet.Items.ToList());
                var itemsWithNoDuplicates = new HashSet<Item>(items);
                var newItemSet = new ItemSet(itemsWithNoDuplicates);
                lock (removedFromConsiderationLock)
                {
                    if (removedFromConsideration.Any(iS => iS == newItemSet))
                    {
                        alreadySeen = true;
                    }
                    removedFromConsideration.Add(newItemSet);
                }
                foreach (var transaction in Transactions.Where(t => !alreadySeen && t.Items.Count() >= targetCount && t.Contains(newItemSet)))
                {
                    lock(frequencySetsCheckLock)
                    { 
                        if (CheckIfFrequencySetContains(newItemSet))
                        {
                            any = true;
                        }
                    }
                }
            });
        }
        return any;
    }

    private bool CheckIfFrequencySetContains(ItemSet itemSet)
    {
        if (FrequencySets.TryGetValue(itemSet, out ItemSet existingItemSet))
        {
            existingItemSet.Count++;
            return true;
        }
        // No item set matches
        FrequencySets.Add(itemSet);
        return false;
    }

    private void Prune()
    {
        if (MinimumSupport >= 1)
        {
            FrequencySets.RemoveWhere(fs => fs.Count <= MinimumSupport);
        }
    }

}