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
    private readonly float angle = 90;
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
        playerController.SideClick += move => MoveFigure(move);
        playerController.RotateClick += () => Rotate(angle);
        playerController.DownClick += () => FigureStep(settings.ExtraDropTime);
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
            // В первом режиме игры достаточно перемещать фигуру, если перемещение допустимо.
            // Во втором режиме игры нужно учесть поведение копии фигуры.
            switch (settings.Mode)
            {
                case GameSettings.Modes.FirstMode:
                    // Если перемещение допустимо, выполнить перемещение.
                    if (IsCorrectMove(movement))
                    {
                        transform.Translate(movement);
                    }
                    break;

                case GameSettings.Modes.SecondMode:
                    // Флаг доступности перемещения для копии фигуры.
                    bool isMovableClone = false;
                    if (Clone != null)
                    {
                        // Проверить допустимость перемещения копии фигуры.
                        isMovableClone = Clone.IsCorrectMove(movement);
                        // Если в результате перемещения копия фигуры была помечен упавшей, сама фигура также считается упавшей.
                        // Пометить соответствующие ячейки игрвого поля заполненными, вызвать событие падения фигуры.
                        if (Clone.IsDroped)
                        {
                            isDroped = true;
                            blockManager.FillBlocks();
                            FigureDroped?.Invoke();
                        }
                    }

                    // Если для копии фигуры позволнено то же перемещение, что и для самой фигуры, выполнить перемещение.
                    if (isMovableClone && IsCorrectMove(movement))
                    {
                        transform.Translate(movement);

                        Clone.transform.Translate(movement);
                    }
                    break;

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
            if (!IsCorrectMove(Vector3.zero))
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
    public bool IsCorrectMove(Vector3 movement)
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

            // Если перемещение было вниз, и достигли последней линии либо произошло столкновение,
            // то перемещение недопустимо, а фигура почемается упавшей.
            if (movement.y != 0 &&
               (rowIndex < 0 || sceneController.Cells[rowIndex, columnIndex]))
            {
                isDroped = true;
                result = false;
                break;
            }

            // Для первого режима:
            // Если перемещение было вбок (или поворот), и произошло столкновение, либо достигли боковой линии,
            // перемещение недопустимо, но фигура может продолжить падать.
            if (settings.Mode == GameSettings.Modes.FirstMode &&
               (sceneController.Cells[rowIndex, columnIndex] ||
                columnIndex < 0 || columnIndex >= width))
            {
                result = false;
                break;
            }

            // Если во втором режиме игры хотя бы один блок копии фигуры (самой фигуры) удалился от границы игрового поля на расстояние превышающее ширину игрового поля,
            // перебросить фигуру (или ее копию) на сторону игрвого поля противоположную пересеченной. 
            // В результате будет выглядеть так, будто фигура вышла через одну границу, а появилась через другую.
            if (settings.Mode == GameSettings.Modes.SecondMode &&
                (xPosition >= 2 * width || xPosition < -width))
            {
                result = MoveToSide(xPosition);
                break;

            }

            // Для второго режима:
            // Если перемещение было вбок (или поворот), и произошло столкновение,
            // перемещение недопустимо, но фигура может продолжить падать.
            if (settings.Mode == GameSettings.Modes.SecondMode &&
                sceneController.Cells[rowIndex, columnIndex])
            {
                result = false;
                break;
            }
        }
        // Отменить перемещение независимо от результата проверки.
        transform.position -= movement;
        // Если фигура упала, отметить заполненные ячейки игрового поля, сгенерировать событие падения.
        if (isDroped)
        {
            blockManager.FillBlocks();
            FigureDroped?.Invoke();
            // Если данная фигура - не клон, обратиться к ее клону и пометить его также упавшим.
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
    /// <param name="xPosition">Текущее положение фигуры по гризонтали</param>
    /// <returns>Вернет истину, если перемещение допустимо.</returns>
    private bool MoveToSide(float xPosition)
    {
        bool result = false;
        Vector3 movement;
        // Если фигура покинула игровое поле через правую границу, то переместить ее к левой, иначе - к правой.
        if (xPosition >= 2 * settings.ColumnCount)
        {
            movement = 2 * settings.ColumnCount * Vector3.left;
        }
        else
        {
            movement = 2 * settings.ColumnCount * Vector3.right;
        }

        // Если рассчитанное перемещение допустимо, выполнить его.
        if (IsCorrectMove(movement))
        {
            transform.Translate(movement);
            result = true;
        }
        return result;
    }
}