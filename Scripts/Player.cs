using Godot;
using System;

public class Player : KinematicBody2D
{
	const int damage = 10;
	const int moveSpeed = 100;
	const int jumpForce = -150;
	const int gravity = 10;
	const int maxGravity = 75;
	int direction = 1;
	public float yVel = 0f;

	public AnimatedSprite playerAnim;
	public Camera2D camera;
	public static Player player;
	private TextureRect healthRect;
	private TextureRect manaRect;
	private RayCast2D punchRay;
	private RayCast2D dashRay;
	private TextureRect darkening;

	private bool dashing = false;
	private int dashTimer = 0;
	private bool timeEvasion = false;
	private int timeEvasionTimer = 0;
	private bool dying = false;
	private int dyingTimer = 0;

	public override void _Ready()
	{
		playerAnim = (AnimatedSprite)GetNode("PlayerAnim");
		camera = (Camera2D)GetNode("PlayerCam");
		punchRay = (RayCast2D)GetNode("PunchRay");
		dashRay = (RayCast2D)GetNode("DashRay");
		healthRect = (TextureRect)GetNode("PlayerCam/PlayerHealthOutline/Health");
		manaRect = (TextureRect)GetNode("PlayerCam/PlayerManaOutline/Mana");
		darkening = (TextureRect)GetNode("PlayerCam/Darkening");
		player = this;
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 velocity = Vector2.Zero;
		if (CanMove())
		{
			if (Input.IsActionPressed("move_left"))
			{
				velocity.x += moveSpeed;
				direction = 1;
				playerAnim.Play("Running");
				playerAnim.FlipH = false;
				punchRay.CastTo = new Vector2(5f, 0f);
			}
			if (Input.IsActionPressed("move_right"))
			{
				velocity.x -= moveSpeed;
				direction = -1;
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
			if (Input.IsActionJustPressed("power3"))
			{
				velocity = Vector2.Zero;
				dashing = true;
			}
			if (Input.IsActionJustPressed("jump") && CanMove())
			{
				yVel = jumpForce;
			}
			if (dying)
			{
				playerAnim.Play("Dying");
				dyingTimer++;
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
			if (dying)
			{
				playerAnim.Play("FallingDying");
				yVel = 15f;
			}
		}
		velocity.y = yVel;

		if (dashing)
		{
			dashTimer++;
			playerAnim.Stop();
			if (dashTimer >= 80)
			{
				GlobalPosition += new Vector2(60f * direction, 0f);
				if (dashRay.IsColliding())
				{
					var target = dashRay.GetCollider();
					for (int i = 0; i < GameData.enemyTypes.Length; i++)
					{
						if (target.GetType().ToString() == GameData.enemyTypes[i])
						{
							switch (GameData.enemyTypes[i])
							{
								case "Enemy":       //didn't really know how else to do this
									{
										Enemy enemy = target as Enemy;
										enemy.Hit(damage * 8);
										break;
									}
								case "RangedEnemy":
									{
										RangedEnemy rangedEnemy = target as RangedEnemy;
										rangedEnemy.Hit(damage * 8);
										break;
									}
								default:
									break;
							}
						}
					}
				}
				dashing = false;
				dashTimer = 0;
			}
		}

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
		if (Input.IsActionJustPressed("power2") && GameData.playerMana >= 25)		//Time dash
		{
			GameData.playerMana -= 25;
		}
		if (Input.IsActionJustPressed("power3") && GameData.playerMana >= 50)		//Time Evasion (evade all attacks for a few seconds)
		{
			timeEvasion = true;
		}
		if (Input.IsActionJustPressed("power4") && GameData.playerMana >= 100)		//Timestop
		{
			GameData.playerMana -= 100;
			GameData.timeStopped = true;
		}
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			GameData.gamePaused = true;
		}

		if (GameData.playerHealth <= 0)
		{
			if (!dying)
				dying = true;
			if (dyingTimer >= 120 && dyingTimer <= 240)
			{
				darkening.Modulate = new Color(0f, 0f, 0f, (-dyingTimer + 120f) / 120f);
			}
			if (dyingTimer >= 360)
			{
				GetTree().ChangeScene("res://Scenes/TitleScreen.tscn");
			}
		}
	}

	private bool CanMove()
	{
		return !dashing && !dying;
	}
}
