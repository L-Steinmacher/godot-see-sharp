using Godot;
using System;

public partial class MainMenu : Control
{
    public void _on_start_pressed () {
        GetTree().ChangeSceneToFile("res://playerTestScene.tscn");

    }

    public void _on_options_pressed () {
        GetTree().ChangeSceneToFile("res://Options.tscn");
    }

    public void _on_quit_pressed () {
        GetTree().Quit();
    }
}
