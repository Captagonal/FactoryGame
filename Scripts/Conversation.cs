using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Conversation : Control
{
	List<string> Lines = new();
	List<Texture2D> Icons = new();
	Timer nextChar, nextLine;
	TextureRect Icon;
	Label Text;
	int charIndex = 0;
	int lineIndex = 0;
	public void create(List<string> Lines, List<Texture2D> Icons)
	{
		this.Lines = Lines;
		this.Icons = Icons;

	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Icon = GetNode<TextureRect>("Icon");
		Text = GetNode<Label>("Text");
		nextChar = GetNode<Timer>("NextChar");
		nextLine = GetNode<Timer>("NextLine");
		nextChar.Timeout += OnWriteChar;
		nextLine.Timeout += PlayCurrentLine;
	// 	start([
	// 	"Line 1",
	// 	"Line 2 sadsadashfkjladh sfkhkjdfhakljfh",
	// 	"Hahah Silly",
	// 	"Line 4 which is extra super long",
	// ], [
	// 	GD.Load<Texture2D>("res://icon.svg"),
	// 	GD.Load<Texture2D>("res://icon.svg"),
	// 	GD.Load<Texture2D>("res://icon.svg"),
	// 	GD.Load<Texture2D>("res://icon.svg"),
	// ]);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Accept"))
		{
			if (!nextLine.IsStopped()) {
				nextLine.Stop();
				PlayCurrentLine();
			}
			// SkipLine();
			nextChar.WaitTime = .03;
		}
		
		if (Input.IsActionJustReleased("Accept"))
		{
			// SkipLine();
			nextChar.WaitTime = 0.076;
		}

	}

	public void start(List<string> Lines, List<Texture2D> Icons)
	{
		Visible = true;
		this.Lines = Lines;
		this.Icons = Icons;
		charIndex = 0;
		lineIndex = 0;
		PlayCurrentLine();

	}
	public void start(Dictionary<string, Texture2D> dict){
		start(dict.Keys.ToList(), dict.Values.ToList());
	}
	private void PlayCurrentLine()
	{
		nextLine.Stop();
		if (lineIndex < Lines.Count)
		{
			Text.Text = "";
			charIndex = 0;

			// Update Icon if available
			if (lineIndex < Icons.Count)
				Icon.Texture = Icons[lineIndex];

			nextChar.Start();
		}
		else
		{
			Visible = false;
		}
	}
	private void OnWriteChar()
	{
		string currentLine = Lines[lineIndex];

		if (charIndex < currentLine.Length)
		{
			Text.Text += currentLine[charIndex];
			charIndex++;
		}
		else
		{
			// Line finished
			nextChar.Stop();
			OnLineFinished();
		}
	}
	private void OnLineFinished()
	{
		lineIndex++;
		nextLine.Start();
	}

	private void SkipLine()
	{
		if (!nextChar.IsStopped())
		{
			// Optional: Skip to end of line if player clicks while typing
			Text.Text = Lines[lineIndex];
			charIndex = Lines[lineIndex].Length;
			nextChar.Stop();
			OnLineFinished();
		}
		else if (lineIndex < Lines.Count - 1)
		{
			OnLineFinished();
		}
	}
}
