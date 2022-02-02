using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс для работы с графическим интерфейсом.
/// </summary>
public class GUIController : MonoBehaviour
{
    [SerializeField] private Text scoreLabel = null; // подпись набранных очков
    [SerializeField] private PlayerController player = null; // объект игрока
    [SerializeField] private SceneController sceneController = null; // объект контроллера сцены
    [SerializeField] private ResultDialog resultDialog = null; // объект окна с результатом игры

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
        // Добавить обработчик к событию, возникающем в контроллере сцены при окончании игры.
        sceneController.GameOver += OnGameOver;
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
    /// Обработчик события окончания игры.
    /// </summary>
    private void OnGameOver()
    {
        resultDialog.Open(player.Score);
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
