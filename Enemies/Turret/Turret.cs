using Godot;
using System;

public partial class Turret : Enemy
{
    private enum TurretState
    {
        Idle,
        Tracking,
        Attacking,
        TakeDamage,
    }
    private TurretState currentState;
    private double attackCooldown = 1.5;
    private double attackCooldownReset = 1.5;
    private bool isAttacking = false;
    private bool active;
    private Sprite2D turretHeadSprite;
    private Node2D turretHeadNode;
    private float initialRotation;
    [Export]
    private bool TracksLeft;

    private PlayerController player;
    [Export]
    public PackedScene projectileScene;
    private RayCast2D targeting;

    public override void _Ready()
    {
        Health = 1;
        DamageDealtAmount = 1;
        turretHeadNode = GetNode<Node2D>("TurretHead");
        turretHeadSprite = turretHeadNode.GetNode<Sprite2D>("TurretHeadSprite2D");
        projectileScene = (PackedScene)ResourceLoader.Load("res://Attacks/Projectile/Projectile.tscn");
        targeting = turretHeadNode.GetNode<RayCast2D>("TargetingRayCast2D");

        if (TracksLeft)
        {
            // Flip RayCast to point in the opposite direction (180 degrees)
            targeting.RotationDegrees = 90f;
            // Move the RayCast2D to the opposite side of the turret head
            Vector2 currentPosition = targeting.Position;
            targeting.Position = new Vector2(-currentPosition.X, currentPosition.Y);
        }

        initialRotation = turretHeadNode.GlobalRotation;
        currentState = TurretState.Idle;
    }

    public override void _Process(double delta)
    {
        ProcessTimers(delta);
        switch (currentState)
        {
            case TurretState.Idle:
                // Should this be in it's own method?
                ProcessIdle();
                break;
            case TurretState.Tracking:
                ProcessTracking();
                break;
            case TurretState.Attacking:
                ProcessAttack();
                break;
            case TurretState.TakeDamage:
                break;
        }
    }

    public override void TakeDamage(int DamageAmount)
    {
        currentState = TurretState.TakeDamage;
        Health -= DamageAmount;

        if (Health <= 0)
        {
            QueueFree();
        }
    }

    private void ProcessIdle()
    {
        //Slowly bring the turret head Node back to the original position.
        turretHeadNode.GlobalRotation = Mathf.LerpAngle(turretHeadNode.GlobalRotation, initialRotation, 0.01f);
    }

    private void ProcessTracking()
    {
        if (active)
        {
            var angleToPlayer = GlobalPosition.AngleToPoint(player.GlobalPosition);

            // Is Seem no good but it good.
            turretHeadSprite.FlipH = !TracksLeft;

            var angleToPlayerDegrees = Mathf.RadToDeg(angleToPlayer);
            if (angleToPlayerDegrees < 0) angleToPlayerDegrees += 360;
            bool playerInFrontOfTurret;

            if (TracksLeft)
            {
                playerInFrontOfTurret = angleToPlayerDegrees >= 120 && angleToPlayerDegrees <= 240;
            }
            else
            {
                playerInFrontOfTurret = angleToPlayerDegrees <= 60 || angleToPlayerDegrees >= 300;
            }
            GD.Print("angle to player degrees: " + angleToPlayerDegrees);

            if (playerInFrontOfTurret)
            {
                // Slowly move the head towards the player's position
                var r = turretHeadNode.GlobalRotation;
                turretHeadNode.GlobalRotation = (float)Mathf.LerpAngle(r, Mathf.DegToRad(angleToPlayerDegrees), 0.05f);

                object target;
                bool isPlayer;
                if (targeting.IsColliding())
                {
                    // GD.Print(targeting.GetCollider().GetType().Name);
                    target = targeting.GetCollider();
                    isPlayer = target.GetType().Name is "PlayerController";

                    if (isPlayer && !isAttacking && playerInFrontOfTurret)
                    {
                        currentState = TurretState.Attacking;
                    }
                }
            }
            else
            {
                ProcessIdle();
            }
        }
        else
        {
            currentState = TurretState.Idle;
        }
    }

    private void ProcessAttack()
    {
        // Godot Intersect Ray requires this layout. SO DONT CHANGE IT!!!
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
            Marker2D projectileSpawn = turretHeadNode.GetNode<Marker2D>("ProjectileSpawn");
            Node2D collider = result["collider"].As<Node2D>();
            if (collider is PlayerController)
            {
                projectileSpawn.LookAt(player.Position);
                Projectile projectile = (Projectile)projectileScene.Instantiate();
                Owner.AddChild(projectile);
                projectile.shooter = this;
                Vector2 direction = (player.GlobalPosition - projectileSpawn.GlobalPosition).Normalized();
                projectile.velocity = direction * projectile.speed;
                projectile.GlobalTransform = projectileSpawn.GlobalTransform;
                // GD.Print("Pew pew X: " + projectile.Transform.X);
                isAttacking = true;
                currentState = TurretState.Tracking;
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
    private void _on_detection_radius_body_entered(Node2D body)
    {
        GD.Print("body: " + body.Name + " has entered the detection radious");
        if (body is PlayerController)
        {
            player = body as PlayerController;
            active = true;
            currentState = TurretState.Tracking;
        }
    }

    private void _on_detection_radius_body_exited(Node2D body)
    {
        // GD.Print("body: " + body.Name + " has exited the detection radious");
        if (body is PlayerController)
        {
            currentState = TurretState.Idle;
        }
    }
}
