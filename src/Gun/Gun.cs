using System;
using Godot;

namespace OutofTheHole.Gun;


public partial class Gun : Node2D
{
    [Export] private float bps = 5f;

    [Export] private float bullet_damage = 30f;

    //arbitrary values for the ability to shoot
    [Export] private PackedScene bullet_scn;
    
    [Export] private float bullet_speed = 800f;
    
    private float fire_rate;

    private float time_until_fire = 300f;

    /// <summary>
    ///     Initiate the fire_rate value when the player spawn;
    /// </summary>
    public override void _Ready()
    {
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
        GetNode<Node2D>("Gun").LookAt(GetViewport().GetMousePosition());
    }

    public void fireBullet()
    {
        var bullet = bullet_scn.Instantiate<RigidBody2D>(); // create the bullet
        bullet.Rotation = GetNode<Node2D>("Gun").RotationDegrees;
        bullet.GlobalPosition = GetNode<Node2D>("Gun/ShootPoint").GlobalPosition;
        GetTree().Root.AddChild(bullet);
        bullet.LinearVelocity = (bullet.GlobalPosition - GetNode<Node2D>("Gun").GlobalPosition).Normalized() * bullet_speed;
    }
}