using Godot;
using System;

public class Knife : RigidBody2D
{
	public bool spawned = false;
	public float velocity = 0f;
	public RigidBody2D parent;

	private void OnKnifeAreaEntered(object body)
	{
		if (body == Player.player)
		{
			GameData.playerHealth -= 5;
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		float movement = 0f;

		if (!GameData.timeStopped)
		{
			movement = velocity;
		}

		if (!GameData.timeStopped)
			MoveLocalX(movement * delta);
	}

	public override void _Process(float delta)
	{
		if (!spawned)
		{
			GlobalPosition = parent.GlobalPosition;
			spawned = true;
		}
	}

	private void OnTimeLeftTimeout()
	{
		QueueFree();
	}
}
