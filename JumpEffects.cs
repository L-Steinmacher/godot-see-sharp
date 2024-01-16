using Godot;
using System;

public partial class JumpEffects : Node
{
    private AnimatedSprite2D animatedSprite2D;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite2D.Play("LiftoffDust");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
    public void _on_animated_sprite_2d_animation_finished()
    {
        if (animatedSprite2D.Animation == "LiftoffDust")
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        QueueFree();
    }
}
