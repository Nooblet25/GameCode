using Godot;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

public class YourNode2DClass : Node2D
{
    [Export] private float LerpSpeed = 0.5f;
    [Export] private float LerpThreshold = 0.1f;

    private Node2D _levelHolder;
    private Node2D _player;
    private List<Node> _levels = new List<Node>();
    private Node2D _currLevel;

    private float _lerpProgress = 0.0f;
    private bool _completedMovement = true;

    public override void _Ready()
    {
        _levelHolder = GetNode<Node2D>("LevelHolder");
        _player = GetNode<Node2D>("Player");

        _currLevel = GetNode<Node2D>("LevelHolder/Level1");

        SaveManager.LoadGame();
        _player.GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");

        foreach (Node level in _levelHolder.GetChildren())
        {
            _levels.Add(level);
        }

        UpdateLevels();

        int coins = 0;
        foreach (var level in SaveManager.SaveData.LevelDic)
        {
            coins += level.Value["coins"];
            GD.Print(coins);
        }
    }

    private void UpdateLevels()
    {
        foreach (Node2D level in _levels)
        {
            if (LevelData.LevelDic.ContainsKey(level.Name))
            {
                var levelData = LevelData.LevelDic[level.Name];
                if (levelData["unlocked"])
                {
                    level.GetNode<Sprite2D>("Sprite2D").Texture = (Texture)GD.Load("res://Imports/Sprites/EpisodeFifteen/unlocked.png");
                    if (levelData["beaten"])
                    {
                        level.GetNode<Sprite2D>("Sprite2D").Texture = (Texture)GD.Load("res://Imports/Sprites/EpisodeFifteen/beaten.png");
                    }
                }
                else
                {
                    level.GetNode<Sprite2D>("Sprite2D").Texture = (Texture)GD.Load("res://Imports/Sprites/EpisodeFifteen/locked.png");
                }
            }
        }
    }

    public override void _Process(float delta)
    {
        Node2D targetLevel = null;

        if (Input.IsActionPressed("up") && _currLevel.HasNode("up"))
        {
            targetLevel = _currLevel.GetNode<Node2D>("up");
        }
        else if (Input.IsActionPressed("down") && _currLevel.HasNode("down"))
        {
            targetLevel = _currLevel.GetNode<Node2D>("down");
        }
        else if (Input.IsActionPressed("left") && _currLevel.HasNode("left"))
        {
            targetLevel = _currLevel.GetNode<Node2D>("left");
        }
        else if (Input.IsActionPressed("right") && _currLevel.HasNode("right"))
        {
            targetLevel = _currLevel.GetNode<Node2D>("right");
        }

        if (Input.IsActionJustPressed("jump"))
        {
            _player.GetNode<AnimationPlayer>("AnimationPlayer").Play("Select");
            GetTree().CreateTimer(0.4f).Connect("timeout", this, nameof(OnJumpTimeout));
        }

        if (targetLevel != null && LevelData.LevelDic.ContainsKey(targetLevel.Name) && LevelData.LevelDic[targetLevel.Name]["unlocked"] && _completedMovement)
        {
            MoveToTargetLevel(targetLevel, delta);
        }
    }

    private void OnJumpTimeout()
    {
        GetTree().ChangeScene("res://Scenes/WorldScenes/" + _currLevel.Name + ".tscn");
    }

    private async void MoveToTargetLevel(Node2D targetLevel, float delta)
    {
        _completedMovement = false;
        _player.GetNode<AnimationPlayer>("AnimationPlayer").Play("Run");
        _lerpProgress = 0.0f;

        while (_lerpProgress < 1.0f)
        {
            _lerpProgress += LerpSpeed * delta;
            _lerpProgress = Mathf.Clamp(_lerpProgress, 0.0f, 1.0f);
            _player.Position = _player.Position.Lerp(targetLevel.GlobalPosition, _lerpProgress);

            if (_player.Position.DistanceTo(targetLevel.GlobalPosition) < LerpThreshold)
            {
                break;
            }

            await ToSignal(GetTree().CreateTimer(delta), "timeout");
        }

        _player.Position = targetLevel.GlobalPosition;
        ShowStats(targetLevel);
        _currLevel = targetLevel;
        _player.GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
        _completedMovement = true;
    }

    private void ShowStats(Node2D targetLevel)
    {
        if (LevelData.LevelDic[targetLevel.Name]["unlocked"])
        {
            var statDisplay = targetLevel.GetNode("StatDisplay");
            statDisplay.Visible = true;
            statDisplay.GetNode<AnimationPlayer>("AnimationPlayer").Play("Show");

            _currLevel.GetNode("StatDisplay").GetNode<AnimationPlayer>("AnimationPlayer").Play("Show", -1.0f, true);

            var levelData = LevelData.LevelDic[targetLevel.Name];
            var coinSprite = targetLevel.GetNode<Sprite2D>("StatDisplay/CoinSprite");
            var skullSprite = targetLevel.GetNode<Sprite2D>("StatDisplay/SkullSprite");
            var healthSprite = targetLevel.GetNode<Sprite2D>("StatDisplay/HealthSprite");

            coinSprite.Visible = levelData["coins"] == levelData["max_coins"] && levelData["score"] > 0;
            skullSprite.Visible = levelData["enemies_beaten"] == levelData["max_enemies_beaten"] && levelData["score"] > 0;
            healthSprite.Visible = levelData["damage_taken"] == 0 && levelData["score"] > 0;
        }
    }
}
