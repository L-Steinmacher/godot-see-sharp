using Godot;
using System;
using System.Linq;

public partial class Turret : RigidBody2D
{
    private double attackCooldown = 1.5;
    private double attackCooldownReset = 1.5;
    private bool isAttacking = false;
    private bool active;
    private Sprite2D sprite;
    private PlayerController player;
    [Export]
    public PackedScene projectileScene;

    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
        projectileScene = (PackedScene)ResourceLoader.Load("res://Enemies/Turret/Projectile.tscn");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        ProcessTimers(delta);
        ProcessAttack();
    }

    private void ProcessAttack()
    {
        if (active)
        {
            var angle = GlobalPosition.AngleToPoint(player.GlobalPosition);
            if (Mathf.Abs(angle) > Mathf.Pi / 2)
            {
                sprite.FlipH = false;
            }
            else
            {
                sprite.FlipH = true;
            }
            if (!isAttacking)
            {
                // Godot Intersect Ray requires this layout
                var queryParameters = new PhysicsRayQueryParameters2D
                {
                    From = Position,
                    To = player.Position,
                    Exclude = new Godot.Collections.Array<Rid> { GetRid() }
                };

                var spaceState = GetWorld2D().DirectSpaceState;
                Godot.Collections.Dictionary result = spaceState.IntersectRay(queryParameters);
                if (result != null && result.ContainsKey("collider"))
                {
                    Marker2D projectileSpawn = GetNode<Marker2D>("ProjectileSpawn");

                    Node2D collider = result["collider"].As<Node2D>();
                    if (collider is PlayerController)
                    {
                        // Marker2D projectileSpawn = GetNode<Marker2D>("ProjectileSpawn");
                        // projectileSpawn.LookAt(player.Position);
                        projectileSpawn.LookAt(player.Position);
                        Projectile projectile = (Projectile)projectileScene.Instantiate();
                        Owner.AddChild(projectile);
                        // Set the velocity of the projectile towards the player
                        Vector2 direction = (player.GlobalPosition - projectileSpawn.GlobalPosition).Normalized();
                        projectile.velocity = direction * projectile.speed; // projectileSpeed should be defined as the speed you want
                        projectile.GlobalTransform = projectileSpawn.GlobalTransform;
                        GD.Print("Pew pew X: " + projectile.Transform.X);
                        isAttacking = true;
                    }
                }
            }
        }
    }

    private void ProcessTimers(double delta)
    {
        if (isAttacking)
        {
            attackCooldown -= delta;
            if (attackCooldown <= 0)
            {
                isAttacking = false;
                attackCooldown = attackCooldownReset;
            }
        }
    }
    private void _on_detection_radious_body_entered(Node2D body)
    {
        GD.Print("body: " + body.Name + " has entered the detection radious");
        if (body is PlayerController)
        {
            player = body as PlayerController;
            active = true;
        }

    }

    private void _on_detection_radious_body_exited(Node2D body)
    {
        GD.Print("body: " + body.Name + " has exited the detection radious");
        if (body is PlayerController)
        {
            active = false;
        }
    }
}
