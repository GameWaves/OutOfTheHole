using System;
using System.Net.Sockets;
using Godot;
using OutOfTheHole.Enemies;
using OutofTheHole.Entity.Players;

namespace OutOfTheHole.Entity.Enemies
{
	public partial class Enemy : OutofTheHole.Entity.Entity
	{
		public new float Speed = 1.0f;
		public new int MaxHp = 60;
		public new float Acceleration = 1.0f;
		public new int Hp;
		public Movements Movements;
		public bool Shooted;

		public bool Reversed;
		public float Gravity;
		
		public int Damage = 30;

		
		public Vector2 Oldvector;

		private int cycledir = 0;
		
		private OutofTheHole.Entity.Entity aggrosource;
		
		
		
		//initialize the enemy
		public override void _Ready()
		{
			Hp = MaxHp;
			Alive = true;
			//initialize the enemy 
			Movements = Movements.IDLE;

			if (Reversed)
			{
				this.Gravity = -ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
				_idleSprite = GetNode<AnimatedSprite2D>("Idle");
				_walkleft = GetNode<AnimatedSprite2D>("WalkLeft");
				_walkright = GetNode<AnimatedSprite2D>("Walkright");
			}
			else
			{
				this.Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
				_idleSprite = GetNode<AnimatedSprite2D>("Idle");
				_walkleft = GetNode<AnimatedSprite2D>("WalkLeft");
				_walkright = GetNode<AnimatedSprite2D>("Walkright");
			}
			
			if (cycledir%2 == 0)
			{
				Movements = Movements.RIGHT;
			}
			else
			{
				Movements = Movements.LEFT;
			}

			cycledir = cycledir + 1;
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

			if (IsOnWall()) {
				if (Movements == Movements.LEFT)
					Movements = Movements.RIGHT;
				else if (Movements == Movements.RIGHT)
					Movements = Movements.LEFT;
			}

			if (aggrosource == null)
			{
				// Show the right sprite based on the movements of the enemy
				if (Movements == Movements.RIGHT)
				{
					velocity.X += Speed;
				
					_walkright.Visible = true;
					_walkleft.Visible = false;
					_idleSprite.Visible = false;
				}
				else if (Movements == Movements.LEFT)
				{
					velocity.X -= Speed;

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
			}
			else
			{
				if (aggrosource.Position.X < this.Position.X)
				{
					Movements = Movements.LEFT;
					velocity.X -= Speed * 2;
				}
				if (aggrosource.Position.X > this.Position.X)
				{
					Movements = Movements.RIGHT;
					velocity.X += Speed * 2;
				}

				if (Math.Abs(aggrosource.Position.X - this.Position.X) < 20)
				{
					if (Math.Abs(aggrosource.Position.Y - this.Position.Y) < 60)
					{
						aggrosource.Hurt(Damage,this);
						Death();
					}				
				}
			}


			//play the sprite
			if (_idleSprite.Visible) _idleSprite.Play();

			if (_walkleft.Visible) _walkleft.Play();

			if (_walkright.Visible) _walkright.Play();

			Velocity = velocity;
			MoveAndSlide();

		}

		//death
		public override void Hurt(int hpLoss,OutofTheHole.Entity.Entity source)
		{
			aggrosource = source;
			Hp -= hpLoss;
			if (Hp <= 0)
			{
				Death();
			}
		}

	}
}
