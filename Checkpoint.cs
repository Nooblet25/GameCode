using Godot;
using System;

public class Checkpoint : Node2D
{
    [Export] public bool Spawnpoint { get; set; } = false;
    [Export] public bool WinCondition { get; set; } = false;

    private bool _activated = false;

    public void Activate()
    {
        if (WinCondition)
        {
            GameManager.Win();
        }

        GameManager.CurrentCheckpoint = this;
        _activated = true;
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Activated");
    }

    private void OnArea2DAreaEntered(Area2D area)
    {
        if (area.GetParent() is Player && !_activated)
        {
            Activate();
        }
    }

    public override void _Ready()
    {
        var area2D = GetNode<Area2D>("Area2D");
        area2D.Connect("area_entered", this, nameof(OnArea2DAreaEntered));
    }
}
