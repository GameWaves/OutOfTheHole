using System;
using Godot;

namespace OutOfTheHole.Gun;

public partial class bullet : RigidBody2D
{
    private Vector2 direction;

    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    [Export] private int longetivity;
    public float Speed = 800.0f;

    /// <summary>
    ///     Initialize the default longevity and compute the direction.
    /// </summary>
    public override void _Ready()
    {
        longetivity = 100;
        direction = new Vector2(1, 0).Rotated(Rotation);
    }

    /// <summary>
    ///     Updated 60 times per seconds, detects when the bullet needs to be destroyed.
    /// </summary>
    /// <param name="delta"></param>
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