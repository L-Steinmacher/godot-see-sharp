using Godot;
using System;

public partial class Projectile : Node2D
{
    private int damageDealtAmmount = 1;
    public string ResourcePath = "res://Enemies/Turret/Projectile.tscn";
    public int speed = 150;
    private double lifespan = 3;
    public Vector2 velocity { get; set; }
    public Turret shooter;

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
        lifespan -= delta;
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
        if (body == shooter) return;
        if (body is CharacterBody2D)
        {
            if (body is Enemy)
            {
                Enemy e = body as Enemy;
                e.TakeDamage(damageDealtAmmount);
            }
            if (body is PlayerController)
            {
                PlayerController pc = body as PlayerController;
                pc.TakeDamage(damageDealtAmmount);
            }
            QueueFree();
        }
        if (body is TileMapLayer)
        {
            QueueFree();
        }
    }
}
