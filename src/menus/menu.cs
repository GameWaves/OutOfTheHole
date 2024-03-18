using Godot;
using System;
using System.Reflection.Metadata;

public partial class menu : Control
{
    private int separation = 8;
    public override void _Ready()
    {
        
        GetNode<VBoxContainer>("OpButtons").Visible = false;
        GetNode<TextureRect>("OpContainerTexture").Visible = false;
        GetNode<Button>("Buttons/Play").GrabFocus();
    }
    public void _on_play_pressed()
    {
        GetTree().ChangeSceneToFile("res://src/multiplayer/multiplayer_controler.tscn");
    }

    public void _on_options_pressed()
    {
        GetNode<VBoxContainer>("Buttons").Visible = false;
        GetNode<VBoxContainer>("OpButtons").Visible = true;
        GetNode<TextureRect>("OpContainerTexture").Visible = true;
        GetNode<Button>("OpButtons/Volume").GrabFocus();
    }

    public void _on_quit_pressed()
    {
        GetTree().Quit();
    }

    public void _on_popup_menu_popup_hide()
    {
        GetNode<VBoxContainer>("Buttons").Visible = true;
        GetNode<Button>("Buttons/Play").GrabFocus();
    }

    public void _on_op_buttons_hidden()
    {
        GetNode<VBoxContainer>("Buttons").Visible = true;
        GetNode<Button>("Buttons/Play").GrabFocus();
    }

    public void _on_back_pressed()
    {
        _Ready();
        GetNode<VBoxContainer>("Buttons").Visible = true;
    }
}
