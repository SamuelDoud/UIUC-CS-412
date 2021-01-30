using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Apriori
{
    public List<Transaction> Transactions {get; set;}

    public List<ItemSet> FrequencySets {get; set;}

    public int MinimumSupport {get; set;}

    public Apriori(string text, int minimumSupport)
    {
        Console.WriteLine("Apriori");
        FrequencySets = new List<ItemSet>();
        MinimumSupport = minimumSupport;
        var lines = text.Split(Environment.NewLine, StringSplitOptions.None);
        Transactions = lines.Select(l => new Transaction(l)).ToList();

        Debug.WriteLine("0");
        var oneFrequencyItems = Transactions.SelectMany(t => t.Items);
        var continueIteration = FrequencyCount(oneFrequencyItems);
        Debug.WriteLine("1");
        
        var level = 2;
        while (continueIteration && level < 100)
        {
            continueIteration = FrequencyCount(level);
            Prune();
            Debug.WriteLine(level);
            level++;
            
        }

        foreach (var itemSet in FrequencySets)
        {
            Debug.WriteLine(itemSet.ToString());
        }

    }
    
    private bool FrequencyCount(IEnumerable<Item> items)
    {
        var any = false;
        foreach (var item in items)
        {
            if (CheckIfItemSetContains(new List<Item>() {item}))
            {
                any = true;
                //Debug.Write("++" + item.Name);
            }
            else
            {
                FrequencySets.Add(new ItemSet(new List<Item>() {item} ));
                //Debug.WriteLine("+"  + item.Name);
            }
        }
        return any;
    }

    private bool FrequencyCount(int targetCount)
    {
        var any = true;
        foreach (var itemSet in FrequencySets.Where(iSet => iSet.Count == targetCount - 1))
        {
            if(CheckIfItemSetContains(itemSet.Items))
            {
                any = true;
            }
        }
        return any;
    }

    private bool CheckIfItemSetContains(List<Item> items)
    {
        foreach (var itemSet in FrequencySets.Where(fs => fs.Items.Count() == items.Count))
        {
            if (itemSet.Items.Count > 1)
            {
                
            }
            if(itemSet.Contains(items))
            {
                // Item Set object handles the increment
                return true;
            }
        }
        FrequencySets.Add(new ItemSet(items));
        return false;
    }

    private void Prune()
    {
        FrequencySets = FrequencySets.Where(fs => fs.Count > MinimumSupport).ToList();
    }

}