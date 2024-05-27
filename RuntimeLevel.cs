using Godot;
using System;
using System.Xml.Linq;

public class RunTimeLevel : Node
{
    [Export] public int MaxScore { get; private set; } = 0;
    [Export] public int MaxCoins { get; private set; } = 0;
    [Export] public int MaxEnemies { get; private set; } = 0;

    private string _levelName;

    public override void _Ready()
    {
        _levelName = Name;

        GameManager.Coins = 0;
        GameManager.Score = 0;
        GameManager.DamageTaken = 0;
        GameManager.EnemiesBeaten = 0;

        GameManager.Connect(nameof(GameManager.LevelBeaten), this, nameof(BeatLevel));

        SetValues();
    }

    private void SetValues()
    {
        foreach (Node node in GetChildren())
        {
            if (node is Coin coin)
            {
                MaxScore += coin.Score;
                MaxCoins += coin.CoinValue;
            }
            else if (node is Sabertooth sabertooth || node is Canon canon)
            {
                MaxScore += sabertooth.Score;
                MaxEnemies += 1;
            }
        }
    }

    private void BeatLevel()
    {
        var levelData = LevelData.LevelDic[_levelName];
        LevelData.GenerateLevel(levelData["unlocks"]);
        LevelData.LevelDic[levelData["unlocks"]]["unlocked"] = true;

        LevelData.UpdateLevel(
            _levelName,
            GameManager.Score,
            MaxScore,
            GameManager.Coins,
            MaxCoins,
            GameManager.EnemiesBeaten,
            MaxEnemies,
            GameManager.DamageTaken,
            true
        );

        SaveManager.SaveGame();
    }
}
