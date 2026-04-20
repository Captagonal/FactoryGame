using Godot;

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

    public Task(StoryProgress progress)
    {
        ItemType[] options;

        switch (progress)
        {
            case StoryProgress.None:
                //starting tutorial
                options = [ItemType.Wood, ItemType.Charcoal, ItemType.CopperOre, ItemType.IronOre];
                itemType = options[GD.Randi() % options.Length];
                amount = (int)(GD.Randi() % 50);
                destination = Destination.Storage;
                break;
            case StoryProgress.TutorialConveyor:
                //Learning Machines
                options = [ItemType.Iron, ItemType.Copper];
                itemType = options[GD.Randi() % options.Length];
                amount = (int)(GD.Randi() % 70);
                destination = Destination.Storage;
                break;
            case StoryProgress.TutorialMachine:
                //Can Use Machines
                
                break;
            case StoryProgress.BloblinNeedsHelp:
                break;
            case StoryProgress.BloblinNeedsHelp2:
                break;
            case StoryProgress.BloblinWantsSpace:
                break;
        }
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
public enum Destination
{
    Storage,
    OutPost,
}