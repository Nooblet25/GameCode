using Godot;
using System;
using System.Reflection.Emit;
using System.Xml.Linq;

public class YourClassName : Node
{
    [Signal]
    public delegate void GainedCoins();
    [Signal]
    public delegate void LevelBeaten();

    private int _coins = 0;
    private int _score = 0;

    private Checkpoint _currentCheckpoint;
    private Node _pauseMenu;
    private Node _winScreen;
    private Label _scoreLabel;

    private Player _player;
    private int _damageTaken = 0;
    private int _enemiesBeaten = 0;
    private bool _paused = false;

    public override void _Ready()
    {
        // _pauseMenu = GetNode<Node>("Path/To/PauseMenu");
        // _winScreen = GetNode<Node>("Path/To/WinScreen");
        // _scoreLabel = GetNode<Label>("Path/To/ScoreLabel");
        // _player = GetNode<Player>("Path/To/Player");
    }

    private void RespawnPlayer()
    {
        _player.Health = _player.MaxHealth;
        if (_currentCheckpoint != null)
        {
            _player.Position = _currentCheckpoint.GlobalPosition;
        }
    }

    private void GainCoins(int coinsGained)
    {
        _coins += coinsGained;
        EmitSignal(nameof(GainedCoins));
    }

    private void Win()
    {
        EmitSignal(nameof(LevelBeaten));
        _winScreen.Visible = true;
        _scoreLabel.Text = "score: " + _score.ToString();
    }

    private void PausePlay()
    {
        _paused = !_paused;
        _pauseMenu.Visible = _paused;
        GetTree().Paused = _paused;
    }

    private void Resume()
    {
        GetTree().Paused = false;
        PausePlay();
    }

    private void Restart()
    {
        _coins = 0;
        _score = 0;
        GetTree().ReloadCurrentScene();
    }

    private void LoadWorld()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://Scenes/WorldScenes/world_map.tscn");
    }

    private void Quit()
    {
        GetTree().Quit();
    }
}
