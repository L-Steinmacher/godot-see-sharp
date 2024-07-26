using Godot;
using System;

public partial class SlimeEnemy : Enemy
{
    private AnimatedSprite2D animatedSprite;
    private string[] animationOptions;

    public override void _Ready()
    {
        DamageDealtAmount = 1;
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animationOptions = animatedSprite.SpriteFrames.GetAnimationNames();
    }

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
        if (!animatedSprite.IsPlaying()){
            animatedSprite.Play("Idle");
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
