using System;
using UnityEngine;

/// <summary>
///  Класс, описывающий поведение фигуры.
/// </summary>
internal class FigureController : MonoBehaviour
{
    // Игровой объект для вращения фигуры.
    [SerializeField] private GameObject rotator;
    // Допустимое время неподвижности фигуры в секундах.
    [SerializeField] private float dropTime = 0.7f;
    [SerializeField] private float extraDropTime = 0.1f;

    // Контроллер сцены.
    private SceneController sceneController = null;
    // Счетчик времени после последнего сдвига фигуры вниз в секундах.
    private float timer = 0;
    // Угол вращения фигуры.
    private float angle = 90;
    // Исходное положение фигуры.
    private Vector3 startPostition;
    // Фиксирование фигуры после падения.
    private bool isDroped = false;

    // Событие окончания падения фигуры.
    public event EventHandler FigureDroped;
    // Свойство для привязки к контроллеру сцены.

    /// <summary>
    /// Возможные направления перемещения фигуры.
    /// </summary>
    private enum Directions
    {
        left = 0,
        right = 1,
        down = 2,
    }

    private void Awake()
    {
        sceneController = SceneController.Instance;
    }

    private void Start()
    {
        startPostition = sceneController.SpawnPosition;
        transform.position = startPostition;

        // Регистрация обработчиков события уничтожения линии на сцене и сдвига верхних линий.
        sceneController.LineDestroy += OnLineDestroy;
        sceneController.LinesShift += OnLinesShift;
    }

    private void FigureStep(float maxTime)
    {
        // Добавить время кадра к счетчику времени.
        // Время контролируется для равномерного движения фигуры вниз.
        timer += Time.deltaTime;
        // Если время превысело допустимое, передвинуть фигуру вниз и запустить таймер заново.
        if (timer >= maxTime)
        {
            MoveFigure(Directions.down);
            timer = 0;
        }
    }

    private void Update()
    {
        if (!isDroped)
        {
            // Сделать фигурой шаг вниз на одну клетку, если требуется.
            FigureStep(dropTime);
            // Проверить нажатие кнопок.
            bool left = Input.GetKeyDown(KeyCode.LeftArrow);
            bool right = Input.GetKeyDown(KeyCode.RightArrow);
            bool down = Input.GetKey(KeyCode.DownArrow);
            bool space = Input.GetKeyDown(KeyCode.Space);

            // Если нажата клавиша, выполнить перемещение фигуры.
            if (left)
            {
                MoveFigure(Directions.left);
            }
            else if (right)
            {
                MoveFigure(Directions.right);
            }

            // Если удерживается кнопка вниз, ускорить движение.
            if (down)
            {
                FigureStep(extraDropTime);
            }

            // Если нажата клавиша Пробел, повернуть фигуру.
            if (space)
            {
                Rotate(angle);
            }

        }
    }

    /// <summary>
    /// Метод перемещения фигуры.
    /// </summary>
    /// <param name="direction">Направление перемещения</param>
    private void MoveFigure(Directions direction)
    {
        Vector3 movement;
        switch (direction)
        {
            case Directions.left:
                movement = Vector3.left;
                if (isCorrectMove(movement))
                {
                    transform.Translate(movement);
                }
                break;

            case Directions.right:
                movement = Vector3.right;
                if (isCorrectMove(movement))
                {
                    transform.Translate(movement);
                }
                break;

            case Directions.down:
                movement = Vector3.down;
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
        // Если выполненный поворот недопустим, отменить его.
        // Величина поступательного перемещения, необходимая методу проверки, отсутствует.
        if (!isCorrectMove(Vector3.zero))
        {
            rotator.transform.Rotate(0, 0, -angle);
        }
    }

    /// <summary>
    /// Метод проверки допустимости перемещения фигуры.
    /// </summary>
    /// <param name="movement">Величина перемещения.</param>
    /// <returns></returns>
    private bool isCorrectMove(Vector3 movement)
    {
        // Результат проверки.
        bool result = true;

        // Расстояние от центра игрового поля до крайних положений по горизонтали и вертикали.
        int width = sceneController.ColumnCount;

        // Пометить клетку как незаполненную.
        bool isFilledCell = false;
        // Выполнить перемещение, чтобы првоерить его допустимость.
        transform.position += movement;
        // Перебрать все блоки фигуры.
        foreach (Transform block in rotator.transform)
        {
            float xPosition = block.position.x;
            float yPosition = block.position.y;

            isFilledCell = sceneController.Cells[(int)yPosition, (int)xPosition];

            // Если достигли земли или другой упавшей фигуры, пометить текущую фигуру упавшей.
            // Вызвать событие для генерации следующей фигуры.
            if (yPosition <= 0 ||
                isFilledCell)
            {
                isDroped = true;
                result = false;
                break;
            }

            // Если хотя бы один блок достиг края игрвого поля, перемещение недопустимо.
            if (xPosition >= width ||
                xPosition < 0)
            {
                result = false;
                break;
            }
        }
        // Отменить перемещение независимо от результата проверки.
        transform.position -= movement;
        // Если фигура упала, отметить заполненные ячейки игрового поля.
        if (isDroped)
        {
            FillBlocks();
            FigureDroped?.Invoke(this, EventArgs.Empty);
        }
        return result;
    }

    private void FillBlocks()
    {
        foreach (Transform block in rotator.transform)
        {
            int rowNumber = (int)block.position.y;
            int columnNumber = (int)block.position.x;
            sceneController.Cells[rowNumber, columnNumber] = true;
        }
    }

    private void OnLineDestroy(int lineNumber)
    {
        foreach (Transform block in rotator.transform)
        {
            if ((int)block.position.y == lineNumber)
            {
                Destroy(block.gameObject);
            }
        }
    }

    private void OnLinesShift(int lineNumber)
    {
        foreach (Transform block in rotator.transform)
        {
            if ((int)block.position.y > lineNumber)
            {
                // Изаменить абсолютные координаты фигуры.
                block.transform.position += Vector3.down;
            }
        }
    }

    private void CreateBlock()
    {
        sceneController.SpawnNewFigure2(gameObject);
    }
}
