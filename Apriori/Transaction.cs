using System.Collections.Generic;
using System.Linq;

public class Transaction
{
    public HashSet<Item> Items {get; private set;}
    public Transaction(string line, string separator=";")
    {
        Items = new HashSet<Item>(line.Split(separator).Select(i => new Item(i)).OrderBy(i => i.Name).ToList());
    }

    public bool Contains(ItemSet itemSet)
    {
        return itemSet.Items.IsSubsetOf(Items);
    }
}