using System;
using Godot;

namespace OutOfTheHole.Bullet;

public partial class BasicBullet : Bullets
{
	
	/// <summary>
	///     Initialize the default longevity and compute the direction.
	/// </summary>
	public override void _Ready()
	{
		Damage = 30;
		Speed = 800.0f;
		Longetivity = 100;
	}

	public override void OnHit()
	{
		// do something on hit 
	}
}
