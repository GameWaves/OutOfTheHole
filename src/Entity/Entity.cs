using Godot;

namespace OutofTheHole.Entity;

public abstract partial class Entity : CharacterBody2D
{
	//Set a acceleration speed
	public float Acceleration;

	//define if the Entity is Alive
	public bool Alive;

	//Set a Sprite
	public AnimatedSprite2D AnimatedSprite2D;
	
	//to define the hp of different Entity
	public int Hp;

	//set maxHp of player
	public int MaxHp;

	//set a initial speed 
	public float Speed;

	//to define the loss of Hp
	//hpLoss is the amount of damage 
	public abstract void Hurt(int hpLoss);

	public void Death()
	{
		QueueFree();
		if (IsQueuedForDeletion()) Free();
	}
}
