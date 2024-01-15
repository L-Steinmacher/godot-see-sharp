using Godot;
using System;

public partial class MainMenu : Control
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Grab focus for the start button
        GetNode<Button>("VBoxContainer/StartButton").GrabFocus();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void _on_start_button_pressed()
    {
        GetTree().ChangeSceneToFile("res://playerTestScene.tscn");
    }

    public void _on_options_button_pressed()
    {
        GD.Print("Options button pressed");
    }

    public void _on_quit_button_pressed()
    {
        GetTree().Quit();
    }
}
