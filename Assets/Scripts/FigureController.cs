using System;
using UnityEngine;

/// <summary>
///  Класс для управления фигурой.
/// </summary>
public class FigureController : MonoBehaviour
{
    /// <summary>
    /// Экземпляр класса контроллера игрока.
    /// </summary>
    private PlayerController playerController = null;
    /// <summary>
    /// Экземпляр класса контроллера сцены.
    /// </summary>
    private SceneController sceneController = null;
    /// <summary>
    /// Экземпляр класса настроек игры.
    /// </summary>
    private GameSettings settings = null;
    /// <summary>
    /// Дополнлительная система координат для вращения фигуры.
    /// </summary>
    [SerializeField] private GameObject rotator;
    /// <summary>
    /// Логический флаг для проверки завершенности падения фигуры.
    /// </summary>
    private bool isDroped = false;

    private float angle = 90; // угол вращения фигуры
    private float timer = 0; // счетчик времени после последнего сдвига фигуры вниз в секундах

    /// <summary>
    /// Событие окончания падения фигуры.
    /// </summary>
    public event Action FigureDroped;

    /// <summary>
    /// Свойство получения и установки клона фигуры.
    /// </summary>
    public FigureController Clone
    {
        get;
        set;
    }

    /// <summary>
    /// Свойство изменяющее и устанавливающее логический флаг падения фигуры.
    /// </summary>
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
        // Установить ссылки на необходимые экземпляры классов для взаимодействия с ними.

        sceneController = SceneController.Instance;
        settings = GameSettings.Instance;
        playerController = PlayerController.Instance;
    }

    private void Start()
    {
        // Зарегистрировать обработчики событий, происходящих в других классах.

        sceneController.LineDestroy += OnLineDestroy;
        sceneController.LinesShift += OnLinesShift;
        playerController.SideClick += MoveFigure;
        playerController.RotateClick += () => Rotate(angle);
        playerController.DownClick += () => FigureStep(settings.ExtraDropTime); ;
    }

    private void Update()
    {
        // Если фигура еще не упала, сдвинуть ее вниз на одну клетку.
        if (!isDroped)
        {
            FigureStep(settings.DropTime);
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
            // Если для клона фигуры позволнено то же перемещение, что и для самой фигуры, выполнить перемещение.
            if (isMovableClone(movement) && isCorrectMove(movement))
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
    public bool isCorrectMove(Vector3 movement)
    {
        bool result = true; // результат проверки
        int width = settings.ColumnCount; // ширина игрового поля (максимально допустимая координата)

        // Выполнить перемещение, чтобы првоерить его допустимость.
        transform.position += movement;
        // Перебрать все блоки фигуры, чтобы определить, не залез ли любой из блоков в недопустимую зону.
        foreach (Transform block in rotator.transform)
        {
            float xPosition = block.position.x;
            float yPosition = block.position.y;
            int rowIndex = Mathf.FloorToInt(yPosition); // индекс строки блока
            int columnIndex = Mathf.FloorToInt(xPosition); // индекс столбца блока

            // Если блок перемещается вбок и встречает препятствие, перемещение недопустимо.
            if (movement.y == 0 && sceneController.Cells[rowIndex, columnIndex])
            {
                result = false;
                break;
            }

            // Если достигли земли или другой упавшей фигуры, пометить текущую фигуру упавшей.
            if (yPosition <= 0 ||
                sceneController.Cells[rowIndex, columnIndex])
            {
                isDroped = true;
                result = false;
                break;
            }

            // Если в первом режиме игры хотя бы один блок достиг края игрвого поля, перемещение недопустимо.
            if (settings.Mode == GameSettings.Modes.firstMode &&
               (xPosition >= width || xPosition < 0))
            {
                result = false;
                break;
            }

            // Если во втором режиме игры хотя бы один блок фигуры удалился от своей копии на расстояние двукратно превышающее границу игрового поля,
            // перебросить фигуру (или ее копию) на сторону игрвого поля обратную пересеченной. 
            // В результате будет выглядеть так, будто фигура вышла через одну границу, а появилась через другую.
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

    /// <summary>
    /// Заполнить ячейки игрвого поля значениями true в местах блоков упавшей фигуры.
    /// </summary>
    private void FillBlocks()
    {
        foreach (Transform block in rotator.transform)
        {
            int rowNumber = Mathf.FloorToInt(block.position.y);
            int columnNumber = Mathf.FloorToInt(block.position.x);
            sceneController.Cells.SetCell(rowNumber, columnNumber);
        }
    }

    /// <summary>
    /// Обработчик события удаления линии с определенным номером.
    /// </summary>
    /// <param name="lineNumber">Линия для удаления.</param>
    private void OnLineDestroy(int lineNumber)
    {
        // Перебрать все блоки фигуры.
        foreach (Transform block in rotator.transform)
        {
            // Если блок расположен в линии для удаления, уничтожить его.
            if (Mathf.FloorToInt(block.position.y) == lineNumber)
            {
                Destroy(block.gameObject);
            }
        }
    }

    /// <summary>
    /// Передвинуть все блоки фигуры, расположенные выше определенной линии
    /// </summary>
    /// <param name="lineNumber">Номер линии, ближе к которой сместятся блоки выше нее.</param>
    private void OnLinesShift(int lineNumber)
    {
        foreach (Transform block in rotator.transform)
        {
            // Если блок расположен выше линии, то сдвинуть его на единицу вниз.
            if (Mathf.FloorToInt(block.position.y) > lineNumber)
            {
                block.transform.position += Vector3.down;
            }
        }
    }

    /// <summary>
    /// Скрыть блоки вне игрового поля.
    /// </summary>
    private void HideBlocks()
    {
        foreach (Transform block in rotator.transform)
        {
            float positionX = block.position.x;
            // Если позиция блока фигуры выходит за границы игрового поля по горизонтали, сделать его невидимым, иначе - сделать видимым.
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

    /// <summary>
    /// Перебросить фигуру или ее копию от одной границы игрового поля к противоположной.
    /// </summary>
    /// <param name="xPosition"></param>
    private void MoveToSide(float xPosition)
    {
        // Если фигура покинула игровое поле через правую границу, то переместить ее к левой, иначе - к правой.
        if (xPosition >= 2 * settings.ColumnCount)
        {
            transform.Translate(2 * settings.ColumnCount * Vector3.left);
        }
        else
        {
            transform.Translate(2 * settings.ColumnCount * Vector3.right);
        }
    }

    /// <summary>
    /// Проверить, может ли копия фигуры переместиться также, как и сама фигура.
    /// </summary>
    /// <param name="movement">Величина перемещения.</param>
    /// <returns>Вернется истина, если перемещение доступно.</returns>
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
}