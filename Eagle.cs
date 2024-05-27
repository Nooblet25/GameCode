using Godot;
using System;
using System.Xml.Linq;

public class Eagle : CharacterBody2D
{
    [Export] public float Speed { get; set; } = -60.0f;
    [Export] public float VerticalSpeed { get; set; } = 20.0f;
    [Export] public int Score { get; set; } = 200;

    private float _gravity;
    private int _verticalDirection = 1;
    private bool _facingRight = false;

    public override void _Ready()
    {
        _gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");

        GetNode<AnimationPlayer>("AnimationPlayer").Play("Attack");

        Timer directionChangeTimer = new Timer();
        directionChangeTimer.WaitTime = 1.0f;
        directionChangeTimer.Connect("timeout", this, nameof(OnDirectionChangeTimeout));
        AddChild(directionChangeTimer);
        directionChangeTimer.Start();

        GetNode<Area2D>("hitbox").Connect("body_entered", this, nameof(OnHitboxBodyEntered));
    }

    public override void _PhysicsProcess(float delta)
    {
        // Horizontal movement
        Velocity.x = Speed;

        // Vertical movement
        if (!IsOnFloor())
        {
            Velocity.y += VerticalSpeed * _verticalDirection * delta;
        }

        MoveAndSlide();
    }

    private void Flip()
    {
        _facingRight = !_facingRight;

        Scale = new Vector2(Mathf.Abs(Scale.x) * (_facingRight ? 1 : -1), Scale.y);
        Speed = Mathf.Abs(Speed) * (_facingRight ? 1 : -1);
    }

    private void OnDirectionChangeTimeout()
    {
        _verticalDirection *= -1;
    }

    private void OnHitboxBodyEntered(Node body)
    {
        if (body.GetParent() is Player)
        {
            GetNode<AnimationPlayer>("AnimationPlayer").Play("die");
        }
    }

    private void Die()
    {
        QueueFree();
        GameManager.Score += Score;
    }

    private void OnArea2DAreaEntered(Area2D area)
    {
        if (area.GetParent() is Player player)
        {
            player.TakeDamage(1);
        }
    }
}
