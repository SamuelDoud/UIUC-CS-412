using System.Collections.Generic;
using System.Linq;

public class Transaction
{
    public IEnumerable<Item> Items {get; private set;}
    public Transaction(string line, string separator=";")
    {
        Items = line.Split(separator).Select(i => new Item(i));
    }

    public bool Contains(List<Item> items)
    {
        return items.All(i => Contains(i));
    }

    public bool Contains(Item item)
    {
        return Items.Contains(item);
    }
}