using Godot;
using System;

public partial class MeleAttack : Spell
{
    public AnimatedSprite2D animatedSprite;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void SetUp(bool faceDirection)
    {
        animatedSprite.FlipH = facingDirection;
        facingDirection = faceDirection;
    }

    public override void CastSpell(bool facingDirection)
    {
        GD.Print("Casting MeleAttack");
        animatedSprite.Play("Attack");
    }

    public override void LoadResourcePath()
    {
        throw new NotImplementedException();
    }

    public void _on_area_2d_body_entered(Node body)
    {
        throw new NotImplementedException();

    }
    public void _on_animated_sprite_2d_animation_finished()
    {
        if (animatedSprite.Animation == "Attack")

            QueueFree();
    }
}
