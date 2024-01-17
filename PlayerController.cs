using Godot;
using System;
using System.Diagnostics;
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
        MeleAttacking,
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
    private const float DashSpeed = 500.0f;
    public const float DashGravity = 0.0f;
    private bool isDashing = false;
    private bool canDash = true;
    private double dashTimer = .3;
    private double dashTimeReset = .3;
    private double dashCooldownTimer = 1;
    private double dashCooldownTimeReset = 1;
    private double damageTimer = .2;
    private double damageTimerReset = .2;
    public bool isAttacking = false;

    private bool isWallJumping = false;
    private double wallJumpTimer = .3;
    private double wallJumpTimeReset = .3;
    private bool canDoubleJump = true;
    private bool isDoubleJumping = false;
    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    public float gravityReset = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private AnimatedSprite2D animatedSprite2D;
    public int Health = 3;
    private bool isTakingDammage = false;
    public float mana = 100f;
    private float maxMana = 100f;
    [Signal]
    public delegate void DeathEventHandler();
    [Export]
    public PackedScene GhostPlayerInstance;
    [Export]
    public PackedScene JumpEffectsInstance;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        GameManager.Player = this;
    }

    public override void _PhysicsProcess(double delta)
    {
        ProcessTimers(delta);
        if (CurrentState != PlayerState.TakingDamage)
        {
            InputManager(delta);
        }
        switch (CurrentState)
        {
            case PlayerState.MeleAttacking:
                if (!isAttacking)
                    ProcessMeleAttack();
                break;
            case PlayerState.Idle:
                ProcessIdle(ref velocity);
                break;
            case PlayerState.Running:
                break;
            case PlayerState.Dashing:
                Velocity = ProcessDash(velocity);
                break;
            case PlayerState.TakingDamage:
                TakeDamage(delta);
                break;
            default:
                break;
        }
    }

    private void TakeDamage(double delta)
    {
        damageTimer -= delta;
        if (damageTimer <= 0)
        {
            CurrentState = PlayerState.Idle;
            damageTimer = damageTimerReset;
            isTakingDammage = false;
        }
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
            if (CurrentState != PlayerState.MeleAttacking)
            {
                if (IsOnFloor())
                {
                    isDoubleJumping = false;
                    if (!isDashing && dashCooldownTimer <= 0)
                    {
                        canDash = true;
                        dashCooldownTimer = dashCooldownTimeReset;
                    }
                }

                if (velocity.X > 0)
                {
                    animatedSprite2D.FlipH = false;
                }
                else if (velocity.X < 0)
                {
                    animatedSprite2D.FlipH = true;
                }
                if (Input.IsActionJustPressed("attack"))
                {
                    CurrentState = PlayerState.MeleAttacking;
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
                if (Input.IsActionJustPressed("interact"))
                {
                    InteractWithObject();
                }
                facingDirection = ProcessMovement(ref velocity);
            }
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
        if (isDashing)
        {
            dashTimer -= delta;
            dashCooldownTimer -= delta;
            GhostPlayer ghostPlayer = GhostPlayerInstance.Instantiate() as GhostPlayer;
            Owner.AddChild(ghostPlayer);
            ghostPlayer.GlobalPosition = this.GlobalPosition;
            ghostPlayer.SetHValue(animatedSprite2D.FlipH);

            if (dashTimer <= 0)
            {
                isDashing = false;
                dashTimer = dashTimeReset;
                Velocity = new Vector2(0, 0);
                // velocity.Y = gravity;
                CurrentState = PlayerState.Idle;
                gravity = gravityReset;
            }
        }
    }
    private void ProcessIdle(ref Vector2 velocity)
    {
        if (velocity.X < 5 && velocity.X > -5)
        {
            if (animatedSprite2D.Animation != "Attack" && animatedSprite2D.IsPlaying())
            {
                animatedSprite2D.Play("Idle");
            }
        }
        velocity.X = Mathf.Lerp(velocity.X, 0, Friction);
    }
    private Vector2 ProcessMovement(ref Vector2 velocity)
    {
        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("move_left", "move_right", "climb_up", "climb_down");
        if (!isTakingDammage && !isDashing)
        {
            if (Input.IsActionJustPressed("dash"))
            {
                CurrentState = PlayerState.Dashing;
            }
            if (direction.X < 0)
            {
                facingDirection.X = -1;
                animatedSprite2D.FlipH = true;
            }
            else if (direction.X > 0)
            {
                facingDirection.X = 1;
                animatedSprite2D.FlipH = false;
            }
            else if (direction.Y < 0)
            {
                facingDirection.Y = -1;
            }
            else if (direction.Y > 0)
            {
                facingDirection.Y = 1;
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
                    ProcessIdle(ref velocity);
                }
            }
            else
            {
                velocity.X = Mathf.Lerp(velocity.X, direction.X * Speed, Acceleration);
            }
        }

        if (velocity.X < 10 && velocity.X > -10)
        {
            isTakingDammage = false;
        }
        return direction;
    }

    private Vector2 ProcessDash(Vector2 velocity)
    {
        // var direction = animatedSprite2D.FlipH ? -1 : 1;
        if (canDash && !isDashing)
        {
            dashTimer = dashTimeReset;
            isDashing = true;
            GD.Print("dash X" + velocity.X);
            velocity.X = DashSpeed * facingDirection.X;
            // GD.Print("dash Y" + facingDirection.Y);

            gravity = DashGravity;
            if (facingDirection.Y < .5 && facingDirection.Y > -.5)
            {
                velocity.Y = 0;
            }
            else if (facingDirection.Y <= -.5 && facingDirection.Y >= -.85)
            {
                velocity.Y = DashSpeed * -.5f;
            }
            else if (facingDirection.Y < -.85)
            {
                gravity = gravityReset;
                velocity.Y = DashSpeed * -1;
            }
            else if (facingDirection.Y >= .5 && facingDirection.Y <= .85)
            {
                velocity.Y = DashSpeed * .5f;
            }
            else if (facingDirection.Y > .85)
            {
                velocity.Y = DashSpeed * 1;
            }
        }
        return velocity;
    }

    private Vector2 ProcessJump(Vector2 velocity)
    {
        var IsColliding = GetNode<RayCast2D>("LeftRayCast2D").IsColliding() || GetNode<RayCast2D>("RightRayCast2D").IsColliding();
        if (!IsOnFloor())
        {
            if (velocity.Y < 0)
                animatedSprite2D.Play("Jump");
            else
                animatedSprite2D.Play("Fall");

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
            if (!IsColliding && canDoubleJump && !isDoubleJumping)
            {
                if (Input.IsActionJustPressed("jump") && canDoubleJump && !isDoubleJumping)
                {
                    JumpEffects je = JumpEffectsInstance.Instantiate() as JumpEffects;
                    Owner.AddChild(je);
                    je.GetNode<AnimatedSprite2D>("AnimatedSprite2D").GlobalPosition = this.GlobalPosition;
                    velocity.Y = JumpVelocity;
                    isDoubleJumping = true;
                    animatedSprite2D.Play("DoubleJump");
                }
            }
        }
        else
        {
            JumpEffects je = JumpEffectsInstance.Instantiate() as JumpEffects;
            Owner.AddChild(je);
            je.GetNode<AnimatedSprite2D>("AnimatedSprite2D").GlobalPosition = this.GlobalPosition;
            velocity.Y = JumpVelocity;
        }
        return velocity;
    }
    private void ProcessMeleAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animatedSprite2D.Play("Attack");
        }
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

    public void UpdateMana(float amount)
    {
        mana += amount;
        if (mana > maxMana)
        {
            mana = maxMana;
        }
        else if (mana < 0)
        {
            mana = 0;
        }
    }
    public void RespawnPlayer()
    {
        Health = 3;
        animatedSprite2D.Play("Idle");
        animatedSprite2D.Show();
    }

    public void InteractWithObject()
    {
        var IsColliding = GetNode<RayCast2D>("LeftRayCast2D").IsColliding() || GetNode<RayCast2D>("RightRayCast2D").IsColliding();
        if (IsColliding)
        {
            Node obj = (Node)GetNode<RayCast2D>("LeftRayCast2D").GetCollider() ?? (Node)GetNode<RayCast2D>("RightRayCast2D").GetCollider();
            if (obj.Owner is Collectable)
            {
                GD.Print("Collectable: " + obj.Name);
                if (obj.Owner is MagicPotion)
                {
                    MagicPotion mp = (MagicPotion)obj.Owner;
                    mp.UsePotion();
                }
            }

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
        if (animatedSprite2D.Animation == "DoubleJump")
        {
            animatedSprite2D.Play("Fall");
        }
        if (animatedSprite2D.Animation == "Attack")
        {
            isAttacking = false;
            CurrentState = PlayerState.Idle;
        }
    }
}
