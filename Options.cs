using Godot;
using System;

public partial class Options : Control
{
    public void _on_back_pressed () {
        GD.Print("Back pressed");
        GetTree().ChangeSceneToFile("res://MainMenu.tscn");
    }
}
