using Godot;
using System;

public partial class MainMenu : Control
{
    public void _on_start_pressed () {
        GD.Print("start pressed.");
        GetTree().ChangeSceneToFile("res://playerTestScene.tscn");

    }

    public void _on_options_pressed () {
        GD.Print("options");
        GetTree().ChangeSceneToFile("res://Options.tscn");
    }

    public void _on_quit_pressed () {
        GD.Print("quit pressed");
        GetTree().Quit();
    }
}
