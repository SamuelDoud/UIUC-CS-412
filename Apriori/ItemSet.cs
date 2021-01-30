using System;
using System.Collections.Generic;
using System.Linq;

public class ItemSet : IEquatable<ItemSet>
{
    public int Count {get; set;} = 1;
    public HashSet<Item> Items {get; private set; }

    public ItemSet(List<Item> items)
    {
        Items = new HashSet<Item>(items.OrderBy(i => i.Name));
    }

    public ItemSet(Item item)
    {
        Items = new HashSet<Item>() { item };
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

    public IEnumerable<ItemSet> OffByOne(IEnumerable<ItemSet> itemSets)
    {
        foreach (var otherItemSet in itemSets.ToList())
        {
            var offCount = 0;
            foreach (var item in otherItemSet.Items)
            {
                if (!this.Contains(item))
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
                yield return otherItemSet;
            }
        }
    }

    private bool Contains(Item item)
    {
        return Items.Contains(item);
    }

    public override string ToString()
    {
        return Count.ToString() + ":" + string.Join(';', Items.Select(i => i.ToString()));
    }

    public bool Equals(ItemSet other)
    {
        return Enumerable.SequenceEqual(this.Items, other.Items);
    }

    public static bool operator ==(ItemSet item1, ItemSet item2)
    {
        if (((object)item1) == null || ((object)item2) == null)
            return Equals(item1, item2);

        return item1.Equals(item2);
    }

    public static bool operator !=(ItemSet item1, ItemSet item2)
    {
        return !(item1 == item2);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ItemSet);
    }

    public override int GetHashCode()
    {
        int hashCode = 0;
        foreach (var item in Items)
        {
            hashCode ^= item.GetHashCode();
        }
        return hashCode;
    }
}