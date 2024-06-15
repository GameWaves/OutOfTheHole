using Godot;

namespace OutofTheHole.Entity;

public abstract partial class Entity : CharacterBody2D
{
	//Set a acceleration speed
	public float Acceleration;

	protected AnimatedSprite2D _idleSprite;

	protected AnimatedSprite2D _walkleft;

	protected AnimatedSprite2D _walkright;
	
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
	public abstract void Hurt(int hpLoss,Entity source);

	public bool IsInvicible { get; set; } = false;
	public int invicibleTime { get; protected set; } = 0;

	public virtual void Death()
	{
		Alive = false;
		QueueFree();
		if (IsQueuedForDeletion()) Free();
	}
}
