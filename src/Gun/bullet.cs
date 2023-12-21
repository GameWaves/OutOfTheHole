using System;
using Godot;

public partial class bullet : RigidBody2D
{
    public const float Speed = 800.0f;
    private Vector2 direction;
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    [Export] private int longetivity = 1;

    public override void _Ready()
    {
        longetivity = 100;
        direction = new Vector2(1, 0).Rotated(Rotation);
    }

    public override void _PhysicsProcess(double delta)
    {
        LinearVelocity = Speed * direction;
        if (longetivity == 0)
        {
            QueueFree();
            if (IsQueuedForDeletion()) Free();
        }

        longetivity -= (int)Math.Ceiling(delta);
    }
}