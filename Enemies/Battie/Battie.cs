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
        Dead,
    }

    public EnemyState CurrentState { get; set; }
    public const float Speed = 25.0f;
    private const float AttackSpeed = 110.0f;

    private AnimatedSprite2D _animatedSprite2D;
    private float _frequency = 2.0f;
    private float _timeElapsed = 0.0f;
    private float _figure8Width = 50.0f;
    private float _figure8Height = 25.0f;
    private PlayerController player;

    public override void _Ready()
    {
        Health = 2;
        DamageDealtAmount = 1;
        CurrentState = EnemyState.Patrolling;
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        switch (CurrentState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Patrolling:
                _animatedSprite2D.Play("Idle");
                velocity = ProcessPatrolling(delta);
                break;
            case EnemyState.TakingDamage:
                break;
            case EnemyState.Chasing:
                velocity = ProcessChasing();
                break;
            case EnemyState.Attacking:
                break;
            case EnemyState.Dead:
                _animatedSprite2D.Play("Death");
                break;
            default:
                break;
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    private Vector2 ProcessPatrolling(double delta)
    {
        Vector2 velocity;
        _timeElapsed += (float)delta;
        float x = _figure8Width * Mathf.Sin(_timeElapsed);
        float y = _figure8Height * Mathf.Cos(_timeElapsed * _frequency);

        velocity = new Vector2(x, y);
        return velocity;
    }

    private Vector2 ProcessChasing()
    {
        Vector2 velocity;
        player = GameManager.Player;
        var playerPosition = player.Position;
        velocity = Position.DirectionTo(playerPosition) * Speed;
        return velocity;
    }

    public override void TakeDamage(int damageTakenAmount)
    {
        Health -= damageTakenAmount;
        // CurrentState = EnemyState.TakingDamage;
        Velocity = new Vector2(0, 0);
        _animatedSprite2D.Play("TakeDamage");
        if (Health <= 0)
        {
            CurrentState = EnemyState.Dead;
        }
    }

    private void _on_detection_radius_body_entered(Node2D body)
    {
        if (body is PlayerController)
        {
            PlayerController pc = body as PlayerController;
            CurrentState = EnemyState.Chasing;
        }
    }

    private void _on_detection_radius_body_exited(Node2D body)
    {
        if (body is PlayerController)
        {
            CurrentState = EnemyState.Patrolling;
        }
    }

    private void _on_animated_sprite_2d_animation_finished()
    {
        if (_animatedSprite2D.Animation == "Death")
        {
            QueueFree();
        }
        if (_animatedSprite2D.Animation == "TakeDamage")
        {
            CurrentState = EnemyState.Patrolling;
        }
    }
}
