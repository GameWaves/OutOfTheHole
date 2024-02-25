using Godot;

namespace OutOfTheHole.Enemies
{

    public class Enemy : CharacterBody2D
    {
        public const float Speed = 10.0f;
        public const float intSpeed = 200.0f;
        public const float SlowSpeed = 40.0f;
        public const float JumpVelocity = -400.0f;
        public const int MaxHp = 100;
        public static bool alive;
        public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
        public int hp;
        private AnimatedSprite2D idleSprite;

        public override void _Ready()
        {
            //initialize the enemy 
            idleSprite = GetNode<AnimatedSprite2D>("Idle");
            idleSprite.Play();
            hp = MaxHp;
            alive = true;
        }

        public override void _PhysicsProcess(double delta)
        {
            //make the enemy move
            MoveAndSlide();
            //TODO: not sure of the concept of this method
        }

        public void Ishurt(int n)
        {
            hp = hp - n;
            if (hp <= 0)
            {
                alive = false;
                idleSprite.Visible = false;
            }
        }
    }
}