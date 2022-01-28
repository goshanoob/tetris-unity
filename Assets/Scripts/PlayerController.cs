using UnityEngine;
using System;

/// <summary>
///  Класс, описывающий игрока в тетрис.
/// </summary>
internal class PlayerController : MonoBehaviour
{
    [SerializeField] private FigureController figure;

    public int Score
    {
        get;
        set;
    } = 0;

    private enum Directions
    {
        left = 0,
        right = 1,
        down = 2,
    }

    public event Action ButtonClick;


    private void Update()
    {
        bool left = Input.GetKeyDown(KeyCode.LeftArrow);
        bool right = Input.GetKeyDown(KeyCode.RightArrow);
        bool down = Input.GetKey(KeyCode.DownArrow);
        bool space = Input.GetKeyDown(KeyCode.Space);
        /*
        // Если нажата клавиша, выполнить перемещение фигуры.
        if (left)
        {
            MoveFigure(Directions.left);
        }
        else if (right)
        {
            MoveFigure(Directions.right);
        }

        // Если нажата кнопка вниз, ускорить движение.
        if (down)
        {
            FigureStep(extraDropTime1);
        }

        // Если нажата кливаша Пробел, повернуть фигуру.
        if (space)
        {
            Rotate(angle);
        }
        */
    }
}
