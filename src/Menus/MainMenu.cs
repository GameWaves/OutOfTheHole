using Godot;

namespace OutofTheHole.Menus;

public partial class MainMenu : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<CanvasLayer>("OptionsMenu").Visible = false;
		GetNode<Button>("MenuMarginContainer/MenuVBoxContainer/PlayButton").GrabFocus();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// Function called when the play button is pressed (show the multiplayer menu)
	/// </summary>
	private void _on_play_button_button_down()
	{
		GetTree().ChangeSceneToFile("res://src/Multiplayer/multiplayer_controler.tscn");
	}

	/// <summary>
	/// Function called when the option button is pressed (show the option menu)
	/// </summary>
	
	private void _on_option_button_button_down()
	{
		GetNode<MarginContainer>("MenuMarginContainer").Visible = false;
		GetNode<CanvasLayer>("OptionsMenu").Visible = true;
		GetNode<Button>("OptionsMenu/OptionsButtons/Volume").GrabFocus();
	}
	
	/// <summary>
	/// Function called when the credits button is pressed (show the credits)
	/// </summary>
	private void _on_credits_button_button_down()
	{
		GetTree().ChangeSceneToFile("res://src/Menus/CreditsMenu.tscn");
	}

	/// <summary>
	/// Function called when the exit button is pressed (quit the game)
	/// </summary>
	private void _on_exit_button_button_down()
	{
		GetTree().Quit();
	}

	private void _on_back_button_down()
	{
		GetNode<MarginContainer>("MenuMarginContainer").Visible = true;
		_Ready();
	}

	private void _on_keymaps_button_down()
	{
		GetNode<Control>("KeymapMenu/InputSettings").Visible = true;
	}
}
