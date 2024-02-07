using Godot;
using OutOfTheHole.Gun;

namespace OutofTheHole.Gun;

public partial class Gun : Node2D
{
	public int Id;
	[Export] private float _bps = 5f;
	[Export] private float _bulletDamage = 30f;
	[Export] private float _bulletSpeed = 800f;

	private float _timeUntilFire = 300f;

	//arbitrary values for the ability to shoot
	[Export] private PackedScene _bulletScn;

	public float FireRate;

	/// <summary>
	///     Initiate the fire_rate value when the player spawn;
	/// </summary>
	public override void _Ready()
	{
		FireRate = 1 / _bps;
		GD.Print(Name);
		GetParent().GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(Id);
		GD.Print(GetParent().GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer"));
	}

	/// <summary>
	///     The function manages all the bullet shooting. It creates the bullet, sets the rotation and position and adds it to
	///     the scene.
	///     The velocity management has been offloaded to the bullet.
	/// </summary>
	/// <param name="gunNode">The node corresponding to the gun</param>
	/// <param name="shootPoint">The shoot point Node included int the Gun</param>
	/// <param name="sceneTree">The global scene tree so the bullet can be added.</param>
	public void FireBullet(Node2D gunNode, Node2D shootPoint, SceneTree sceneTree)
	{
		var bullet = _bulletScn.Instantiate<BasicBullet>(); // create the bullet
		bullet.Speed = _bulletSpeed;
		bullet.Rotation = Rotation;
		bullet.GlobalPosition = shootPoint.GlobalPosition;
		sceneTree.Root.AddChild(bullet);
	}
}
