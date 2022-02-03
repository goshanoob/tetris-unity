using System;

/// <summary>
/// Графический интерфейс.
/// </summary>
public interface IUIManager
{
    event Action RestartClicked;
    void OnScoreChanged(int score);
    void OnGameOver();
}
