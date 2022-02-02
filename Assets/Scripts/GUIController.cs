using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Класс для работы с графическим интерфейсом.
/// </summary>
internal class GUIController : MonoBehaviour
{
    [SerializeField] private Text scoreLabel = null; // подпись набранных очков
    [SerializeField] private PlayerController player = null; // объект игрока

    /// <summary>
    /// Событие нажатия кнопки перезагрузки игры.
    /// </summary>
    public event Action RestartClicked;

    /// <summary>
    /// Событие выбора первого режима игры.
    /// </summary>
    public event Action FirstModeChecked;

    /// <summary>
    /// Событие выбора второго режима игры.
    /// </summary>
    public event Action SecondModeChecked;

    private void Awake()
    {
        // Добавить обработчик к собтию объекта игрока, возникающем при изменении счета.
        player.ScoreChanged += OnScoreChanged;
    }

    /// <summary>
    /// Обработчик изменения счета игры.
    /// </summary>
    /// <param name="score">Количество очков.</param>
    private void OnScoreChanged(int score)
    {
        scoreLabel.text = score.ToString();
    }

    /// <summary>
    /// Обработчик события нажатия кнопки рестарта игры.
    /// </summary>
    public void OnRestart()
    {
        // Вызвать собственное событие данного класса при нажатии кнопки рестарта для оповещения объектов-подписчиков.
        RestartClicked?.Invoke();
    }

    /// <summary>
    /// Обработчик события нажатия кнопки выбора первого режима игры.
    /// </summary>
    public void OnFirstModeChanged()
    {
        FirstModeChecked?.Invoke();
    }

    /// <summary>
    /// Обработчик события нажатия кнопки выбора второго режима игры.
    /// </summary>
    public void OnSecondModeChanged()
    {
        SecondModeChecked?.Invoke();
    }
}
