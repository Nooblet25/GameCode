using Godot;
using System.Threading.Tasks;

public class Player : CharacterBody2D
{
    [Export] public float Speed = 110.0f;
    [Export] public float JumpVelocity = -300.0f;

    private AnimatedSprite2D _animatedSprite2D;

    public int MaxHealth { get; private set; } = 3;
    public int Health { get; private set; }
    public bool CanTakeDamage { get; private set; } = true;

    private float _gravity;

    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");

        GameManager.DamageTaken = 0;
        Health = MaxHealth;
        GameManager.Player = this;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!IsOnFloor())
        {
            Velocity.y += _gravity * delta;
        }

        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            Velocity.y = JumpVelocity;
        }

        float inputAxis = Input.GetAxis("ui_left", "ui_right");
        if (inputAxis != 0)
        {
            Velocity.x = inputAxis * Speed;
        }
        else
        {
            Velocity.x = Mathf.MoveToward(Velocity.x, 0, Speed);
        }

        MoveAndSlide();
        UpdateAnimations(inputAxis);

        if (Position.y >= 600)
        {
            Die();
        }
    }

    private void UpdateAnimations(float inputAxis)
    {
        if (inputAxis != 0)
        {
            _animatedSprite2D.FlipH = inputAxis < 0;
            _animatedSprite2D.Play("run");
        }
        else
        {
            _animatedSprite2D.Play("idle");
        }

        if (!IsOnFloor())
        {
            _animatedSprite2D.Play("jump");
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (CanTakeDamage)
        {
            IFrame();

            GameManager.DamageTaken += 1;

            Health -= damageAmount;

            GetNode<Healthbar>("Healthbar").UpdateHealthbar(Health, MaxHealth);

            if (Health <= 0)
            {
                Die();
            }
        }
    }

    private async void IFrame()
    {
        CanTakeDamage = false;
        await Task.Delay(1000); // 1 second
        CanTakeDamage = true;
    }

    private void Die()
    {
        GameManager.RespawnPlayer();
    }
}
