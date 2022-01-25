using UnityEngine;

/// <summary>
///  Класс, описывающий поведение фигуры.
/// </summary>
public class FigureController : MonoBehaviour
{
    // Возможное ускорение фигуры при движении вниз.
    [SerializeField] private float speedMultiplier = 2;

    /// <summary>
    /// Возможные направления перемещения фигуры.
    /// </summary>
    private enum Directions
    {
        left = 0,
        right = 1,
        down = 2,
    }
    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveFigure(Directions.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveFigure(Directions.right);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveFigure(Directions.down);
        }
    }

    /// <summary>
    /// Метод перемещения фигуры на сцене.
    /// </summary>
    /// <param name="direction">Направление перемещения.</param>
    private void MoveFigure(Directions direction)
    {
        switch(direction){
            case Directions.left:
                transform.Translate(Vector3.left);
                break;

            case Directions.right:
                transform.Translate(Vector3.right);
                break;

            case Directions.down:
                transform.Translate(speedMultiplier * Vector3.down);
                break;
        }
    }
}
