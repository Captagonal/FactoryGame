public class Task
{
    public ItemType itemType { get; private set; }
    public int amount { get; private set; }
    public Destination destination { get; private set; }
    public Task(ItemType itemType, int amount, Destination destination)
    {
        this.itemType = itemType;
        this.amount = amount;
        this.destination = destination;
    }

    public bool TaskCompleted()
    {
        return amount <= 0;
    }

    public void ProcessTask()
    {
        if (amount > 0)
        {
            amount--;
        }
    }

}
public enum Destination{
    Storage,
    OutPost,
}