using System;
using Godot;


namespace OutofTheHole.Entity.Players;

using OutofTheHole.Helpers;

public partial class Player : Entity
{
	public string GunType = "Basic";
	
	//Set a Jump Height / Jump speed
	public const float JumpVelocity = -400.0f;

	public new static bool Alive;

	private Gun.Gun _gunObject;

	private float _timeUntilFire = 300f;

	public new float Acceleration = 10.0f;

	public float Gravity;
 
	private float GunRotation;

	[Export] private PackedScene _gunScene;

	/// <summary>
	/// All value are in pixel
	/// </summary>
	public new int Hp;

	//define the sprite (currently placholder)

	public new int MaxHp = 100;

	public new float Speed = 200.0f;
	

	
	public bool Reversed;
	public override void _Ready()
	{
		// instantiate the gun of the player 
		_gunObject = _gunScene.Instantiate<Gun.Gun>();
		_gunObject.Id = int.Parse(Name);
		
		//set the sprite
		if (Reversed)
		{
			this.Gravity = -ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
			_idleSprite = GetNode<AnimatedSprite2D>("Idle2");
			_walkleft = GetNode<AnimatedSprite2D>("WalkLeft2");
			_walkright = GetNode<AnimatedSprite2D>("WalkRight2");
		}
		else
		{
			this.Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
			_idleSprite = GetNode<AnimatedSprite2D>("Idle");
			_walkleft = GetNode<AnimatedSprite2D>("WalkLeft");
			_walkright = GetNode<AnimatedSprite2D>("Walkright");
		}

		//set player hp
		Hp = MaxHp;
		Alive = true;

		// Allows this player to be played only by the player that is assigned to player 1
		GD.Print(Name);
		GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse((string)Name));
	}


	/// <summary>
	/// Main loop, will update every 60 frame
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

			if (Reversed)
			{
				// Add the gravity to the vector velocity /!\ on godot, y axis is inverted, as such you gain Y when you go down 
				if (!IsOnCeiling()) velocity.Y += Gravity * (float)delta;
				
				// Handle Jump
				if ((Input.IsActionPressed("jump") || Input.IsKeyPressed(Key.Up)) && IsOnCeiling())
					velocity.Y -= JumpVelocity;
			}
			else
			{
				// Add the gravity to the vector velocity /!\ on godot, y axis is inverted, as such you gain Y when you go down 
				if (!IsOnFloor()) velocity.Y += Gravity * (float)delta;
				
				// Handle Jump
				if ((Input.IsActionPressed("jump") || Input.IsKeyPressed(Key.Up)) && IsOnFloor())
					velocity.Y += JumpVelocity;
			}

			Vector2 pos = CoordsHelper.ConvertCoords(new Vector2(1920, 1080), new Vector2(320, 180),
				GetViewport().GetMousePosition());
			
			GetNode<Node2D>("Gun").LookAt(pos);

			//set movement (currenty arrow)
			if (Input.IsActionPressed("move_right"))
			{
				//show (or not) each sprite
				_idleSprite.Visible = false;
				_walkleft.Visible = false;
				_walkright.Visible = true;
				if (velocity.X >= 300)
					velocity.X -= Speed;
				else if (velocity.X < 0)
					velocity.X -= Acceleration;
				else
					velocity.X = Speed;
			}

			if (Input.IsActionPressed("move_left"))
			{
				//show (or not) each sprite
				_idleSprite.Visible = false;
				_walkleft.Visible = true;
				_walkright.Visible = false;
				if (velocity.X <= -300)
					velocity.X += Speed;
				else if (velocity.X > 0)
					velocity.X += Acceleration;
				else
					velocity.X = -Speed;
			}

			//adding a litlle momentum
			if ((!Input.IsActionPressed("move_left") && !Input.IsActionPressed("move_right")) ||
				(Input.IsActionPressed("move_left") && Input.IsActionPressed("move_right")))
			{
				//show (or not) each sprite
				_idleSprite.Visible = true;
				_walkleft.Visible = false;
				_walkright.Visible = false;
				if (velocity.X <= 8 * Acceleration && velocity.X >= -8 * Acceleration) velocity.X = 0;

				if (velocity.X > 0) velocity.X -= 8 * Acceleration;

				if (velocity.X < 0) velocity.X += 8 * Acceleration;
			}

			Velocity = velocity;

			//play the sprite
			if (_idleSprite.Visible) _idleSprite.Play();

			if (_walkleft.Visible) _walkleft.Play();

			if (_walkright.Visible) _walkright.Play();

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
	/// Set the damage, and death
	/// </summary>
	/// <param name="n"> the amount of damage taken</param>
	public override void Hurt(int n,Entity source)
	{
		Hp = Hp - n;
		if (Hp <= 0)
		{
			Death();
		}
	}
	
	/// <summary>
	/// This function receives the fire commands from the other player via the "FireBulletRpc" message
	/// It sends the calls to the FireBullet function in the Gun Class.
	/// </summary>
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	
	private void FireBulletRpc()
	{
		var gunNode = GetNode<Node2D>("Gun");
		var shootPoint = GetNode<Node2D>("Gun/ShootPoint");
		_gunObject.FireBullet(gunNode, shootPoint, GetTree(),GunType,this);
	}
}
