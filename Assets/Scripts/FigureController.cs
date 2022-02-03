using System;
using UnityEngine;

/// <summary>
/// Контроллер фигуры.
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
    /// Экземпляр класса управления блоками фигуры.
    /// </summary>
    [SerializeField] private BlockManager blockManager = null;
    /// <summary>
    /// Дополнительная система координат для вращения фигуры.
    /// </summary>
    [SerializeField] private GameObject rotator;
    /// <summary>
    /// Логический флаг для проверки завершенности падения фигуры.
    /// </summary>
    private bool isDroped = false;
    /// <summary>
    /// Угол вращения фигуры.
    /// </summary>
    private float angle = 90;
    /// <summary>
    /// Счетчик времени после последнего сдвига фигуры вниз в секундах.
    /// </summary>
    private float timer = 0;

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

        // Обработчкики событий удаления линий.
        sceneController.LineDestroy += blockManager.OnLineDestroy;
        sceneController.LinesShift += blockManager.OnLinesShift;

        // Обработчики событий перемещения.
        playerController.SideClick += move =>
        {
            MoveFigure(move);
        };
           
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
        blockManager.HideBlocks();
    }

    /// <summary>
    /// Вызвать падение фигуры на одну клетку в заданный момент времени.
    /// </summary>
    /// <param name="maxTime"></param>
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
    /// Переместить фигуру.
    /// </summary>
    /// <param name="movement">Относительная величина перемещения.</param>
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
    /// Повернуть фигуру на заданный угол.
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
    /// Проверить, допустимо ли перемещение фигуры.
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

            // Если во втором режиме игры хотя бы один блок копии фигуры (самой фигуры) удалился от границы игрового поля на расстояние превышающее ширину игрового поля,
            // перебросить фигуру (или ее копию) на сторону игрвого поля противоположную пересеченной. 
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
            blockManager.FillBlocks();
            FigureDroped?.Invoke();
            if (Clone != null)
            {
                Clone.IsDroped = true;
            }
        }
        return result;
    }

    /// <summary>
    /// Перебросить фигуру или ее копию от одной границы игрового поля к противоположной.
    /// </summary>
    /// <param name="xPosition">Текущее положение фигуры.</param>
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
                blockManager.FillBlocks();
                FigureDroped?.Invoke();
            }
        }
        return result;
    }
}