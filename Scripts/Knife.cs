using Godot;
using System;

public class Knife : Area2D
{
	private float velocity = 0f;
	private RangedEnemy thrower;
	private Sprite knifeSprite;

	public override void _Ready()
	{
		knifeSprite = (Sprite)GetNode("KnifeSprite");
	}

	private void OnKnifeBodyEntered(object body)
	{
		if (body == Player.player)
		{
			GameData.playerHealth -= 5;
		}
		if (body != thrower)
			QueueFree();
	}

	public override void _PhysicsProcess(float delta)
	{
		float movement = 0f;

		if (!GameData.timeStopped)
		{
			movement = velocity;
		}
		if (velocity > 0f)
			knifeSprite.FlipH = false;
		if (velocity < 0f)
			knifeSprite.FlipH = true;

		MoveLocalX(movement * delta);
	}

	private void OnTimeLeftTimeout()
	{
		QueueFree();
	}
}
