using Godot;
using System;

public partial class mvplayer : CharacterBody2D
{
	
	/// <summary>
	/// All value are in pixel 
	/// </summary>
	//Set a acceleration speed (arbitrarly chosen)
	public const float Speed = 10.0f;
	//Set a initial speed (arbitrarly chosen)
	public const float intSpeed = 200.0f;
	//Set a slowing down speed (arbitrarly chosen)
	public const float SlowSpeed = 40.0f;
	//Set a Jump Height / Jump speed (arbitrarly chosen)
	public const float JumpVelocity = -400.0f;
	//Set a gravity (do not change, is the default to have a consistant gravity accros the game)
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	//set maxHp of player
	public const int MaxHp = 100;
	public int hp;
	
	//define the sprite (currently placholder)
	private AnimatedSprite2D idleSprite;
	private AnimatedSprite2D Walkleft;
	private AnimatedSprite2D Walkright;

	public override void _Ready()
	{
		//set the sprite
		idleSprite = GetNode<AnimatedSprite2D>("Idle");
		Walkleft = GetNode<AnimatedSprite2D>("WalkLeft");
		Walkright = GetNode<AnimatedSprite2D>("Walkright");
		
		//set player hp
		hp = MaxHp;
	}


	/// <summary>
	/// Main loop, will update every 60 frame 
	/// </summary>
	/// <param name="delta"> it is the time</param>
	/// <returns> void </returns>
	public override void _PhysicsProcess(double delta)
	{
		//create a variable velocity 
		Vector2 velocity = Velocity;

		// Add the gravity to the vector velocity /!\ on godot, y axis is inverted, as such you gain Y when you go down 
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump
		if ((Input.IsKeyPressed(Key.Space) || Input.IsKeyPressed(Key.Up)) && IsOnFloor())
		{
			velocity.Y += JumpVelocity;
		}
		
		//set movement (currenty arrow)
		if (Input.IsKeyPressed(Key.Right))
		{
			//show (or not) each sprite
			idleSprite.Visible = false;
			Walkleft.Visible = false;
			Walkright.Visible = true;
			if (velocity.X >= 200)
			{
				velocity.X += Speed;
			}
			else if (velocity.X < 0 )
			{
				velocity.X += SlowSpeed;
			}
			else
			{
				velocity.X = intSpeed;
			}
		}
		
		if (Input.IsKeyPressed(Key.Left))
		{
			//show (or not) each sprite
			idleSprite.Visible = false;
			Walkleft.Visible = true;
			Walkright.Visible = false;
			if (velocity.X <= -200)
			{
				velocity.X -= Speed;
			}
			else if (velocity.X > 0 )
			{
				velocity.X -= SlowSpeed;
			}
			else
			{
				velocity.X = -intSpeed;
			}
			
		}
		
		//adding a litlle momentum
		if (!Input.IsKeyPressed(Key.Left) && !Input.IsKeyPressed(Key.Right))
		{
			//show (or not) each sprite
			idleSprite.Visible = true;
			Walkleft.Visible = false;
			Walkright.Visible = false;
			if (velocity.X <= SlowSpeed && velocity.X >= -SlowSpeed)
			{
				velocity.X = 0;

			}
			if (velocity.X > 0)
			{
				velocity.X -= SlowSpeed;
			}
			if (velocity.X < 0)
			{
				velocity.X += SlowSpeed;
			}
		}
		
		Velocity = velocity;
		
		//play the sprite 
		if (idleSprite.Visible)
		{
			idleSprite.Play();
		}
		if (idleSprite.Visible)
		{
			idleSprite.Play();
		}
		if (idleSprite.Visible)
		{
			idleSprite.Play();
		}
		
		//kill yourself (to test gameover screen)
		if (Input.IsKeyPressed(Key.K))
		{
			ishurt(MaxHp);
		}

		
		// function MoveAndSlide apply the Velocity to the player
		MoveAndSlide();
	}

	/// <summary>
	/// Set the damage, and death  
	/// </summary>
	/// <param name="n"> the amount of damage taken</param>
	public void ishurt(int n)
	{
		hp = hp - n;
		if (hp <= 0)
		{
			GetTree().ChangeSceneToFile("res://GameOver.tscn");
		}
	}
}
