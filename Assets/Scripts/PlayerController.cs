using System;
using UnityEngine;

/// <summary>
///  Класс, описывающий игрока в тетрис.
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Количество очков, набранное игроком.
    /// </summary>
    private int score = 0;

    /// <summary>
    /// Событие изменения счета.
    /// </summary>
    public event Action<int> ScoreChanged;
    /// <summary>
    /// Событие вращения фигуры.
    /// </summary>
    public event Action RotateClick;
    /// <summary>
    /// Событие нажатия кнопки вниз.
    /// </summary>
    public event Action DownClick;
    /// <summary>
    /// Событие нажатия кнопкок вбок.
    /// </summary>
    public event Action<Vector3> SideClick;

    /// <summary>
    /// Количество набранных очков.
    /// </summary>
    public int Score
    {
        get => score;
        set
        {
            score = value;
            // Сгенерировать событие изменения счета при установке свойства.
            ScoreChanged?.Invoke(score);
        }
    }

    /// <summary>
    /// Экзепляр класса, описывающего игрока.
    /// </summary>
    public static PlayerController Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        // Инициализация свойства для доступа к текущему экземпляру данного класса.
        Instance = this;
    }

    private void Update()
    {
        bool left = Input.GetKeyDown(KeyCode.LeftArrow);
        bool right = Input.GetKeyDown(KeyCode.RightArrow);
        bool down = Input.GetKey(KeyCode.DownArrow);
        bool space = Input.GetKeyDown(KeyCode.Space);

        // Если нажата клавиша, выполнить перемещение фигуры.
        if (left)
        {
            SideClick?.Invoke(Vector3.left);
        }
        else if (right)
        {
            SideClick?.Invoke(Vector3.right);
        }

        // Если нажата кнопка вниз, ускорить движение.
        if (down)
        {
            DownClick?.Invoke();
        }

        // Если нажата кливаша Пробел, повернуть фигуру.
        if (space)
        {
            RotateClick?.Invoke();
        }
    }
}
