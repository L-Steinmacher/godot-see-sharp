using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	public const float WallJumpVerticalVelocity = -400.0f;
	private const float Friction = 0.3f;
	private const float Acceleration = 0.2f;
	private const float DashSpeed = 1000.0f;
	private bool isDashing = false;
	private bool canDash = true;
	private double dashTimer = 1;
	private double dashTimeReset = 1;
	public bool isAttacking = false;
	private bool isWallJumping = false;
	private double wallJumpTimer = .3;
	private double wallJumpTimeReset = .3;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public float dashGravity = 0.0f;

	public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity.Y += gravity * (float)delta;
        }

        if (IsOnFloor())
            if (!isDashing)
            {
                canDash = true;
            }

        velocity = ProcessJump(delta, velocity);
        Vector2 direction = ProcessMovement(ref velocity);

        velocity = ProcessDash(delta, velocity, direction);

        Velocity = velocity;
        MoveAndSlide();
    }

    private Vector2 ProcessMovement(ref Vector2 velocity)
    {
        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		if (direction.X < 0)
				GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = true;
		else if (direction.X > 0)
				GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = false;
		if (IsOnFloor()) {
			if (direction != Vector2.Zero)
			{
				GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("Run");
				velocity.X = Mathf.Lerp(velocity.X, direction.X * Speed, Acceleration);
			}
			else
			{
				GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("Idle");
				velocity.X = Mathf.Lerp(Velocity.X, 0, Friction);
			}
		}

        return direction;
    }

    private Vector2 ProcessDash(double delta, Vector2 velocity, Vector2 direction)
    {
        if (canDash)
        {
            if (Input.IsActionJustPressed("dash"))
            {
                velocity.Y = dashGravity;
                isDashing = true;
                canDash = false;
                velocity.X = DashSpeed * direction.X;

            }
        }
        if (isDashing)
        {
            dashTimer -= delta;
            if (dashTimer <= 0)
            {
                isDashing = false;
                dashTimer = dashTimeReset;
                velocity = new Vector2(0, 0);
                velocity.Y = gravity;
            }
        }

        return velocity;
    }

    private Vector2 ProcessJump(double delta, Vector2 velocity)
    {
		if (!IsOnFloor()) {
			if (velocity.Y < 0)
				GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("Jump");
			else
				GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("Fall");
		}
        if (Input.IsActionPressed("ui_accept") && IsOnFloor())
            velocity.Y = JumpVelocity;

        if (!isWallJumping)
        {
            if (Input.IsActionJustPressed("ui_accept") && GetNode<RayCast2D>("LeftRayCast2D").IsColliding())
            {
                velocity.Y = WallJumpVerticalVelocity;
                velocity.X = -JumpVelocity;
                isWallJumping = true;
            }
            if (Input.IsActionJustPressed("ui_accept") && GetNode<RayCast2D>("RightRayCast2D").IsColliding())
            {
                velocity.Y = WallJumpVerticalVelocity;
                velocity.X = JumpVelocity;
                isWallJumping = true;
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

        return velocity;
    }
}
