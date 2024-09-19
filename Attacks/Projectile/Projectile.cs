using Godot;
using System;

public partial class Projectile : Spell
{
    private int damageDealtAmmount = 1;
    private string ResourcePath = "res://Attacks/Projectile/Projectile.tscn";
    public int speed = 150;
    private double lifespan = 3;
    public Vector2 velocity { get; set; }
    public CharacterBody2D shooter;
    private PlayerController player;

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

    public override string LoadResourcePath()
    {
        return ResourcePath;
    }

    public override void CastSpell(bool faceDirection)
    {
        Vector2 direction;
        direction = faceDirection ? new Vector2(-1, 0) : new Vector2(1, 0);
        velocity = direction * speed;
        GD.Print("pew pew");
    }

    public override void SetUp(bool faceDirection)
    {
        throw new NotImplementedException();
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
    private void _on_visible_on_screen_notifier_2d_screen_exited()
    {
        QueueFree();
    }
}

