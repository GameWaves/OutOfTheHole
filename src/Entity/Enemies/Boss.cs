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
	public int MaxHp = 2000;

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
			if (MaxHp > Hp*2 )
			{
				tier = 3;
			}
			else if (MaxHp*3 > Hp*4)
			{
				tier = 2;
			}
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
						var rdm = new Random();
						
						bool reversed;
						if (rdm.Next() % 2 == 0)
						{
							reversed = true; 
						}
						else
						{ 
							reversed = false;
						}
						
						SpawnProjectile(reversed);
					}

					if (tier == 2)
					{
						var rdm1 = new Random();
						if (rdm1.Next() %2 == 0)
						{
							SpawnProjectile(false);
							SpawnProjectile(true);
						}
						else
						{
							bool reversed;

							var rdm2 = new Random();
							if (rdm2.Next() % 2 == 0)
							{
								reversed = true; 
							}
							else
							{ 
								reversed = false;
							}
							SpawnProjectile(reversed);
						}


					}

					else
					{
						SpawnProjectile(false);
						SpawnProjectile(true);

					}
					cowldown = 50;
				}
			}
		}
	}
	
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void SpawnProjectile(bool reversed)
	{
		
		//GD.Print($"Spawned BossProjectile for {Multiplayer.GetUniqueId()} Peer : {(Multiplayer.GetPeers()[0])}");
		BossProjectile bossProjectile = _bossProjectileScene.Instantiate<BossProjectile>();
		bossProjectile.Reversed = reversed;
		AddChild(bossProjectile);
		
		// Here we pass the message to the other peer (Player 2) that it should instantiate the Projectile.
		if (Multiplayer.GetUniqueId() == 1)
			RpcId(Multiplayer.GetPeers()[0], "SpawnProjectile", reversed);
	}
}
