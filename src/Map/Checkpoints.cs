using Godot;
using System;
using OutofTheHole.Entity;
using OutofTheHole.Entity.Players;

public partial class Checkpoints : Node2D
{
    public override void _Ready()
    {
        if (int.Parse(Name) % 2 == 0)
        {
            GetNode<Sprite2D>("Sprite").Visible = false;
        }
        else
        {
            GetNode<Sprite2D>("SpriteReversed").Visible = false;
        }
    }
    private void _on_checkpoints_entered(Node2D player)
    {
        GD.Print("entered checkpoint");
        if (player is Player)
        {
            if (int.Parse(player.Name) == 1 && int.Parse(Name) % 2 != 0)
            {
                GD.Print($"player {player.Name} pass the checkpoint");
                ((Player)player).Spawn = this.Position;
            }
            else if (int.Parse(player.Name) != 1 && int.Parse(Name) % 2 == 0)
            {
                GD.Print($"player {player.Name} pass the checkpoint");
                ((Player)player).Spawn = this.Position;
            }
        }
    }
}
