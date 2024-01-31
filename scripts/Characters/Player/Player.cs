namespace OutOfTheHole.scripts.Characters.Player;

public partial class Player : Character
{
    public Player(string name, float speed, int hp, float jumpVelocity, bool inverted) : base(name, speed, hp,
        jumpVelocity, inverted)
    {
    }

    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
    }
}