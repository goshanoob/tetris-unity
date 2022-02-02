using System;

/// <summary>
/// Методы графического интерфейса.
/// </summary>
public interface IUIManager
{
    event Action RestartClicked;
    void OnScoreChanged(int score);
    void OnGameOver();
}
