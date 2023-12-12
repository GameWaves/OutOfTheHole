using Godot;
using System;

public partial class Label : Godot.Label
{
	// Appele cette méthode lorsque modification du texte, la police et la couleur
	public void UpdateGameOverLabel()
	{
		// Accéde au nœud Label
		Label gameOverLabel = GetNode<Label>("GameOverLabel");

		// Modifie le texte
		gameOverLabel.Text = "GAME OVER";

		// Modifie le style de police
		DynamicFont font = new DynamicFont();
		font.FontData = GD.Load<DynamicFontData>("res://chemin/vers/votre/police.tres");
		gameOverLabel.AddFontOverride("font", font);

		// Modifiez la taille de police
		gameOverLabel.RectMinSize = new Vector2(200, 100);  // Ajuste

		// Modifiez la couleur du texte
		gameOverLabel.Modulate = new Color(1, 0, 0, 1);  // Rouge, ajuste les valeurs RGBA selon vos besoins
	}
}


