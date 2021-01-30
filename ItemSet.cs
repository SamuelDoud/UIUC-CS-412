using System;
using System.Collections.Generic;
using System.Linq;

public class ItemSet
{
    public int Count {get; set;} = 1;
    public List<Item> Items {get; set;}
    public ItemSet(List<Item> items)
    {
        Items = items;
        Count = 1;
    }

    public bool Contains(List<Item> items)
    {
        var contains = items.All(i => Contains(i));
        if (contains)
        {
            Count++;
        }
        return contains;
    }

    public bool Contains(Item item)
    {
        return Items.Contains(item);
    }

    public override string ToString()
    {
        return Count.ToString() + ":" + string.Join(';', Items.Select(i => i.ToString()));
    }
}