using System;
using Godot;

public partial class OptionMenu : CanvasLayer
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    /// <summary>
    ///     Called when the keymap button is pressed
    /// </summary>
    private void _on_keymap_button_button_down()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Called when the volumes button is pressed
    /// </summary>
    private void _on_volumes_button_button_down()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Called when the exit button is pressed
    /// </summary>
    private void _on_exit_button_button_down()
    {
        GetTree().ChangeSceneToFile("res://src/menus/MainMenu.tscn");
    }
}