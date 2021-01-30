using System;

public class Item : IEquatable<Item>
{
    public string Name {get; set;}
    public Item(string name)
    {
        Name = name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }

    public override string ToString()
    {
        return Name;
    }

    public bool Equals(Item other)
    {
        if (other == null)
            return false;

        return this.Name == other.Name;
    }

    public override bool Equals(Object obj)
    {
        if (obj == null)
            return false;

        Item item = obj as Item;
        if (item == null)
            return false;
        else
            return Equals(item);
    }

    public static bool operator == (Item item1, Item item2)
    {
        if (((object)item1) == null || ((object)item2) == null)
            return Object.Equals(item1, item2);

        return item1.Equals(item1);
    }

    public static bool operator != (Item item1, Item item2)
    {
        return !(item1 == item2);
    }
}