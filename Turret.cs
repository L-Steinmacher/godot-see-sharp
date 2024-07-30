using Godot;
using System;

public partial class Turret : RigidBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void _on_detection_radious_body_entered(Node2D body) {
        GD.Print("body: " + body.Name + " has entered the detection radious");
    }

    private void _on_detection_radious_body_exited(Node2D body) {
        GD.Print("body: " + body.Name + " has exited the detection radious");
    }
}
