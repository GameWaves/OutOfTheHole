using System;
using Godot;
using OutOfTheHole.Entity.Enemies;

namespace OutOfTheHole.Bullet;

public partial class BasicBullet : Bullets
{
	
	/// <summary>
	/// Initialize the default longevity and compute the direction.
	/// </summary>
	public override void _Ready()
	{
		Damage = 30;
		Speed = 800.0f;
		Longetivity = 100;
	}

	public override void OnHit(Node2D target)
	{
		if (target is OutofTheHole.Entity.Entity entity)
		{
			// GD.Print($"Hit from {source.Name} to {entity.Name}");
			if (entity != Source)
			{
				entity.Hurt(Damage,Source);
			}
		}

	}
}

