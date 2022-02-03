using goshanoob.TETRIS;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Диалоговое окно об окончании игры.
/// </summary>
public class ResultDialog : MonoBehaviour, IDialogWindow
{
    /// <summary>
    /// Надпись с результатом игры.
    /// </summary>
    [SerializeField] private Text resultLabel = null;

    private void Start()
    {
        // Закрыть окно при старте.
        Close();
    }

    /// <summary>
    /// Открыть диалоговое окно с результатами игры.
    /// </summary>
    public void Open(string score)
    {
        resultLabel.text = $"Ваш результат: {score} линия(-ий)";
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Закрыть диалоговое окно.
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }
}