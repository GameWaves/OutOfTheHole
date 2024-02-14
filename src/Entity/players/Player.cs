using System;
using Godot;
using OutofTheHole.Gun;

namespace Entity.players;

public partial class Player : OutOfTheHole.Entity
{
	public string GunType = "Basic";
	
	//Set a Jump Height / Jump speed
	public const float JumpVelocity = -400.0f;

	public new static bool Alive;

	private Gun _gunObject;

	private float _timeUntilFire = 300f;

	public new float Acceleration = 10.0f;

	public float Gravity;
 
	private float GunRotation;

	[Export] private PackedScene gunScene;

	/// <summary>
	///     All value are in pixel
	/// </summary>
	public new int Hp;

	//define the sprite (currently placholder)
	private AnimatedSprite2D idleSprite;

	public new int MaxHp = 100;

	public new float Speed = 200.0f;

	private AnimatedSprite2D Walkleft;

	private AnimatedSprite2D Walkright;

	public bool reversed;
	public override void _Ready()
	{
		// instantiate the gun of the player 
		_gunObject = gunScene.Instantiate<Gun>();
		_gunObject.Id = int.Parse(Name);
		
		//set the sprite
		if (reversed)
		{
			this.Gravity = -ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
			idleSprite = GetNode<AnimatedSprite2D>("Idle2");
			Walkleft = GetNode<AnimatedSprite2D>("WalkLeft2");
			Walkright = GetNode<AnimatedSprite2D>("WalkRight2");
		}
		else
		{
			this.Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
			idleSprite = GetNode<AnimatedSprite2D>("Idle");
			Walkleft = GetNode<AnimatedSprite2D>("WalkLeft");
			Walkright = GetNode<AnimatedSprite2D>("Walkright");
		}

		//set player hp
		Hp = MaxHp;
		Alive = true;

		// Allows this player to be played only by the player that is assigned to player 1
		GD.Print(Name);
		GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse((string)Name));
	}


	/// <summary>
	///     Main loop, will update every 60 frame
	/// </summary>
	/// <param name="delta"> it is the time</param>
	/// <returns> void </returns>
	public override void _PhysicsProcess(double delta)
	{

		// Check if the player is allowed to control this player according to the authority set at initialization. 
		if (Alive && GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() ==
			Multiplayer.GetUniqueId())
		{
			//create a variable velocity 
			var velocity = Velocity;

			if (reversed)
			{
				// Add the gravity to the vector velocity /!\ on godot, y axis is inverted, as such you gain Y when you go down 
				if (!IsOnCeiling()) velocity.Y += Gravity * (float)delta;
				
				// Handle Jump
				if ((Input.IsKeyPressed(Key.Space) || Input.IsKeyPressed(Key.Up)) && IsOnCeiling())
					velocity.Y -= JumpVelocity;
			}
			else
			{
				// Add the gravity to the vector velocity /!\ on godot, y axis is inverted, as such you gain Y when you go down 
				if (!IsOnFloor()) velocity.Y += Gravity * (float)delta;
				
				// Handle Jump
				if ((Input.IsKeyPressed(Key.Space) || Input.IsKeyPressed(Key.Up)) && IsOnFloor())
					velocity.Y += JumpVelocity;
			}
			
			GetNode<Node2D>("Gun").LookAt(GetViewport().GetMousePosition());

			//set movement (currenty arrow)
			if (Input.IsKeyPressed(Key.Right))
			{
				//show (or not) each sprite
				idleSprite.Visible = false;
				Walkleft.Visible = false;
				Walkright.Visible = true;
				if (velocity.X >= 300)
					velocity.X -= Speed;
				else if (velocity.X < 0)
					velocity.X -= Acceleration;
				else
					velocity.X = Speed;
			}

			if (Input.IsKeyPressed(Key.Left))
			{
				//show (or not) each sprite
				idleSprite.Visible = false;
				Walkleft.Visible = true;
				Walkright.Visible = false;
				if (velocity.X <= -300)
					velocity.X += Speed;
				else if (velocity.X > 0)
					velocity.X += Acceleration;
				else
					velocity.X = -Speed;
			}

			//adding a litlle momentum
			if ((!Input.IsKeyPressed(Key.Left) && !Input.IsKeyPressed(Key.Right)) ||
				(Input.IsKeyPressed(Key.Left) && Input.IsKeyPressed(Key.Right)))
			{
				//show (or not) each sprite
				idleSprite.Visible = true;
				Walkleft.Visible = false;
				Walkright.Visible = false;
				if (velocity.X <= 8 * Acceleration && velocity.X >= -8 * Acceleration) velocity.X = 0;

				if (velocity.X > 0) velocity.X -= 8 * Acceleration;

				if (velocity.X < 0) velocity.X += 8 * Acceleration;
			}

			Velocity = velocity;

			//play the sprite
			if (idleSprite.Visible) idleSprite.Play();

			if (Walkleft.Visible) Walkleft.Play();

			if (Walkright.Visible) Walkright.Play();

			//kill yourself (to test gameover screen)
			if (Input.IsKeyPressed(Key.K)) Hurt(MaxHp);

			if (Input.IsActionPressed("click") && _gunObject.FireRate < _timeUntilFire)
			{
				_timeUntilFire = 0f;
				Rpc("FireBulletRpc");
			}
			else
			{
				_timeUntilFire += (float)delta; //timer until ability to shoot again
			}


			// function MoveAndSlide apply the Velocity to the player
			MoveAndSlide();

			//Prepare the gun rotation for the sync and the "Mathf.Lerp".
			GunRotation = GetNode<Node2D>("Gun").RotationDegrees;
		}
		else
		{
			//Always sync the Gun rotation
			GetNode<Node2D>("Gun").RotationDegrees =
				Mathf.Lerp(GetNode<Node2D>("Gun").RotationDegrees, GunRotation, .1f);
		}
	}

	/// <summary>
	///     Set the damage, and death
	/// </summary>
	/// <param name="n"> the amount of damage taken</param>
	public override void Hurt(int n)
	{
		Hp = Hp - n;
		if (Hp <= 0)
		{
			Alive = false;
			//hide the player 
			idleSprite.Visible = false;
			Walkright.Visible = false;
			Walkleft.Visible = false;
			death();
		}
	}
	
	/// <summary>
	///     This function receives the fire commands from the other player via the "FireBulletRpc" message
	///     It sends the calls to the FireBullet function in the Gun Class.
	/// </summary>
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	
	private void FireBulletRpc()
	{
		var gunNode = GetNode<Node2D>("Gun");
		var shootPoint = GetNode<Node2D>("Gun/ShootPoint");
		_gunObject.FireBullet(gunNode, shootPoint, GetTree(),GunType);
	}
}
