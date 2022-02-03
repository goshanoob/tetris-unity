using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс для работы с графическим интерфейсом.
/// </summary>
public class GUIController : MonoBehaviour
{
    /// <summary>
    /// Надпись набранных очков.
    /// </summary>
    [SerializeField] private Text scoreLabel = null;
    /// <summary>
    /// Объект игрока.
    /// </summary>
    [SerializeField] private PlayerController player = null;
    /// <summary>
    /// Объект контроллера сцены.
    /// </summary>
    [SerializeField] private SceneController sceneController = null;
    /// <summary>
    /// Оъект окна с результатом игры.
    /// </summary>
    [SerializeField] private ResultDialog resultDialog = null;

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
    /// Обновить количество набранных очков на экране.
    /// </summary>
    /// <param name="score">Количество очков.</param>
    private void OnScoreChanged(int score)
    {
        scoreLabel.text = score.ToString();
    }

    /// <summary>
    /// Вывести сообщение об окончании игры.
    /// </summary>
    private void OnGameOver()
    {
        resultDialog.Open(player.Score);
    }

    /// <summary>
    /// Вызвать событие перезапуска игры.
    /// </summary>
    public void OnRestart()
    {
        // Вызвать собственное событие данного класса при нажатии кнопки рестарта для оповещения объектов-подписчиков.
        RestartClicked?.Invoke();
    }

    /// <summary>
    /// Вызвать событие выбора первого режима игры.
    /// </summary>
    public void OnFirstModeChanged()
    {
        FirstModeChecked?.Invoke();
    }

    /// <summary>
    /// Вызвать событие выбора второго режима игры.
    /// </summary>
    public void OnSecondModeChanged()
    {
        SecondModeChecked?.Invoke();
    }
}
