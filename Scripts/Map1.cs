using Godot;
using System;

public class Map1 : Node2D
{
	private Random random = new Random();

	[Export]
	public PackedScene enemyToSpawn1;

	[Export]
	public PackedScene enemyToSpawn2;

	[Export]
	public PackedScene enemyToSpawn3;

	[Export]
	public PackedScene enemyToSpawn4;

	private Timer enemySpawnTimer;

	private Position2D playerSpawn;

	public override void _Ready()
	{
		enemySpawnTimer = (Timer)GetNode("EnemySpawnTimer");
		playerSpawn = (Position2D)GetNode("PlayerSpawn");
		/*KinematicBody2D player = (KinematicBody2D)player.Instance();
		AddChild(mobInstance);
		Position2D spawnPoint = (Position2D)GetNode("SpawnPos" + (RandRangeInt() / 33));
		mobInstance.Position = spawnPoint.Position;*/
	}

	private void OnEnemySpawnTimerEnd()
	{
		// Create a Mob instance and add it to the scene.
		if (RandRangeInt(50) == 0)		//to add more just cut the 50 into more pieces (50/100 is 2 potential spawns)
		{
			var mobInstance = (KinematicBody2D)enemyToSpawn1.Instance();
			AddChild(mobInstance);
			Position2D spawnPoint = (Position2D)GetNode("SpawnPos" + (RandRangeInt() / 33));
			mobInstance.Position = spawnPoint.Position;
		}
		else //if ((RandRangeInt() / 50) == 1)
		{
			var mobInstance = (KinematicBody2D)enemyToSpawn2.Instance();
			AddChild(mobInstance);
			Position2D spawnPoint = (Position2D)GetNode("SpawnPos" + (RandRangeInt() / 33));
			mobInstance.Position = spawnPoint.Position;
		}

		//enemySpawnTimer.Start();
	}

	private float RandRange(float min, float max)
	{
		return (float)random.NextDouble() * (max - min) + min;
	}

	private int RandRangeInt(int dividedBy = 1)
	{
		return (int)((random.NextDouble() * 100) / dividedBy);
	}
}
