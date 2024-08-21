using Godot;
using System;

public partial class Battie : Enemy
{
    public const float Speed = 300.0f;
    public const float JumpVelocity = -400.0f;
    private AnimatedSprite2D animatedSprite2D;

    public override void _Ready()
    {
        Health = 2;
        DamageDealtAmount = 1;
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        // Implement all this shit
        Velocity = velocity;
        MoveAndSlide();
    }

    public override void TakeDamage(int DamageTakenAmount)
    {
        Health -= DamageDealtAmount;
        if (Health <= 0)
        {
            animatedSprite2D.Play("Death");

        }
    }

    private void _on_animated_sprite_2d_animation_finished()
    {
        if (animatedSprite2D.Animation == "Death")
        {
            QueueFree();
        }
    }
}
