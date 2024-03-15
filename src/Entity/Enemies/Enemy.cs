using System;
using Godot;
using OutOfTheHole.Enemies;

namespace OutOfTheHole.Entity.Enemies
{

	public partial class Enemy : OutofTheHole.Entity.Entity
	{
		public new float Speed = 100.0f;
		public new int MaxHp = 300;
		public new float Acceleration = 5.0f;
		public new int Hp;
		public Movements Movements;
		public bool Shooted;

		private AnimatedSprite2D _idleSprite;
		private AnimatedSprite2D _walkleft;
		private AnimatedSprite2D _walkright;

		public bool Reversed;
		public float Gravity;

		//initialize the enemy
		public override void _Ready()
		{
			Hp = 200;
			Alive = true;
			//initialize the enemy 
			Movements = Movements.IDLE;

			if (Reversed)
			{
				this.Gravity = -ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
				_idleSprite = GetNode<AnimatedSprite2D>("Idle");
				_walkleft = GetNode<AnimatedSprite2D>("WalkLeft");
				_walkright = GetNode<AnimatedSprite2D>("WalkRight");
			}

			GD.Print("Enemy Ready");
			// GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse((string)Name));
		}

		public override void _PhysicsProcess(double delta)
		{
			//TODO: not sure of the concept of this method

			Vector2 velocity = Velocity;
			if (Reversed)
			{
				if (!IsOnCeiling()) velocity.Y += Gravity * (float)delta;
			}
			else
			{
				if (!IsOnFloor()) velocity.Y += Gravity * (float)delta;
			}

			// Show the right sprite based on the movements of the enemy
			if (Movements == Movements.RIGHT)
			{
				_walkright.Visible = true;
				_walkleft.Visible = false;
				_idleSprite.Visible = false;
			}
			else if (Movements == Movements.LEFT)
			{
				_walkright.Visible = false;
				_walkleft.Visible = true;
				_idleSprite.Visible = false;
			}
			else
			{
				_walkright.Visible = false;
				_walkleft.Visible = false;
				_idleSprite.Visible = true;
			}

			// Play the sprite
			if (_idleSprite.Visible) _idleSprite.Play();
			if (_walkleft.Visible) _walkleft.Play();
			if (_walkright.Visible) _walkright.Play();

			MoveAndSlide();
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
