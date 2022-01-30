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

    private GameSettings settings = null;
    // Контроллер игрока.
    private PlayerController playerController = null;
    // Счетчик времени после последнего сдвига фигуры вниз в секундах.
    private float timer = 0;
    // Угол вращения фигуры.
    private float angle = 90;
    // Фиксирование фигуры после падения.
    private bool isDroped = false;
    private bool firstTime = true;
    public bool isClone = false;
    // Событие окончания падения фигуры.
    public event Action FigureDroped;


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

    private void FigureStep(float maxTime)
    {
        // Добавить время кадра к счетчику времени.
        // Время контролируется для равномерного движения фигуры вниз.
        timer += Time.deltaTime;
        // Если время превысело допустимое, передвинуть фигуру вниз и запустить таймер заново.
        if (timer >= maxTime && !isDroped)
        {
            MoveFigure(Vector3.down);
            timer = 0;
        }
    }

    private void Update()
    {
        // Если фигура еще не упала, сдвинуть ее вниз на одну клетку.
        if (!isDroped)
        {
            FigureStep(dropTime);
        }

        CheckTeleportation();
        OutLiteBlock();
    }

    /// <summary>
    /// Метод перемещения фигуры.
    /// </summary>
    /// <param name="movement">Относительная величина перемещения</param>
    private void MoveFigure(Vector3 movement)
    {
        if (!isDroped)
        {
            if (isCorrectMove(movement))
            {
                transform.Translate(movement);
            }
        }
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
            if (settings.Mode == GameSettings.Modes.firstMode &&
               (xPosition >= width || xPosition < 0))
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
            FigureDroped?.Invoke();
            DestroyFigure();
        }

        return result;
    }

    private void FillBlocks()
    {
        foreach (Transform block in rotator.transform)
        {
            int rowNumber = (int)block.position.y;
            int columnNumber = (int)block.position.x;
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


    private void CheckTeleportation()
    {
        foreach (Transform block in rotator.transform)
        {
            float xPosition = block.position.x;
            float yPosition = block.position.y;
            int width = sceneController.ColumnCount;

            if (xPosition > width ||
                    xPosition <= 0)
            {
                if (firstTime && !isClone)
                {
                    CloneBlock();
                    firstTime = false;
                }
            }
        }
    }

    private void CloneBlock()
    {
        sceneController.SpawnNewFigure2(gameObject);
    }

    private void OutLiteBlock()
    {
        foreach (Transform block in rotator.transform)
        {
            int width = sceneController.ColumnCount;
            if (block.position.x > width || block.position.x <= 0)
            {
                block.gameObject.SetActive(false);
            }
            else
            {
                block.gameObject.SetActive(true);
            }
        }
    }

    private void DestroyFigure()
    {
        foreach (Transform block in rotator.transform)
        {
            if (block.gameObject.activeSelf)
            {
                return;
            }
        }
        Destroy(gameObject);
    }
}
