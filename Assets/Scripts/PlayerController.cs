using System;
using UnityEngine;

/// <summary>
///  Класс, описывающий игрока в тетрис.
/// </summary>
internal class PlayerController : MonoBehaviour
{
    private int score = 0;

    public event Action<int> ScoreChanged;
    public event Action RotateClick;
    public event Action DownClick;
    public event Action<Vector3> ButtonClick;

    public int Score
    {
        get => score;
        set
        {
            score = value;
            ScoreChanged?.Invoke(score);
        }
    }

    public static PlayerController Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
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
            ButtonClick?.Invoke(Vector3.left);
        }
        else if (right)
        {
            ButtonClick?.Invoke(Vector3.right);
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
