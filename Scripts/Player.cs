using Godot;
using System;

public class Player : KinematicBody2D
{
	const int damage = 10;
	const int moveSpeed = 100;
	const int jumpForce = -150;
	const int gravity = 10;
	const int maxGravity = 75;
	public float yVel = 0f;

	public AnimatedSprite playerAnim;
	public Camera2D camera;
	public static Player player;
	private TextureRect healthRect;
	private TextureRect manaRect;
	private RayCast2D punchRay;

	public override void _Ready()
	{
		playerAnim = (AnimatedSprite)GetNode("PlayerAnim");
		camera = (Camera2D)GetNode("PlayerCam");
		punchRay = (RayCast2D)GetNode("PunchRay");
		healthRect = (TextureRect)GetNode("PlayerCam/PlayerHealthOutline/Health");
		manaRect = (TextureRect)GetNode("PlayerCam/PlayerManaOutline/Mana");
		player = this;
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 velocity = Vector2.Zero;
		if (!Input.IsActionPressed("power1"))
		{
			if (Input.IsActionPressed("move_left"))
			{
				velocity.x += moveSpeed;
				playerAnim.Play("Running");
				playerAnim.FlipH = false;
				punchRay.CastTo = new Vector2(5f, 0f);
			}
			if (Input.IsActionPressed("move_right"))
			{
				velocity.x -= moveSpeed;
				playerAnim.Play("Running");
				playerAnim.FlipH = true;
				punchRay.CastTo = new Vector2(-7f, 0f);
			}
			if (velocity.x == 0f)
			{
				playerAnim.Play("Idle");
			}
		}
		if (IsOnFloor())
		{
			if (Input.IsActionPressed("power1"))
			{
				playerAnim.Play("Punching");
			}
			if (Input.IsActionJustPressed("jump"))
			{
				yVel = jumpForce;
			}
		}
		if (!IsOnFloor())
		{
			if (yVel <= 0)
			{
				playerAnim.Play("Jumping");
			}
			if (yVel > 0)
			{
				playerAnim.Play("Falling");
			}
			if (yVel < maxGravity)
			{
				yVel += gravity;
			}
		}
		velocity.y = yVel;

		MoveAndSlide(velocity, new Vector2(0, -1));
	}

	public override void _Process(float delta)
	{
		healthRect.RectScale = new Vector2(GameData.playerHealth / 3.846f, 7f);
		manaRect.RectScale = new Vector2(GameData.playerMana / 3.846f, 7f);
		if (Input.IsActionPressed("power1") && punchRay.IsColliding())
		{
			var target = punchRay.GetCollider();
			for (int i = 0; i < GameData.enemyTypes.Length; i++)
			{
				if (target.GetType().ToString() == GameData.enemyTypes[i])
				{
					switch (GameData.enemyTypes[i])
					{
						case "Enemy":       //didn't really know how else to do this
						{
							Enemy enemy = target as Enemy;
							enemy.Hit(damage);
							break;
						}
						case "RangedEnemy":
						{
							RangedEnemy rangedEnemy = target as RangedEnemy;
							rangedEnemy.Hit(damage);
							break;
						}
						default:
							break;
					}
				}
			}
		}
		if (Input.IsActionJustPressed("power2") && GameData.playerMana >= 100)
		{
			GameData.playerMana -= 100;
			GameData.timeStopped = true;
		}
	}
}
