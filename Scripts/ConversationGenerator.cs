using System;
using System.Collections.Generic;
using Godot;

public class ConversationGenerator
{
    public static Dictionary<Character, Dictionary<Expression, Texture2D>> CharacterIcon { get; } = new Dictionary<Character, Dictionary<Expression, Texture2D>> {
        { Character.Bloblin, new Dictionary<Expression, Texture2D> {
            {Expression.Suprised, GD.Load<Texture2D>("res://Models/textures/Bloblin/Suprised.png") },
            {Expression.Disapointed, GD.Load<Texture2D>("res://Models/textures/Bloblin/Disapointed.png") },
            {Expression.Happy, GD.Load<Texture2D>("res://Models/textures/Bloblin/Happy.png") },
            {Expression.Neutral, GD.Load<Texture2D>("res://Models/textures/Bloblin/Neutral.png") },
            {Expression.Space, GD.Load<Texture2D>("res://Models/textures/Bloblin/Space.png") },
            {Expression.Sad, GD.Load<Texture2D>("res://Models/textures/Bloblin/Sad.png") } }
        },
        { Character.Human, new Dictionary<Expression, Texture2D> {
            {Expression.Happy, GD.Load<Texture2D>("path/to/texture") },
            {Expression.Disapointed, GD.Load<Texture2D>("path/to/texture") },
            {Expression.Suprised, GD.Load<Texture2D>("path/to/texture") } }
        },
    };
    public static Dictionary<string, Texture2D> TaskToConversation(Task task)
    {
        List<string> Text = new();
        List<Texture2D> Icons = new();

        Text.Add("Hello I was wondering if you could help me");
        Icons.Add(CharacterIcon[Character.Bloblin][Expression.Neutral]);

        Text.Add("I need you to deliver " + task.amount + " " + task.itemType.ToString() + " to " + task.destination);
        Icons.Add(CharacterIcon[Character.Bloblin][Expression.Neutral]);

        Text.Add("Thank you so much for your help!!");
        Icons.Add(CharacterIcon[Character.Bloblin][Expression.Happy]);

        var result = new Dictionary<string, Texture2D>();
        for (int i = 0; i < Text.Count; i++)
        {
            result[Text[i]] = Icons[i];
        }
        return result;
    }

    public static Dictionary<string, Texture2D> tut1 = new Dictionary<string, Texture2D>{
        {"First lets teach you how to use conveyors!!",CharacterIcon[Character.Bloblin][Expression.Happy] },
        {"Press \"F\" to open the build menu",CharacterIcon[Character.Bloblin][Expression.Happy] },
        {"And then select conveyor!!",CharacterIcon[Character.Bloblin][Expression.Happy] },
        {"Now click on the world to add it to the world",CharacterIcon[Character.Bloblin][Expression.Happy] },
    };
    public static Dictionary<string, Texture2D> introConversation = new Dictionary<string, Texture2D>{
        {"Haiiii I'm Bloblin",CharacterIcon[Character.Bloblin][Expression.Happy] },
        {"I've been alone for a while, so It's REALLY nice to see a new face",CharacterIcon[Character.Bloblin][Expression.Sad] },
        {"I'm wondering if you could maybe do me a favor?",CharacterIcon[Character.Bloblin][Expression.Neutral] },
        {"You see its like my all time dream to go to Space",CharacterIcon[Character.Bloblin][Expression.Space] },
        {"But I have no way to get there",CharacterIcon[Character.Bloblin][Expression.Sad] },
        {"Maybe you could help though?",CharacterIcon[Character.Bloblin][Expression.Happy] },
    };
    public static Dictionary<string, Texture2D> ch2Conversation = new Dictionary<string, Texture2D>{
        {"How's it goinggggg",CharacterIcon[Character.Bloblin][Expression.Happy] },
        {"You've been working so hard",CharacterIcon[Character.Bloblin][Expression.Happy] },
        {"It really means a lot to me...",CharacterIcon[Character.Bloblin][Expression.Happy] },
        {"Do you think I'm gonna make it to space?",CharacterIcon[Character.Bloblin][Expression.Neutral] },
        {"I hope I can",CharacterIcon[Character.Bloblin][Expression.Neutral] },
    };
    public static Dictionary<string, Texture2D> ch3Conversation = new Dictionary<string, Texture2D>{
        {"We're almost there",CharacterIcon[Character.Bloblin][Expression.Happy] },
        {"I can't believe It's actually happening",CharacterIcon[Character.Bloblin][Expression.Happy] },
        {"I'm not even sure if im ready...",CharacterIcon[Character.Bloblin][Expression.Disapointed] },
        {"Maybe if you come with me?",CharacterIcon[Character.Bloblin][Expression.Neutral] },
    };

}
public enum Character
{
    Bloblin,
    Human
}
public enum Expression
{
    Neutral,
    Suprised,
    Happy,
    Sad,
    Mad,
    Disapointed,
    Space
}