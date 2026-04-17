using System;
using System.Collections.Generic;

public class Recipie
{
    List<ItemType> inputs;
    public ItemType output { get; private set; }
    public Recipie(List<ItemType> inputs, ItemType output)
    {
        this.inputs = inputs;
        this.output = output;
    }

    internal bool CanProcess(Item item)
    {
        return inputs.Contains(item.getType());
    }
}