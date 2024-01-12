using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class PlayerController : CharacterBody2D
{
    public enum PlayerState
    {
        Idle,
        Running,
        Falling,
        Dashing,
        WallJumping,
        Attacking,
        TakingDamage,
    }

    public PlayerState CurrentState = PlayerState.Idle;
    public Vector2 facingDirection = new Vector2(0, 0);
    private Vector2 velocity = new Vector2();
    public const float Speed = 300.0f;
    public const float JumpVelocity = -400.0f;
    public const float WallJumpVerticalVelocity = -300.0f;
    private const float Friction = 0.3f;
    private const float Acceleration = 0.2f;
    private const float DashSpeed = 800.0f;
    public const float DashGravity = 0.0f;
    private bool isDashing = false;
    private bool canDash = true;
    private double dashTimer = .2;
    private double dashTimeReset = .2;
    private double dashCooldownTimer = 1;
    private double dashCooldownTimeReset = 1;
    private double damageTimer = .2;
    private double damageTimerReset = .2;
    public bool isAttacking = false;
    private bool isWallJumping = false;
    private double wallJumpTimer = .3;
    private double wallJumpTimeReset = .3;
    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private AnimatedSprite2D animatedSprite2D;
    public int Health = 3;
    private bool isTakingDammage = false;
    [Signal]
    public delegate void DeathEventHandler();
    [Export]
    public PackedScene GhostPlayerInstance;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (CurrentState != PlayerState.TakingDamage)
        {
            InputManager(delta);
        }
        switch (CurrentState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Running:
                break;
            case PlayerState.Dashing:
                ProcessDash(velocity);
                break;
            case PlayerState.WallJumping:
                break;
            case PlayerState.Attacking:
                break;
            case PlayerState.TakingDamage:
                damageTimer -= delta;
                if (damageTimer <= 0)
                {
                    CurrentState = PlayerState.Idle;
                    damageTimer = damageTimerReset;
                    isTakingDammage = false;
                }
                break;
            default:
                break;
        }
        ProcessTimers(delta);
    }

    private void InputManager(double delta)
    {
        if (Health > 0)
        {
            velocity = Velocity;
            // Add the gravity.
            if (!IsOnFloor())
            {
                velocity.Y += gravity * (float)delta;
            }

            if (IsOnFloor())
                if (!isDashing && dashCooldownTimer <= 0)
                {
                    canDash = true;
                    dashCooldownTimer = dashCooldownTimeReset;
                }
            {
                canDash = true;
            }
            if (velocity.X > 0)
            {
                animatedSprite2D.FlipH = false;
            }
            else if (velocity.X < 0)
            {
                animatedSprite2D.FlipH = true;
            }

            if (Input.IsActionJustPressed("jump"))
            {
                velocity = ProcessJump(velocity);
            }
            if (Input.IsActionJustPressed("dash"))
            {
                CurrentState = PlayerState.Dashing;
                // velocity = ProcessDash(delta, velocity, facingDirection);
            }
            facingDirection = ProcessMovement(ref velocity);

            Velocity = velocity;
            MoveAndSlide();
        }
    }

    private void ProcessTimers(double delta)
    {
        if (isTakingDammage)
        {
            damageTimer -= delta;
            if (damageTimer <= 0)
            {
                CurrentState = PlayerState.Idle;
                damageTimer = damageTimerReset;
                isTakingDammage = false;
            }
        }
        if (isWallJumping)
        {
            wallJumpTimer -= delta;
            if (wallJumpTimer <= 0)
            {
                isWallJumping = false;
                wallJumpTimer = wallJumpTimeReset;
            }
        }
        if (CurrentState == PlayerState.Dashing)
        {
            GD.Print("dashTimer: " + dashTimer);
            dashTimer -= delta;
            dashCooldownTimer -= delta;
            GhostPlayer ghostPlayer = GhostPlayerInstance.Instantiate() as GhostPlayer;
            Owner.AddChild(ghostPlayer);
            ghostPlayer.GlobalPosition = this.GlobalPosition;
            ghostPlayer.SetHValue(animatedSprite2D.FlipH);

            if (dashTimer <= 0)
            {
                isDashing = false;
                velocity = new Vector2(0, 0);
                velocity.Y = gravity;
                CurrentState = PlayerState.Idle;
            }
        }
    }
    private Vector2 ProcessMovement(ref Vector2 velocity)
    {
        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("move_left", "move_right", "climb_up", "climb_down");
        if (!isTakingDammage)
        {
            if (Input.IsActionJustPressed("dash"))
            {
                CurrentState = PlayerState.Dashing;
            }
            if (direction.X < 0)
            {
                facingDirection = new Vector2(-1, 0);
                animatedSprite2D.FlipH = true;
            }
            else if (direction.X > 0)
            {
                facingDirection = new Vector2(1, 0);
                animatedSprite2D.FlipH = false;
            }
            if (IsOnFloor())
            {
                if (direction != Vector2.Zero)
                {
                    animatedSprite2D.Play("Run");
                    velocity.X = Mathf.Lerp(velocity.X, direction.X * Speed, Acceleration);
                }
                else
                {
                    animatedSprite2D.Play("Idle");
                    velocity.X = Mathf.Lerp(Velocity.X, 0, Friction);
                }
            }
        }


        if (velocity.X < 10 && velocity.X > -10)
        {
            isTakingDammage = false;
        }
        return direction;
    }

    private void ProcessDash(Vector2 velocity)
    {

        var direction = animatedSprite2D.FlipH ? -1 : 1;
        if (canDash && !isDashing)
        {
            dashTimer = dashTimeReset;
            isDashing = true;
            canDash = false;
            Velocity = new Vector2(DashSpeed * direction, 0);
        }
    }

    private Vector2 ProcessJump(Vector2 velocity)
    {

        if (!IsOnFloor())
        {
            if (velocity.Y < 0)
                animatedSprite2D.Play("Jump");
            else
                animatedSprite2D.Play("Fall");
        }

        velocity.Y = JumpVelocity;

        if (!IsOnFloor())
        {
            if (!isWallJumping)
            {
                if (Input.IsActionJustPressed("jump") && GetNode<RayCast2D>("LeftRayCast2D").IsColliding())
                {
                    animatedSprite2D.FlipH = false;
                    velocity.Y = WallJumpVerticalVelocity;
                    velocity.X = -JumpVelocity;
                    isWallJumping = true;
                }
                if (Input.IsActionJustPressed("jump") && GetNode<RayCast2D>("RightRayCast2D").IsColliding())
                {
                    animatedSprite2D.FlipH = true;
                    velocity.Y = WallJumpVerticalVelocity;
                    velocity.X = JumpVelocity;
                    isWallJumping = true;
                }
            }
        }
        return velocity;
    }
    public void TakeDamage(int damage)
    {
        var currentDirection = animatedSprite2D.FlipH ? -1 : 1;
        Velocity = new Vector2(-200 * currentDirection, -100);
        animatedSprite2D.Play("TakeDamage");
        CurrentState = PlayerState.TakingDamage;
        isTakingDammage = true;
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            animatedSprite2D.Play("Death");
        }
    }

    public void _on_animated_sprite_animation_finished()
    {
        GD.Print("animation finished: " + animatedSprite2D.Animation);
        if (animatedSprite2D.Animation == "Death")
        {
            GD.Print("dead");
            animatedSprite2D.Stop();
            animatedSprite2D.Hide();
            Velocity = new Vector2(0, 0);
            EmitSignal(nameof(Death));
        }
    }

    public void RespawnPlayer()
    {
        Health = 3;
        animatedSprite2D.Play("Idle");
        animatedSprite2D.Show();
    }
}
