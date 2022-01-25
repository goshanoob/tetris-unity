using UnityEngine;

// Требуется компонент управления сценой.
[RequireComponent(typeof(SceneController))]

/// <summary>
///  Класс, описывающий поведение фигуры.
/// </summary>
public class FigureController : MonoBehaviour
{
    // Сериализованный контроллер сцены для добавления в редакторе.
    [SerializeField] private SceneController sceneController;
    [SerializeField] private GameObject rotator;

    // Скорость движения фигуры вниз.
    [SerializeField] private float dropSpeed = 1;
    // Ускоренное движение фигуры вниз.
    [SerializeField] private float speedMultiplier = 1;
    // Допустимое время неподвижности фигуры в секундах.
    [SerializeField] private float dropTime = 1;
    // Счетчик времени после последнего сдвига фигуры вниз в секундах.
    private float timer = 0;
    // Угол вращения фигуры.
    private float angle = 90;
    // Исходное положение фигуры.
    private Vector3 startPostition;

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
        startPostition = SceneController.spawnPosition;
        transform.position = startPostition;
    }

    private void Update()
    {
        // Добавить время кадра к счетчику времени.
        // Время контролируется для равномерного движения фигуры вниз.
        timer += Time.deltaTime;
        // Если время превысело допустимое, передвинуть фигуру вниз и запустить таймер заново.
        if(timer >= dropTime)
        {
            MoveFigure(Directions.down);
            timer = 0;
        }

        // Если нажата клавиша, выполнить перемещение фигуры.
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveFigure(Directions.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveFigure(Directions.right);
        }

        // Если нажата кнопка вниз, уск
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveFigure(Directions.down, speedMultiplier);
        }

        // Если нажата кливаша Пробел, повернуть фигуру.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Rotate(angle);
        }
    }

    /// <summary>
    /// Метод перемещения фигуры в системе координат сцены.
    /// </summary>
    /// <param name="direction">Направление перемещения.</param>
    private void MoveFigure(Directions direction, float speedMultiplier = 1)
    {
        Vector3 movement;
        switch(direction){
            case Directions.left:
                movement = transform.TransformDirection(Vector3.left);
                if (isCorrectMove(movement))
                {
                    transform.Translate(movement);
                }
                break;

            case Directions.right:
                movement = transform.TransformDirection(Vector3.right);
                if (isCorrectMove(movement))
                {
                    transform.Translate(movement);
                }
                break;

            case Directions.down:
                movement = speedMultiplier * dropSpeed * Vector3.down;
                if (isCorrectMove(movement))
                {
                    transform.Translate(movement);
                }
                
                break;
        }
    }

    /// <summary>
    /// Метод вращения фигуры на заданный угол.
    /// </summary>
    /// <param name="angle">Угол поворота</param>
    private void Rotate(float angle)
    {
        rotator.transform.Rotate(0, 0, angle);
    }

    private bool isCorrectMove(Vector3 movement)
    {
        bool result = true;
        float xPosition = transform.position.x + movement.x;
        float yPosition = transform.position.y + movement.y;
        float edgeX = sceneController.ColumnCount / 2;
        if (xPosition < -edgeX
            || xPosition > edgeX
            || yPosition < -sceneController.RowCount / 2)
        {
            result = false;
        }
        return result;
    }

    private void OnTriggerEnter(Collider other)
    {
    }
}
