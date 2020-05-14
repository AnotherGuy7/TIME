using Godot;
using System;

public class TitleScreen : Control
{
	private AnimationPlayer screenScroller;
	private Camera2D titleCamera;

	public override void _Ready()
	{
		screenScroller = (AnimationPlayer)GetNode("ScreenScroller");
		titleCamera = (Camera2D)GetNode("TitleCam");
	}

	private void OnTextureButtonPressed()
	{
		screenScroller.Play("Character");
	}

	private void OnPlayerButton1Pressed()
	{
		screenScroller.Play("Map");
		GameData.playerType = 1;
	}


	private void OnMapButton1Pressed()
	{
		GetTree().ChangeScene("res://Scenes/Map1.tscn");
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("ui_cancel") && !screenScroller.IsPlaying())
		{
			if (titleCamera.GlobalPosition == new Vector2(600f, 60f))
			{
				screenScroller.PlayBackwards("Character");
			}
			if (titleCamera.GlobalPosition == new Vector2(360f, 60f))
			{
				screenScroller.Play("TIME");
			}
			if (titleCamera.GlobalPosition == new Vector2(120f, 60f))
			{
				GetTree().Quit();
			}
		}
	}
}
