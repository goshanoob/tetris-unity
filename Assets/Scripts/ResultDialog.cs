using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Диалоговое окно об окончании игры.
/// </summary>
public class ResultDialog : MonoBehaviour
{
    [SerializeField] private Text resultLabel = null; // подпись с результатом игры

    private void Start()
    {
        // Закрыть окно при старте.
        Close();
    }

    /// <summary>
    /// Открыть диалоговое окно с результатами игры.
    /// </summary>
    public void Open(int score)
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
