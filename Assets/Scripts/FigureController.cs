using System;
using UnityEngine;

/// <summary>
///  Класс для управления фигурой.
/// </summary>
internal class FigureController : MonoBehaviour
{
    // Игровой объект для вращения фигуры.
    [SerializeField] protected GameObject rotator;
    // Допустимое время неподвижности фигуры в секундах.
    [SerializeField] private float dropTime = 0.7f;
    [SerializeField] private float extraDropTime = 0.1f;

    // Контроллер сцены.
    protected SceneController sceneController = null;
    protected bool isDroped = false;

    protected GameSettings settings = null;
    // Контроллер игрока.
    private PlayerController playerController = null;

    // Угол вращения фигуры.
    private float angle = 90;

    // Счетчик времени после последнего сдвига фигуры вниз в секундах.
    private float timer = 0;

    // Событие окончания падения фигуры.
    public event Action FigureDroped;

    public FigureController Clone
    {
        get;
        set;
    }

    // Фиксирование фигуры после падения
    public bool IsDroped
    {
        get
        {
            return isDroped;
        }
        set
        {
            isDroped = value;
        }
    }

    private void Awake()
    {
        sceneController = SceneController.Instance;
        settings = GameSettings.Instance;
        playerController = PlayerController.Instance;
    }

    private void Start()
    {
        sceneController.LineDestroy += OnLineDestroy;
        sceneController.LinesShift += OnLinesShift;
        playerController.ButtonClick += MoveFigure;
        playerController.RotateClick += () => Rotate(angle);
        playerController.DownClick += () => FigureStep(extraDropTime); ;
    }

    private void Update()
    {
        // Если фигура еще не упала, сдвинуть ее вниз на одну клетку.
        if (!isDroped)
        {
            FigureStep(dropTime);
        }
        // Скрыть блоки вне игрвого поля.
        HideBlocks();
    }

    private void FigureStep(float maxTime)
    {
        // Добавить время кадра к счетчику времени.
        // Время контролируется для равномерного движения фигуры вниз.
        timer += Time.deltaTime;
        // Если время превысело допустимое, передвинуть фигуру вниз и запустить таймер заново.
        if (!isDroped && timer >= maxTime)
        {
            MoveFigure(Vector3.down);
            timer = 0;
        }
    }

    /// <summary>
    /// Метод перемещения фигуры.
    /// </summary>
    /// <param name="movement">Относительная величина перемещения</param>
    private void MoveFigure(Vector3 movement)
    {
        if (!isDroped)
        {
            if (isMovableClone(movement) && isCorrectMove(movement))
            {
                transform.Translate(movement);
            }
        }
    }

    private bool isMovableClone(Vector3 movement)
    {
        bool result = true;
        if (Clone != null)
        {
            result = Clone.isCorrectMove(movement);
            if (!result)
            {
                isDroped = true;
                FillBlocks();
                FigureDroped?.Invoke();
            }
        }
        return result;
    }

    /// <summary>
    /// Метод вращения фигуры на заданный угол.
    /// </summary>
    /// <param name="angle">Угол поворота</param>
    private void Rotate(float angle)
    {
        if (!isDroped)
        {
            rotator.transform.Rotate(0, 0, angle);
            // Если выполненный поворот недопустим, отменить его.
            // Величина поступательного перемещения, необходимая методу проверки, отсутствует.
            if (!isCorrectMove(Vector3.zero))
            {
                rotator.transform.Rotate(0, 0, -angle);
            }
        }
    }

    /// <summary>
    /// Метод проверки допустимости перемещения фигуры.
    /// </summary>
    /// <param name="movement">Величина перемещения.</param>
    /// <returns></returns>
    public bool isCorrectMove(Vector3 movement)
    {
        // Результат проверки.
        bool result = true;
        // Расстояние от центра игрового поля до крайних положений по горизонтали и вертикали.
        int width = settings.ColumnCount;
        // Выполнить перемещение, чтобы првоерить его допустимость.
        transform.position += movement;
        // Перебрать все блоки фигуры.
        foreach (Transform block in rotator.transform)
        {
            float xPosition = block.position.x;
            float yPosition = block.position.y;
            int rowIndex = Mathf.FloorToInt(yPosition);
            int columnIndex = Mathf.FloorToInt(xPosition);

            // Если достигли земли или другой упавшей фигуры, пометить текущую фигуру упавшей.
            if (yPosition <= 0 ||
                sceneController.Cells[rowIndex, columnIndex])
            {
                isDroped = true;
                result = false;
                break;
            }

            // Если хотя бы один блок достиг края игрвого поля, перемещение недопустимо.
            if (settings.Mode == GameSettings.Modes.firstMode &&
               (xPosition >= width || xPosition < 0))
            {
                result = false;
                break;
            }

            if (settings.Mode == GameSettings.Modes.secondMode &&
                (xPosition >= 2 * width || xPosition < -width))
            {
                MoveToSide(xPosition);
                break;
            }


        }
        // Отменить перемещение независимо от результата проверки.
        transform.position -= movement;
        // Если фигура упала, отметить заполненные ячейки игрового поля, сгенерировать событие падения, уничтожить невидимую копию фигуры.
        if (isDroped)
        {
            FillBlocks();
            FigureDroped?.Invoke();
            if (Clone != null)
            {
                Clone.IsDroped = true;
            }
        }
        return result;
    }

    protected void FillBlocks()
    {
        foreach (Transform block in rotator.transform)
        {
            int rowNumber = Mathf.FloorToInt(block.position.y);
            int columnNumber = Mathf.FloorToInt(block.position.x);
            sceneController.Cells.SetCell(rowNumber, columnNumber);
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

    private void HideBlocks()
    {
        foreach (Transform block in rotator.transform)
        {
            float positionX = block.position.x;
            if (positionX >= settings.ColumnCount ||
                positionX < 0)
            {
                block.gameObject.SetActive(false);
            }
            else
            {
                block.gameObject.SetActive(true);
            }
        }
    }

    private void MoveToSide(float xPosition)
    {
        if (xPosition >= 2 * settings.ColumnCount)
        {
            transform.Translate(2 * settings.ColumnCount * Vector3.left);
        }
        else
        {
            transform.Translate(2 * settings.ColumnCount * Vector3.right);
        }
    }
}