using Godot;
using OutofTheHole.Entity;

namespace OutOfTheHole.Enemies
{

	public partial class Enemy : Entity
	{
		//initialize the enemy
		public override void _Ready()
		{
			Speed = 100.0f;
			MaxHp = 300;
			Hp = 200;
			Acceleration = 100;
			Alive = true;
			AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			AnimatedSprite2D.Play();
			//initialize the enemy 
			Alive = true;
		}

		public override void _PhysicsProcess(double delta)
		{
			//make the enemy move
			MoveAndSlide();
			//TODO: not sure of the concept of this method
		}
		
		//death
		public override void Hurt(int hpLoss)
		{
			Hp -= hpLoss;
			if (Hp <= 0)
			{
				Death();
			}
		}
	}
}
