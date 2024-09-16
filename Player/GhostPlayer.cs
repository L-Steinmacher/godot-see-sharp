using Godot;
using System;

public partial class GhostPlayer : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("fade_out");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void SetHValue(bool val)
    {
        GetNode<Sprite2D>("Sprite2D").FlipH = val;
    }

    public void Destroy()
    {
        QueueFree();
    }
}
