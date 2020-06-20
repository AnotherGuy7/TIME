using Godot;
using System;

public class GameData : Node2D		//variables and stuff
{
	public static int playerType;
	public static int playerHealth;
	public static int playerMana;
	public static int score;

	public static bool timeStopped = false;
	public static bool gamePaused = false;

	private int abilityDuration;

	public static string[] enemyTypes = new string[10];

	public static Node2D enemiesCategory;		//this is used for projectiles like Ranged Enemy's Knife

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
			playerHealth = 0;
		}
		if (playerMana <= 0)
		{
			playerMana = 0;
		}
		if (playerMana >= 100)
		{
			playerMana = 100;
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
