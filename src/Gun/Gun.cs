using System;
using Godot;

public partial class Gun : Node2D
{
    [Export] private float bps = 5f;

    [Export] private float bullet_damage = 30f;

    //arbitrary values for the ability to shoot
    [Export] private PackedScene bullet_scn;
    [Export] private float bullet_speed = 800f;
    [Export] private CharacterBody2D CharacterBody;

    private float fire_rate;

    private float time_until_fire = 300f;

    /// <summary>
    ///     Initiate the fire_rate value when the player spawn;
    /// </summary>
    public override void _Ready()
    {
        GD.Print("HEYY");
        fire_rate = 1 / bps;
        GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(Name));
        GD.Print(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer"));
    }

    /// <summary>
    ///     Test when the left_click is pressed and create the bullet as a new node with initial values (also verify if the
    ///     player is able to shoot)
    /// </summary>
    /// <param name="delta">seconds</param>
    public override void _Process(double delta)
    {
        
        var maxdistance = 20f;
        if (Input.IsActionPressed("click") && fire_rate < time_until_fire )
        {
            var playerPosition = CharacterBody.GlobalPosition;

            var bullet = bullet_scn.Instantiate<RigidBody2D>(); // create the bullet
            var mousePostion = GetGlobalMousePosition() - playerPosition;

            bullet.GlobalPosition =
                playerPosition + maxdistance * mousePostion.Normalized(); //change the position according to the mouse

            if (mousePostion.X < 0)
                bullet.Rotation = (float)Math.Atan(mousePostion.Y / mousePostion.X) + Mathf.Pi;
            else
                bullet.Rotation = (float)Math.Atan(mousePostion.Y / mousePostion.X);

            GetTree().Root.AddChild(bullet);
            bullet.LinearVelocity = (bullet.GlobalPosition - playerPosition).Normalized() * bullet_speed;
            time_until_fire = 0f;
        }
        else
        {
            time_until_fire += (float)delta; //timer until ability to shoot again
        }
    }
}