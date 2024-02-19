using Godot;
using OutOfTheHole.Bullet;

namespace OutofTheHole.Gun;

public partial class Gun : Node2D
{
	public int Id;

	private float _timeUntilFire;

	//arbitrary values for the ability to shoot

	[Export] private PackedScene _bulletScene;
	
	public float FireRate;
	
	/// <summary>
	/// Initiate the fire_rate value when the player spawn;
	/// </summary>
	public override void _Ready()
	{
		GD.Print(Name);
		GetParent().GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(Id);
		GD.Print(GetParent().GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer"));
	}

	/// <summary>
	/// The function manages all the bullet shooting. It creates the bullet, sets the rotation and position and adds it to
	/// the scene.
	/// The velocity management has been offloaded to the bullet.
	/// </summary>
	/// <param name="gunNode">The node corresponding to the gun</param>
	/// <param name="shootPoint">The shoot point Node included int the Gun</param>
	/// <param name="sceneTree">The global scene tree so the bullet can be added.</param>
	public void FireBullet(Node2D gunNode, Node2D shootPoint, SceneTree sceneTree,string type)
	{
		if (type == "Basic")
		{
			//arbitrary value for Basic gun
			FireRate = 1 / 5f;
		}
		var bullet = _bulletScene.Instantiate<BasicBullet>(); // create the bullet
		bullet.Rotation = gunNode.Rotation;
		bullet.GlobalPosition = shootPoint.GlobalPosition;
		sceneTree.Root.AddChild(bullet);
	}
}
