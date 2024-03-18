using Godot;
using System;

public partial class optionsMenu : Control
{
    public void _Ready()
    {
        BoxContainer opButtons = GetNode<BoxContainer>("BoxContainer");
        Button vol = new Button();
        vol.Text = "Volume";
        vol.Theme = GetNode<Theme>("res://assets/Menu_theme.tres");
        opButtons.AddChild(vol);
        Button keymap = new Button();
        keymap.Text = "Keymap";
        keymap.Theme = GetNode<Theme>("res://assets/Menu_theme.tres");
        opButtons.AddChild(keymap);
        vol.GrabFocus();
    }
}
