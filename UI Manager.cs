using Godot;
using System;
using System.Reflection.Emit;

public class YourCanvasLayerClass : CanvasLayer
{
    public override void _Ready()
    {
        GameManager.PauseMenu = GetNode<Control>("PauseMenu");
        GameManager.WinScreen = GetNode<Control>("WinScreen");
        GameManager.ScoreLabel = GetNode<Label>("WinScreen/Label");

        GameManager.Connect("gained_coins", this, nameof(UpdateCoinDisplay));
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("pause"))
        {
            GameManager.PausePlay();
            GetTree().Paused = GameManager.Paused;
        }
    }

    private void UpdateCoinDisplay()
    {
        GetNode<Label>("CoinDisplay").Text = GameManager.Coins.ToString();
    }

    private void _on_Resume_pressed()
    {
        GameManager.Resume();
    }

    private void _on_Restart_pressed()
    {
        GameManager.Restart();
    }

    private void _on_WorldMap_pressed()
    {
        GameManager.LoadWorld();
    }

    private void _on_Quit_pressed()
    {
        GameManager.Quit();
    }

    private void _on_FinnishLevel_pressed()
    {
        GameManager.LoadWorld();
    }
}
