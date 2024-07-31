using Godot;
using System;

public partial class Projectile : Node2D
{
    private int damageDealtAmmount = 1;
    public string ResourcePath = "res://Enemies/Turret/Projectile.tscn";
    public int speed = 150;
    private double lifespan = 5;
    public Vector2 velocity { get; set; }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        lifespan -= delta;
        // var t = Transform2D.Identity;
        // Position += t.X * (float)(speed * delta);
        Position += velocity * (float)delta;
        if (lifespan <= 0)
        {
            QueueFree();
        }
    }

    public string LoadResourcePath()
    {
        return ResourcePath;
    }

    private void _on_area_2d_body_entered(Node2D body)
    {

        if (body is CharacterBody2D)
        {
            if (body is PlayerController)
            {
                PlayerController pc = body as PlayerController;
                pc.TakeDamage(damageDealtAmmount);
            }
            QueueFree();
        }
    }
}
