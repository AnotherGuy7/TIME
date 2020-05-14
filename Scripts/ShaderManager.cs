using Godot;
using System;

public class ShaderManager : Control		//basically, this is tied to the player camera so when things go visible here it also affects the player
{
	public static TextureRect timestopShader;

	public override void _Ready()
	{
		timestopShader = (TextureRect)GetNode("Negative");
	}

	public override void _Process(float delta)
	{
		timestopShader.Visible = GameData.timeStopped;
	}
}
