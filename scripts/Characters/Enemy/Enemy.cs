using System;
using Godot;

namespace OutOfTheHole.scripts.Characters.Enemy;

public partial class Enemy : Character
{
	public Enemy(string name, float speed, int hp, float jumpVelocity, bool inverted) : base(name, speed, hp,
		jumpVelocity, inverted)
	{
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
	}

	private double GetDistanceWithCharacter(Character character)
	{
		return Math.Sqrt(Math.Pow(character.Position.X - Position.X, 2) +
						 Math.Pow(character.Position.Y - Position.Y, 2));
	}

	public Character GetClosestPlayer(Character[] charactersList)
	{
		// TODO: Should characterList be implemented in multiplayer?

		Character closestPlayer = charactersList[0];
		double closestDistance = GetDistanceWithCharacter(charactersList[0]);

		for (var i = 1; i < charactersList.Length; i++)
		{
			double length = GetDistanceWithCharacter(charactersList[i]);
			if (length < closestDistance)
				(closestPlayer, closestDistance) = (charactersList[i], length);
		}

		return closestPlayer;
	}
}
