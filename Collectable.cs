using Godot;
using System;

public abstract partial class Collectable : Interactable
{


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
    public void InteractWithObject(Node obj)
    {
        GD.Print("Potion collected in Collectable Class: " + obj.Name);
    }
}
