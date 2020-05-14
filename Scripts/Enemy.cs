using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public class Enemy : KinematicBody2D
{
	const int moveSpeed = 60;
	//const int jumpForce = -150;
	const int gravity = 10;
	const int maxGravity = 75;

	private float yVel = 0f;
	private int attackCooldown = 0;     //so the player doesn't get dealt 10 damage every frame that he's in the punch area
	private int healthShowTimer = 0;

	public AnimatedSprite enemyAnim;
	private Area2D punchArea;
	private TextureRect healthRect;
	private TextureRect healthRectOutline;
	private TextureRect dyingShader;
	private CollisionShape2D collision;
	private Player target;
	private bool attack = false;
	private int enemyHealth = 100;
	public Enemy enemy;
	private bool dying = false;
	private int dyingTimer = 0;

	private Color fullyVisible = new Color(1f, 1f, 1f, 1f);
	private Color invisible = new Color(1f, 1f, 1f, 0f);

	public override void _Ready()
	{
		enemyAnim = (AnimatedSprite)GetNode("EnemySprite");
		punchArea = (Area2D)GetNode("PunchArea");
		healthRect = (TextureRect)GetNode("HealthOutline/Health");
		healthRectOutline = (TextureRect)GetNode("HealthOutline");
		dyingShader = (TextureRect)GetNode("EnemySprite/Dying");
		collision = (CollisionShape2D)GetNode("EnemyShape");
		healthRectOutline.Modulate = invisible;
		enemyHealth = 100;
		enemy = this;
	}
	
	private void OnDetectionAreaEntered(object body)
	{
		if (body == Player.player)
			target = Player.player;
	}
	
	private void OnPunchAreaEntered(object body)
	{
		if (body == Player.player && attack && attackCooldown <= 0)
		{
			GameData.playerHealth -= 10;
			attackCooldown += 35;
		}
	}

	private void OnPunchDetectionEntered(object body)
	{
		if (body == Player.player)
			attack = true;
	}
	
	private void OnPunchDetectionExited(object body)
	{
		if (body == Player.player)
			attack = false;
	}

	public void Hit(int damage = 0)		//updates life and shows the health bar
	{
		enemyHealth -= damage;
		healthShowTimer = 240;
		healthRectOutline.Modulate = fullyVisible;
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 velocity = Vector2.Zero;
		if (!GameData.timeStopped)
		{
			if (target != null)
			{
				if (!dying)
				{
					if (!attack)
					{
						if (GlobalPosition.x < target.GlobalPosition.x)
						{
							velocity.x += moveSpeed;
							enemyAnim.Play("Running");
							enemyAnim.FlipH = false;
							punchArea.Position = new Vector2(-2f, 1f);
						}
						if (GlobalPosition.x >= target.GlobalPosition.x)
						{
							velocity.x -= moveSpeed;
							enemyAnim.Play("Running");
							enemyAnim.FlipH = true;
							punchArea.Position = new Vector2(1f, 1f);
						}
						if (velocity.x == 0f)       //if the enemy is jumping or falling, new animations get passed under this
						{
							enemyAnim.Play("Idle");
						}
					}
					/*if (IsOnFloor())
					{
						if (Input.IsActionPressed("Power1"))
						{
							punchArea.Visible = true;
						}
						if (Input.IsActionJustPressed("jump"))
						{
							yVel = jumpForce;
						}
					}*/
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
				}
				velocity.y = yVel;
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
					}
				}
			}

			if (dyingTimer <= 0 && !GameData.timeStopped)
				MoveAndSlide(velocity, new Vector2(0, -1));
		}
	}

	public override void _Process(float delta)
	{
		if (GameData.timeStopped)
			enemyAnim.Stop();

		if (attackCooldown > 0)
			attackCooldown--;

		healthRect.RectScale = new Vector2(enemyHealth / 10f, 2f);
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
		if (dyingTimer > 0)
		{
			enemyAnim.Play("Dying");
			dyingShader.Visible = true;
			collision.Disabled = true;
		}
		if (dyingTimer >= 120)
		{
			Die();
		}
	}

	private void Die()
	{
		GameData.playerMana += 5;
		GameData.score += 10;
		QueueFree();
	}
}
