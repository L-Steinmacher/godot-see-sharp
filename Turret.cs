using Godot;
using System;
using System.Linq;

public partial class Turret : RigidBody2D
{
    private double attackCooldown = 1.5;
    private double attackCooldownReset = 1.5;
    private bool isAttacking = false;
    private bool active;
    private PlayerController player;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
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
            if (!isAttacking)
            {
                var queryParameters = new PhysicsRayQueryParameters2D
                {
                    From = this.Position,
                    To = player.Position,
                    Exclude = new Godot.Collections.Array<Rid> { this.GetRid() }
                };

                var spaceState = GetWorld2D().DirectSpaceState;
                Godot.Collections.Dictionary result = spaceState.IntersectRay(queryParameters);
                if (result != null && result.ContainsKey("collider"))
                {
                    Node collider = result["collider"].As<Node>();
                    if (collider is PlayerController player)
                    {
                        GD.Print("Pew pew");
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
