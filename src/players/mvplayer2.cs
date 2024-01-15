using Godot;

namespace OutofTheHole.players;

public partial class mvplayer2 : CharacterBody2D
{
    /// <summary>
    ///     Same as player 1  but this time with player 2 (inverted)
    /// </summary>
    public const float Speed2 = 10.0f;

    //Set a initial speed (arbitrarly chosen)
    public const float intSpeed2 = 200.0f;

    //Set a slowing down speed (arbitrarly chosen)
    public const float SlowSpeed2 = 40.0f;

    //Set a Jump Height / Jump speed (arbitrarly chosen)
    public const float JumpVelocity2 = 400.0f;

    //set maxHp of player
    public const int MaxHp2 = 100;

    //to know if the player is alive
    public static bool alive;
    [Export] private float bps = 5f;

    [Export] private float bullet_damage = 30f;

    //arbitrary values for the ability to shoot
    [Export] private PackedScene bullet_scn;
    [Export] private float bullet_speed = 800f;
    [Export] private CharacterBody2D CharacterBody;


    private float fire_rate;

    //Set a gravity (do not change, is the default to have a consistant gravity accros the game)
    public float gravity2 = -ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private float GunRotation;
    public int hp2;

    //define the sprite (currently placholder)
    private AnimatedSprite2D idleSprite;
    private float time_until_fire = 300f;
    private AnimatedSprite2D Walkleft;
    private AnimatedSprite2D Walkright;

    public override void _Ready()
    {
        //set the sprite
        idleSprite = GetNode<AnimatedSprite2D>("Idle2");
        Walkleft = GetNode<AnimatedSprite2D>("WalkLeft2");
        Walkright = GetNode<AnimatedSprite2D>("WalkRight2");
        alive = true;

        // Allows this player to be played only by the player that is assigned to player 2
        GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(Name));
    }

    /// <summary>
    ///     Main loop, will update every 60 frame
    /// </summary>
    /// <param name="delta"> it is the time</param>
    /// <returns> void </returns>
    public override void _PhysicsProcess(double delta)
    {
        // Check if the player is allowed to control this player according to the authority set at initialization. 
        if (alive && GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() ==
            Multiplayer.GetUniqueId())
        {
            //create a variable velocity 
            var velocity = Velocity;

            GetNode<Node2D>("Gun").LookAt(GetViewport().GetMousePosition());

            // /!\ Player is inverted as such he is on the ceilling. if you use the function is on floor replace it with is on ceilling

            // Add the gravity to the vector velocity /!\ on godot, y axis is inverted, as such you gain Y when you go down 
            if (!IsOnCeiling())
                velocity.Y += gravity2 * (float)delta;

            // Handle Jump
            if ((Input.IsKeyPressed(Key.Space) || Input.IsKeyPressed(Key.Up)) && IsOnCeiling())
                velocity.Y += JumpVelocity2;

            //set movement (currenty arrow)
            if (Input.IsKeyPressed(Key.Right))
            {
                //show (or not) each sprite
                idleSprite.Visible = false;
                Walkleft.Visible = false;
                Walkright.Visible = true;
                if (velocity.X >= 200)
                    velocity.X += Speed2;
                else if (velocity.X < 0)
                    velocity.X += SlowSpeed2;
                else
                    velocity.X = intSpeed2;
            }

            if (Input.IsKeyPressed(Key.Left))
            {
                //show (or not) each sprite
                idleSprite.Visible = false;
                Walkleft.Visible = true;
                Walkright.Visible = false;
                if (velocity.X <= -200)
                    velocity.X -= Speed2;
                else if (velocity.X > 0)
                    velocity.X -= SlowSpeed2;
                else
                    velocity.X = -intSpeed2;
            }

            //adding a litlle momentum
            if ((!Input.IsKeyPressed(Key.Left) && !Input.IsKeyPressed(Key.Right)) ||
                (Input.IsKeyPressed(Key.Left) && Input.IsKeyPressed(Key.Right)))
            {
                //show (or not) each sprite
                idleSprite.Visible = true;
                Walkleft.Visible = false;
                Walkright.Visible = false;
                if (velocity.X <= SlowSpeed2 && velocity.X >= -SlowSpeed2) velocity.X = 0;

                if (velocity.X > 0) velocity.X -= SlowSpeed2;

                if (velocity.X < 0) velocity.X += SlowSpeed2;
            }

            Velocity = velocity;


            //play the sprite
            if (idleSprite.Visible) idleSprite.Play();

            if (Walkleft.Visible) Walkleft.Play();

            if (Walkright.Visible) Walkright.Play();

            //kill yourself (to test gameover screen)
            if (Input.IsKeyPressed(Key.K)) ishurt2(MaxHp2);
            if (Input.IsActionPressed("click") && fire_rate < time_until_fire)
            {
                time_until_fire = 0f;
                Rpc("fire");
            }
            else
            {
                time_until_fire += (float)delta; //timer until ability to shoot again
            }


            // function MoveAndSlide apply the Velocity to the player
            MoveAndSlide();
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
    public void ishurt2(int n)
    {
        hp2 = hp2 - n;
        if (hp2 <= 0)
        {
            alive = false;
            //hide the player 
            idleSprite.Visible = false;
            Walkright.Visible = false;
            Walkleft.Visible = false;
            if (!mvplayer.alive) GetTree().ChangeSceneToFile("res://GameOver.tscn");
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void fire()
    {
        var bullet = bullet_scn.Instantiate<RigidBody2D>(); // create the bullet
        bullet.Rotation = GetNode<Node2D>("Gun").Rotation;
        bullet.GlobalPosition = GetNode<Node2D>("Gun/ShootPoint").GlobalPosition;
        GetTree().Root.AddChild(bullet);
    }
}