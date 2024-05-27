using Godot;
using System;
using System.Resources;
using System.Xml.Linq;

public class Cherry : Area2D
{
    [Export] public int Coins { get; set; } = 1;
    [Export] public int Score { get; set; } = 50;

    private AudioStreamPlayer _audioStreamPlayer;

    public override void _Ready()
    {
        _audioStreamPlayer = new AudioStreamPlayer();
        AddChild(_audioStreamPlayer);

        Connect("body_entered", this, nameof(OnBodyEntered));
    }

    private void SpawnFeedback()
    {
        var sceneToSpawn = (PackedScene)ResourceLoader.Load("res://Pickups/Feedback/feedback.tscn");
        var newSceneInstance = (Node2D)sceneToSpawn.Instance();
        GetTree().CurrentScene.AddChild(newSceneInstance);
        newSceneInstance.GlobalPosition = GlobalPosition;
    }

    private void OnBodyEntered(Node body)
    {
        GameManager.GainCoins(Coins);
        GameManager.Score += Score;
        SpawnFeedback();
        QueueFree();
    }
}
