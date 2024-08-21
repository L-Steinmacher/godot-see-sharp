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
    public const float Speed = 2.0f;

    private AnimatedSprite2D _animatedSprite2D;
    private float _timeElapsed = 0.0f;
    private float _figure8Width = 50.0f;
    private float _figure8Height = 25.0f;

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
                velocity = ProcessPatrolling(delta);
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

        Vector2 ProcessPatrolling(double delta)
        {
            Vector2 velocity;
            _timeElapsed += (float)delta;
            float x = _figure8Width * Mathf.Sin(_timeElapsed);
            float y = _figure8Height * Mathf.Cos(_timeElapsed * 2);

            velocity = new Vector2(x, y);
            return velocity;
        }
    }


    public override void TakeDamage(int damageTakenAmount)
    {
        EnemyState previousState = CurrentState;
        CurrentState = EnemyState.TakingDamage;

        Health -= damageTakenAmount;

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
