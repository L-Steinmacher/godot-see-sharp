using Godot;
using System;

public partial class SlimeEnimy : Node2D
{
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        sprite = GetNode<Sprite2D>("Sprite2D");
        animationPlayer.Play("Idle");
    }

    public void _on_area_2d_body_entered(Node body)
    {
        if (body is CharacterBody2D)
        {
            if (body is PlayerController)
            {
                PlayerController pc = body as PlayerController;
                pc.TakeDamage(1);
            }
        }
    }

    public void _on_area_2d_body_exited(Node body)
    {
        if (body is CharacterBody2D)
        {
            if (body is PlayerController)
            {
                animationPlayer.Play("Idle");
                GD.Print("Player has left");
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
