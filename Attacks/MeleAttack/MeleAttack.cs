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
        GD.Print("For mele attack use the MeleCast(bool faceDirection, CastAnimation cast) method");
        animatedSprite.Play("Cast");
    }

    public void MeleCast(bool faceDirection, CastAnimation cast)
    {
        GD.Print(cast);
        switch (cast)
        {
            case CastAnimation.Single:
                GD.Print("Single");
                animatedSprite.FlipH = faceDirection;
                animatedSprite.Play("Single");
                break;
            case CastAnimation.Double:
                GD.Print("Double");
                animatedSprite.FlipH = faceDirection;
                animatedSprite.Play("Double");
                break;
            case CastAnimation.Tripple:
                GD.Print("Tripple");
                animatedSprite.FlipH = faceDirection;
                animatedSprite.Play("Tripple");

                break;
            default:
                GD.Print("Default");
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
            e.TakeDamage(1);
        }

    }
    public void _on_animated_sprite_2d_animation_finished()
    {
        if (animatedSprite.Animation == "Single")
            QueueFree();
        if (animatedSprite.Animation == "DoubleSlash")
        {
            QueueFree();
        }
        if (animatedSprite.Animation == "Tripple")
        {
            QueueFree();
        }
    }
}
