using Godot;
using System;

public partial class Battie : Enemy
{
    public enum EnemyState
    {
        Idle,
        Patrolling,
        Chasing,
        Attacking,
        TakingDamage,
    }

    public EnemyState CurrentState { get; set; } = EnemyState.Patrolling;
    public const float Speed = 300.0f;
    public const float JumpVelocity = -400.0f;

    private AnimatedSprite2D _animatedSprite2D;

    public override void _Ready()
    {
        Health = 2;
        DamageDealtAmount = 1;
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        _animatedSprite2D.Play("Idle");
        Vector2 velocity = Velocity;

        switch (CurrentState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Patrolling:
                ProcessPatrolling(delta);
                break;
            case EnemyState.TakingDamage:
                break;
            case EnemyState.Chasing:
                break;
            case EnemyState.Attacking:
                break;
            default:
                break;
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public void ProcessPatrolling(double delta)
    {
        // Patrolling logic to be implemented
    }

    public override void TakeDamage(int damageTakenAmount)
    {
        EnemyState previousState = CurrentState;
        CurrentState = EnemyState.TakingDamage;

        Health -= damageTakenAmount; // Use the passed argument

        if (Health <= 0)
        {
            _animatedSprite2D.Play("Death");
        }
        else
        {
            CurrentState = previousState;
        }
    }

    private void _on_animated_sprite_2d_animation_finished()
    {
        if (_animatedSprite2D.Animation == "Death")
        {
            QueueFree();
        }
    }
}
