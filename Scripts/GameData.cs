using Godot;
using System;

public class GameData : Node2D		//variables and stuff
{
	public static int playerType;
	public static int playerHealth;
	public static int playerMana;
	public static int score;

	public static bool timeStopped = false;

	private int abilityDuration;

	public static string[] enemyTypes = new string[10];

	public override void _Ready()
	{
		playerHealth = 100;
		playerMana = 100;
		score = 0;
		enemyTypes[0] = "Enemy";
		enemyTypes[1] = "RangedEnemy";
	}

	public override void _Process(float delta)
	{
		if (playerHealth <= 0)
		{
			GetTree().ChangeScene("res://Scenes/TitleScreen.tscn");
			playerHealth = 0;
		}
		if (timeStopped)
		{
			abilityDuration++;
			if (abilityDuration >= 180)
			{
				abilityDuration = 0;
				timeStopped = false;
			}
		}
	}
}
