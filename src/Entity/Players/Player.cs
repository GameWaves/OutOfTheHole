using System;
using Godot;


namespace OutofTheHole.Entity.Players;

using OutofTheHole.Helpers;

public partial class Player : Entity
{
	public string GunType = "Basic";
	
	//Set a Jump Height / Jump speed
	public const float JumpVelocity = -250.0f;

	public new static bool Alive;

	private Gun.Gun _gunObject;

	private float _timeUntilFire = 300f;

	public new float Acceleration = 10.0f;

	public float Gravity;
 
	private float GunRotation;

	private AnimationPlayer _spirtePlayer;

	private string _lastInput;

	private bool _gotPicked;

	public Vector2 Spawn;

	[Export] private PackedScene _gunScene;

	[Export] public Camera2D Cam;

	/// <summary>
	/// All value are in pixel
	/// </summary>
	public new int Hp;

	//define the sprite (currently placholder)

	public new int MaxHp = 100;

	public new float Speed = 100.0f;

	public bool jump;
	
	public bool Reversed;
	public override void _Ready()
	{
		GD.Print(_gunScene);
		// instantiate the gun of the player 
		_gunObject = _gunScene.Instantiate<Gun.Gun>();
		_gunObject.Id = int.Parse(Name);
		
		//set the sprite
		if (Reversed)
		{
			this.Gravity = -ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
			Spawn = GetParent().GetNode("Checkpoints").GetNode<Node2D>("0").Position;
			Position = Spawn;
		}
		else
		{
			this.Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
			Spawn = GetParent().GetNode("Checkpoints").GetNode<Node2D>("1").Position;
			Position = Spawn;
		}
		_spirtePlayer = GetNode<AnimationPlayer>("Animations");
		
		//set player hp
		Hp = MaxHp;
		Alive = true;

		// Allows this player to be played only by the player that is assigned to player 1
		GD.Print(Name);
		GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse((string)Name));
		
		// Instantiate the personal camera for each player
		if (Multiplayer.GetUniqueId() == GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority())
		{
			Cam.MakeCurrent();
		}

		_spirtePlayer.Play("WalkRight");
	}

	/// <summary>
	/// Will be called every frame
	/// </summary>
	/// <param name="delta"></param>
	public override void _Process(double delta)
	{
		base._Process(delta);
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
				if (jump)
				{
					velocity.Y -= JumpVelocity;
					jump = false;
				}
			}
			else
			{
				// Add the gravity to the vector velocity /!\ on godot, y axis is inverted, as such you gain Y when you go down 
				if (!IsOnFloor()) velocity.Y += Gravity * (float)delta;
				
				// Handle Jump
				if (jump)
				{
					velocity.Y += JumpVelocity;
					jump = false;
				}
			}

			Vector2 pos = GetGlobalMousePosition();
			
			GetNode<Node2D>("Gun").LookAt(pos);

			//set movement (currenty arrow)
			if (Input.IsActionPressed("move_right"))
			{
				if (velocity.X >= 300)
					velocity.X -= Speed;
				else if (velocity.X < 0)
					velocity.X -= Acceleration;
				else
					velocity.X = Speed;
				//show the animation going right
				_spirtePlayer.Play("WalkRight");
				_lastInput = " ";
			}

			if (Input.IsActionPressed("move_left"))
			{
				if (velocity.X <= -300)
					velocity.X += Speed;
				else if (velocity.X > 0)
					velocity.X += Acceleration;
				else
					velocity.X = -Speed;
				//show the animation going left
				_spirtePlayer.Play("WalkLeft");
				_lastInput = "left";
			}

			//adding a litlle momentum
			if ((!Input.IsActionPressed("move_left") && !Input.IsActionPressed("move_right")) ||
				(Input.IsActionPressed("move_left") && Input.IsActionPressed("move_right")))
			{
				//show (or not) each sprite
				if (_lastInput == "left")
					_spirtePlayer.Play("IdleLeft");
				else
					_spirtePlayer.Play("IdleRight");
				if (velocity.X <= 8 * Acceleration && velocity.X >= -8 * Acceleration) velocity.X = 0;

				if (velocity.X > 0) velocity.X -= 8 * Acceleration;

				if (velocity.X < 0) velocity.X += 8 * Acceleration;
			}

			Velocity = velocity;
			
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
			if (invicibleTime != 0)
			{
				invicibleTime -= 1;
			}
			else
			{
				IsInvicible = false;
			}
		}
		else
		{
			//Always sync the Gun rotation
			GetNode<Node2D>("Gun").RotationDegrees =
				Mathf.Lerp(GetNode<Node2D>("Gun").RotationDegrees, GunRotation, .1f);
		}

		if (_gotPicked)
		{
			KillPlayer(this);
		}
	}
	/// <summary>
	/// Wrapper for Death() Method that also includes a game quit. 
	/// </summary>
	/// <param name="target"></param>
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void KillPlayer(Entity target)
	{
		
		Position = Spawn;
		Hp = MaxHp;
		//Death();
		//GetTree().Quit();
	}
	
	/// <summary>
	/// Sends the message to both instances that player should be Hurt, with intensity n and from the source.
	/// If the player is invicible, nothing append
	/// </summary>
	/// <param name="n"> the amount of damage taken</param>
	/// <param name="source">Entity That Hurt the player</param>
	public override void Hurt(int n, Entity source)
	{
		if (IsInvicible != true)
		{
			if (source is Player)
			{
				n = 1;
				jump = true;
			}
			Rpc("HurtPlayer", n, source);	
		}
		
	}
	
	/// <summary>
	/// Set the damage, and death
	/// </summary>
	/// <param name="n"> the amount of damage taken</param>
	/// <param name="source">Entity That Hurt the player</param>
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void HurtPlayer(int n, Entity source)
	{
		if (IsInvicible != true)
		{
			Hp = Hp - n;
			GD.Print($"Player {Name} Hurted by {source.Name}, Hp = {Hp}, Managed by {Multiplayer.GetUniqueId()}");
			if (Hp <= 0)
			{
				// GD.Print($"Player {Name} Killed by {source.Name}", $" ID {Name}");
				Rpc("KillPlayer", Name); // Propagates the info that the player {Name} Should be killed. 
			}
		}

		IsInvicible = true;
		invicibleTime = 50;

	}
	
	/// <summary>
	/// This function receives the fire commands from the other player via the "FireBulletRpc" message
	/// It sends the calls to the FireBullet function in the Gun Class.
	/// </summary>
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void FireBulletRpc()
	{
		// GD.Print($"{Name} shot, Managed by {Multiplayer.GetUniqueId()}");
		var gunNode = GetNode<Node2D>("Gun");
		var shootPoint = GetNode<Node2D>("Gun/ShootPoint");
		_gunObject.FireBullet(gunNode, shootPoint, GetTree(),GunType,this);
	}

	private void _on_hit_box_map_body_entered(Node2D body)
	{
		GD.Print("got picked by body");
		_gotPicked = true;
	}
	
}
