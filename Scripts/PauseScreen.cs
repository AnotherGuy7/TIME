using Godot;
using System;

public class PauseScreen : Control		//will always be with the player so it doesn't really need to be autoloaded or anything
{
	private void OnContinuePressed()
	{
		GameData.gamePaused = false;
	}


	private void OnBackToMenuPressed()
	{
		GetTree().ChangeScene("res://Scenes/TitleScreen.tscn");
	}

	public override void _Process(float delta)
	{
		GetTree().Paused = Visible = GameData.gamePaused;
	}
}
