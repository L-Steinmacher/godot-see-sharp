using Godot;
using System;

public partial class MeleAttack : Spell
{
    public enum CastAnimation
    {
        Single,
        Double,
        Tripple,
    }
    private AnimatedSprite2D animatedSprite;
    private int attackDamage = 1;
    public string ResourcePath = "res://Attacks/MeleAttack/MeleAttack.tscn";

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
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

    public void MeleCast(bool faceDirection, CastAnimation cast)
    {
        switch (cast)
        {
            case CastAnimation.Single:
                animatedSprite.FlipH = faceDirection;
                animatedSprite.Play("Single");
                break;
            case CastAnimation.Double:
                animatedSprite.FlipH = faceDirection;
                animatedSprite.Play("Double");
                break;
            case CastAnimation.Tripple:
                animatedSprite.FlipH = faceDirection;
                animatedSprite.Play("Tripple");
                break;
            default:
                GD.Print("Default case: If you see this then something went wrong in MeleAttack.cs");
                break;
        }
    }

    public override string LoadResourcePath()
    {
        return ResourcePath;
    }

    public void _on_area_2d_body_entered(Node2D body)
    {
        if (body is Enemy)
        {
            Enemy e = body as Enemy;
            int dammageAmount = animatedSprite.Animation == "Tripple" ? 2 : 1;
            e.TakeDamage(dammageAmount);

        }

    }
    public void _on_animated_sprite_2d_animation_finished()
    {
        QueueFree();
    }
}
