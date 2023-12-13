using System;
using Godot;

public partial class bullet : RigidBody2D
{
    [Export] private int longetivity = 1;

    public override void _Ready()
    {
        longetivity = 100;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (longetivity == 0)
        {
            QueueFree();
            if (IsQueuedForDeletion()) Free();
        }

        longetivity -= (int)Math.Ceiling(delta);
    }
}