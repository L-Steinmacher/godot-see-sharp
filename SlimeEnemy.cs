using Godot;
using System;

public partial class SlimeEnemy : Enemy
{
    private Vector2 velocity = new();
    private AnimatedSprite2D animatedSprite;
    private string[] animationOptions;
    private double idleTimer = 1;
    private float walkSpeed = 30;
    private RayCast2D leftRayCast2D;
    private RayCast2D rightRayCast2D;


    public override void _Ready()
    {
        Health = 2;
        DamageDealtAmount = 1;
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animationOptions = animatedSprite.SpriteFrames.GetAnimationNames();
        leftRayCast2D = GetNode<RayCast2D>("LeftRayCast2d");
        rightRayCast2D = GetNode<RayCast2D>("RightRayCast2d");
    }

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
        if ( Health > 0) {
            velocity = Velocity;
            if (!IsOnFloor()) {
                velocity.Y += gravity * (float)delta;
            }

            if (!animatedSprite.IsPlaying()){
                animatedSprite.Play("Idle");
            }

            if (idleTimer > 0) {
                idleTimer -= delta;
            }

            if ( idleTimer <= 0) {
                float facingDirection = animatedSprite.FlipH ? 1: -1;

                // Here be bugs!
                if (!leftRayCast2D.IsColliding()) {
                    animatedSprite.FlipH = !animatedSprite.FlipH;
                    facingDirection = facingDirection * -1;
                }
                animatedSprite.Play("Walk");
                velocity.X = walkSpeed * facingDirection;
            }

            Velocity = velocity;
            MoveAndSlide();
        }

	}

    public override void TakeDamage(int DamageAmount)
    {
        Health -= DamageAmount;
        if (Health <= 0){
            animatedSprite.Play("Death");
        }
    }

    private void _on_area_2d_body_entered(Node2D body) {
        if (body is CharacterBody2D) {
            if (body is PlayerController) {
                PlayerController pc = body as PlayerController;
                pc.TakeDamage(DamageDealtAmount);
            }
        }
    }

    private void _on_animated_sprite_2d_animation_finished() {
        if (animatedSprite.Animation == "Death") {
            QueueFree();
        }
    }
}
