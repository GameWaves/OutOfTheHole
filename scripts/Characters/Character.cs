using Godot;

namespace OutOfTheHole.scripts.Characters;

public partial class Character : CharacterBody2D
{
    public float Speed { get; }
    public float Hp { get; set; }
    public float MaxHp { get; }
    public bool Alive { get; set; }
    public float JumpVelocity { get; set; }
    public float Gravity { get; set; }

    public Character(string name, float speed, int hp, float jumpVelocity, bool inverted)
    {
        Speed = speed;
        Hp = hp;
        MaxHp = hp;
        Alive = true;
        JumpVelocity = jumpVelocity;
        Gravity = (inverted ? -1 : 1) * ProjectSettings.GetSetting("Physics/2d/default_gravity").AsSingle();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void Hurt(float damage)
    {
        Hp -= damage;
        if (Hp <= 0) Kill();
    }

    public void Kill()
    {
        Alive = false;
        // TODO: Hide the Sprite
    }

    // To use the position of the character, do this :
    // (float x, float y) = (Position.X, Position.Y);
}