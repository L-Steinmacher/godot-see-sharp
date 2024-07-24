using Godot;
using System;

public partial class MeleAttack : Spell
{
    private AnimatedSprite2D animatedSprite;
    public string ResourcePath = "res://Spells/MeleAttack.tscn";

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }
    public override void _Process(double delta)
    {
    }

    public override void SetUp(bool faceDirection)
    {
        animatedSprite.FlipH = faceDirection;
    }

    public override void CastSpell(bool faceDirection)
    {
        animatedSprite.FlipH = faceDirection;

        animatedSprite.Play("Cast");
    }

    public override string LoadResourcePath()
    {
        return ResourcePath;
    }

    public void _on_area_2d_body_entered(Node body)
    {
        throw new NotImplementedException();

    }
    public void _on_animated_sprite_2d_animation_finished()
    {
        if (animatedSprite.Animation == "Cast")
            QueueFree();
    }
}
