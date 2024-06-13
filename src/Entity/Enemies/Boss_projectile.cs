using Godot;
using System;
using OutofTheHole.Entity;
using OutOfTheHole.Entity.Enemies;
using OutofTheHole.Entity.Players;

public partial class Boss_projectile : OutofTheHole.Entity.Entity
{
	public int MaxHp = 100;
	public bool Reversed;
	public float Gravity;
	public float Speed = 10f;
	public bool candamage = true;
	public override void Hurt(int hpLoss, Entity source)
	{
	}

	public void Remove()
	{
		QueueFree();
		if (IsQueuedForDeletion()) Free();
	}

	public override void _Ready()
	{
		Hp = MaxHp;
		Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	}

	public override void _PhysicsProcess(double delta)
	{		
		if (Multiplayer.GetUniqueId() == 1)
		{
			RotationDegrees -= 10;
				Vector2 velocity = Velocity;
				
				if (Reversed)
				{
					if (!IsOnCeiling()) velocity.Y -= Gravity * (float)delta;
				}
				else
				{
					if (!IsOnFloor()) velocity.Y += Gravity * (float)delta;
				}


				velocity.X -= Speed;

				if (candamage)
				{
					for (int i = 0; i < GetSlideCollisionCount(); i++)
					{
						var c = GetSlideCollision(i).GetCollider();
						if (c is Player)
						{
							candamage = false;
							Hurt(10,c as Entity);
							Remove();
						}

						if (c is Enemy)
						{
							(c as Enemy).Death();
						}
					}
				}

				
				Velocity = velocity;
				MoveAndSlide();

				Hp -= 1;
				if (Hp <= 0)
				{
					Remove();
				}
				
			}

	}

}
