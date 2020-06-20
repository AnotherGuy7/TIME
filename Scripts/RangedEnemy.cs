using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public class RangedEnemy : KinematicBody2D
{
	[Export]
	public PackedScene weapon;

	const int moveSpeed = 60;
	const int jumpForce = -150;
	const int gravity = 10;
	const int maxGravity = 75;

	private float yVel = 0f;
	private int attackCooldown = 0;     //so the player doesn't get dealt 10 damage every frame that he's in the punch area
	private int healthShowTimer = 0;

	public AnimatedSprite enemyAnim;
	private TextureRect healthRect;
	private TextureRect healthRectOutline;
	private CollisionShape2D collision;
	private RayCast2D wallDetector;

	private bool attack = false;
	private int direction = 1;
	private int enemyHealth = 100;
	private bool dying = false;
	private int dyingTimer = 0;
	private int directionChangeCooldown = 0;
	private Random rand = new Random();

	private Color fullyVisible = new Color(1f, 1f, 1f, 1f);
	private Color invisible = new Color(1f, 1f, 1f, 0f);

	public override void _Ready()
	{
		enemyAnim = (AnimatedSprite)GetNode("EnemySprite");
		healthRect = (TextureRect)GetNode("HealthOutline/Health");
		healthRectOutline = (TextureRect)GetNode("HealthOutline");
		collision = (CollisionShape2D)GetNode("EnemyShape");
		wallDetector = (RayCast2D)GetNode("WallDetector");
		healthRectOutline.Modulate = invisible;
		enemyHealth = 75;
	}

	public void Hit(int damage = 0)     //updates life and shows the health bar
	{
		enemyHealth -= damage;
		healthShowTimer = 240;
		healthRectOutline.Modulate = fullyVisible;
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 velocity = Vector2.Zero;
		if (CanMove())
		{
			if (direction == 1)		//right
			{
				velocity.x += moveSpeed;
				enemyAnim.Play("Running");
				enemyAnim.FlipH = false;
				wallDetector.CastTo = new Vector2(7f, 0f);
			}
			if (direction == -1)		//left
			{
				velocity.x -= moveSpeed;
				enemyAnim.Play("Running");
				enemyAnim.FlipH = true;
				wallDetector.CastTo = new Vector2(-8f, 0f);
			}
			if (velocity.x == 0f)       //if the enemy is jumping or falling, new animations get passed under this
			{
				enemyAnim.Play("Idle");
			}
			if (IsOnFloor())
			{
				if (RandRangeInt(75) == 1)
				{
					yVel = jumpForce;
				}
			}
			if (!IsOnFloor())
			{
				if (yVel <= 0)
				{
					enemyAnim.Play("Jumping");
				}
				if (yVel > 0)
				{
					enemyAnim.Play("Falling");
				}
				if (yVel < maxGravity)
				{
					yVel += gravity;
				}
			}
			if (attack)
			{
				velocity = Vector2.Zero;
				enemyAnim.Play("Punching");
			}
			velocity.y = yVel;
		}
		if (dying)
		{
			if (!IsOnFloor())
			{
				velocity.y = 15f;
				enemyAnim.Play("FallingDying");
			}
			if (IsOnFloor())
			{
				dyingTimer++;
				collision.Disabled = true;
				enemyAnim.Play("Dying");
			}
		}

		if (attack)
		{
			Area2D projectile = (Area2D)weapon.Instance();
			GameData.enemiesCategory.AddChild(projectile);
			projectile.Set("thrower", this);
			projectile.Set("velocity", moveSpeed * 1.5f * direction);
			projectile.GlobalPosition = GlobalPosition;
			attack = false;
		}

		if (dyingTimer <= 0 && !GameData.timeStopped)		//if the enemy is on the ground and dying and time isn't stopped
			MoveAndSlide(velocity, new Vector2(0, -1));
	}

	public override void _Process(float delta)
	{
		healthRect.RectScale = new Vector2(enemyHealth / 7.5f, 2f);
		if (healthShowTimer > 0)
			healthShowTimer--;
		else
			healthRectOutline.Modulate = invisible;

		if (healthShowTimer < 120)
		{
			healthRectOutline.Modulate = new Color(1f, 1f, 1f, healthShowTimer / 120f);
		}
		if (enemyHealth <= 0)
		{
			enemyHealth = 0;
			dying = true;
		}
		if (GameData.timeStopped)
			enemyAnim.Stop();
		if (dyingTimer > 0)
		{
			Modulate = new Color(1f, 1f, 1f, (-dyingTimer + 120) / 120f);
			if (dyingTimer >= 120)
			{
				Die();
			}
		}

		if (attackCooldown > 0)
			attackCooldown--;

		if (!GameData.timeStopped)
		{
			if (attackCooldown <= 0)
			{
				attack = true;
				attackCooldown += 120 + RandRangeInt();
			}
		}
		if (directionChangeCooldown > 0)
			directionChangeCooldown--;
		if (directionChangeCooldown <= 0)
		{
			directionChangeCooldown += 180;
			int directionChange = RandRangeInt(50);		//I had to do something weird here cause I don't really know how to get a negative of a random from RandRangeInt
			if (directionChange == 0)
				direction = -1;
			else
				direction = 1;
		}
	}

	private void Die()
	{
		GameData.playerMana += 10;
		GameData.score += 15;
		QueueFree();
	}

	private bool CanMove()
	{
		return !dying && !GameData.timeStopped;
	}

	private int RandRangeInt(int dividedBy = 1)
	{
		return (int)((rand.NextDouble() * 100) / dividedBy);
	}
}
