using Godot;
using System;
using OutofTheHole.Entity;
using OutofTheHole.Entity.Players;
using OutofTheHole.Multiplayer;

public partial class Boss : Entity
{

	[Export] private PackedScene _bossProjectileScene;

	public bool awaken = false;
	public Player P1 = GameManager.Players[0].Player;
	public Player P2 = GameManager.Players[1].Player;
	public int cowldown = 50;
	public int tier;
	public int MaxHp = 1000;

	public override void Hurt(int hpLoss, Entity source)
	{
		awaken = true;
		Hp -= hpLoss;
		if (Hp <= 0)
		{
			QueueFree();
			if (IsQueuedForDeletion()) Free();
		}
	}

	public override void _Ready()
	{
		Hp = MaxHp;
		if (P1.Reversed)
		{
			(P1, P2) = (P2, P1);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (awaken)
		{
			if (Multiplayer.GetUniqueId() == 1)
			{
				if (P1.IsOnFloor() || P2.IsOnCeiling())
				{
					cowldown -= 1;
				}

				if (cowldown <= 0)
				{
					if (tier <= 1)
					{
						Boss_projectile bossProjectile;
						bossProjectile = _bossProjectileScene.Instantiate<Boss_projectile>();
						if (P1.IsOnFloor())
						{
							bossProjectile.Reversed = true; 
						}
						else
						{
							bossProjectile.Reversed = false;
						}
						
						AddChild(bossProjectile);
					}

					if (tier == 2)
					{
						if (P1.IsOnFloor() && P2.IsOnCeiling())
						{
							if (P1.Hp < P2.Hp)
							{
								Boss_projectile boss_projectile = _bossProjectileScene.Instantiate<Boss_projectile>();
								boss_projectile.Reversed = false;
								AddChild(boss_projectile);
							}
							else
							{
								Boss_projectile boss_projectile = _bossProjectileScene.Instantiate<Boss_projectile>();
								boss_projectile.Reversed = true;
								AddChild(boss_projectile);
							}
						}
						else if (P1.IsOnFloor())
						{
							Boss_projectile boss_projectile = _bossProjectileScene.Instantiate<Boss_projectile>();
							boss_projectile.Reversed = false;
							AddChild(boss_projectile);
						}
						else
						{
							Boss_projectile boss_projectile = _bossProjectileScene.Instantiate<Boss_projectile>();
							boss_projectile.Reversed = true;
							AddChild(boss_projectile);
						}

					}

					else
					{
						if (P1.IsOnFloor() && P2.IsOnCeiling())
						{

								Boss_projectile boss_projectile1 = _bossProjectileScene.Instantiate<Boss_projectile>();
								boss_projectile1.Reversed = false;
								AddChild(boss_projectile1);

								Boss_projectile boss_projectile2 = _bossProjectileScene.Instantiate<Boss_projectile>();
								boss_projectile2.Reversed = true;
								AddChild(boss_projectile2);
						}
						if (P1.IsOnFloor())
						{
							Boss_projectile boss_projectile = _bossProjectileScene.Instantiate<Boss_projectile>();
							boss_projectile.Reversed = false;
							AddChild(boss_projectile);
						}
						else
						{
							Boss_projectile boss_projectile = _bossProjectileScene.Instantiate<Boss_projectile>();
							boss_projectile.Reversed = true;
							AddChild(boss_projectile);
						}
					}

					cowldown = 50;
				}
			}

		}
	}

}
