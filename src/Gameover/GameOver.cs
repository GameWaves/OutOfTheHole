using Godot;

namespace OutofTheHole.Gameover;

public partial class GameOver : Node2D
{
	[Export] private PackedScene _fallbackscene;
	
	/// <summary>
	/// function that is called when a button is pressed (currently the retry button)
	/// </summary>
	private void _on_button_pressed()
    {
        // change the scene to the selected fallback scene
        GetTree().ChangeSceneToPacked(_fallbackscene);
        //GetTree().ChangeSceneToFile("res://Map.tscn");
    }

	/// <summary>
	/// function that is called when a button is pressed (currently the leave button)
	/// </summary>
	private void _on_button_2_pressed()
    {
        //close the game
        GetTree().Quit();
    }
}