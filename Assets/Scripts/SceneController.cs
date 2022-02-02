using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using goshanoob.Tetris;

/// <summary>
/// Контроллер игрового поля.
/// </summary>
public class SceneController : MonoBehaviour
{
    /// <summary>
    /// Генаратор случайных номеров фигур в соответствии с вероятностями выпадения.
    /// </summary>
    private Randomizer figureRandoms;

    [SerializeField] private GameSettings settings = null; // экземпляр класса настроек игры
    [SerializeField] private GameObject ground = null; // объект игрового поля
    [SerializeField] private PlayerController player = null; // экземпляр класса для работы с игроком (ввод, очки)
    [SerializeField] private GUIController gui = null; // объект графического интерфейса
    [SerializeField] private Camera mainCamera = null; // объект камеры
    [Header("Префабы фигур")]
    [SerializeField] private GameObject[] figures = null; // массив объектов фигур

    /// <summary>
    /// Событие, вызывающее уничтожение заполненных линий.
    /// </summary>
    public event Action<int> LineDestroy;

    /// <summary>
    /// Событие, вызывающее сдвиг линий, находящихся выше уничтоженной.
    /// </summary>
    public event Action<int> LinesShift;

    /// <summary>
    /// Событие окончания игры.
    /// </summary>
    public event Action GameOver;

    /// <summary>
    /// Свойство для доступа к заполненным ячейкам игрвого поля.
    /// </summary>
    public CellingField Cells
    {
        get;
        set;
    }

    /// <summary>
    /// Свойсто для доступа к экземпляру данного класса.
    /// </summary>
    public static SceneController Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        // Инициализировать экземпляр текущего класса для доступа из других классов.
        Instance = this;
        // Обработать смену игрвого режима и нажатие на "рестарт", перезапустив сцену.
        settings.GameModeChanged += OnRestartClicked;
        /// Обработать нажатие на кнопку перезапуска в графическом меню.
        gui.RestartClicked += OnRestartClicked;
    }

    private void Start()
    {
        // Инициализировать массив логических значений, в котором true - заполненная ячейка.
        // Размерность с запасом из-за положения фигур над игровым полем до выпадания.
        Cells = new CellingField(settings.RowCount + 3, settings.ColumnCount);
        // Если выбран второй режим игры, изменить внешний вид игрового поля.
        if (settings.Mode == GameSettings.Modes.secondMode)
        {
            SetUpGround();
        }
        // Подготовить фигуры к случайному выпадению на основе вероятностей.
        CreateFigures();
        // Вызвать случайную фигуру на сцену.
        SpawnNewFigure();
    }

    /// <summary>
    /// Настроить размер и положение игрового поля в зависимости от выбранного режима.
    /// </summary>
    private void SetUpGround()
    {
        int columns = settings.ColumnCount;
        int rows = settings.RowCount;
        float halfColumns = columns / 2;
        float halfRows = rows / 2;

        // Задать размер и положение игрвого поля на сцене.
        ground.transform.localScale = new Vector3(columns, rows, 1);
        ground.transform.position = new Vector3(halfColumns, halfRows, 0);
        // Поместить камеру над центром поля.
        mainCamera.transform.position = new Vector3(halfColumns, halfRows, -100);
    }

    /// <summary>
    /// Подготовить массив вероятностей выпадения фигур.
    /// </summary>
    private void CreateFigures()
    {
        // Получить массив вероятностей выпадения фигур.
        int figuresCount = (int)settings.Mode;
        double[] probabilities = new double[figuresCount];
        for (int i = 0; i < figuresCount; i++)
        {
            probabilities[i] = figures[i].GetComponent<Figure>().Probability;
        }

        // Если выбран второй режим игры, изменить вероятность выпадения седьмой фигуры.
        if (settings.Mode == GameSettings.Modes.secondMode)
        {
            // Убедиться, что седьмая фигура подключена в седьмую ячейку в редакторе.
            Figure7 figure7 = figures[6].GetComponent<Figure7>();
            if (figure7 != null)
            {
                probabilities[6] = figure7.SecondProbability;
            }
        }
        // Получить экземпляр структуры для генерации случайных значений с учетом их вероятности.
        figureRandoms = new Randomizer(probabilities);
    }

    /// <summary>
    /// Получить случайную фигуру из массива фигур.
    /// </summary>
    /// <returns></returns>
    private GameObject GetRandomFigure() => figures[figureRandoms.GetNextNumber()];

    /// <summary>
    /// Добавить случайную фигуру на сцену, а также ее копию.
    /// </summary>
    private void SpawnNewFigure()
    {
        GameObject figure = GetRandomFigure();
        GameObject newFigure = Instantiate(figure, settings.SpawnPosition, Quaternion.identity);
        // Сообщить экземпляру фигуры о текущем контроллере.
        FigureController figureContoller = newFigure.GetComponent<FigureController>();

        // Зарегистрировать обработчкик события падения фигуры на дно игрового поля.
        figureContoller.FigureDroped += OnFigureDroped;

        // Если выбран второй режим игры, создать фигуру, дублирующую выпавшую.
        if (settings.Mode == GameSettings.Modes.secondMode)
        {
            // Позиция фигуры-копии симметрична относительно левой границы игрового поля.
            Vector3 clonePosition = settings.SpawnPosition + Vector3.left * settings.ColumnCount;
            GameObject figureClone = Instantiate(figure, clonePosition, Quaternion.identity);
            FigureController figureCloneController = figureClone.GetComponent<FigureController>();

            // Сохранить ссылку на клон в классе фигуры-оригинала для взаимосвязанного движения.
            figureContoller.Clone = figureCloneController;
        }
    }

    /// <summary>
    /// Удалить заполненные линии.
    /// </summary>
    private void CheckLines()
    {
        for (int i = 0; i < settings.RowCount; i++)
        {
            // Проверить заполненность линий. Достаточное число линий для удаления зависит от выбранного режима.
            bool willDestroy = true;
            for (int k = 0; k < settings.LinesForDestroy; k++)
            {
                willDestroy = willDestroy && Cells.CheckLine(i + k);
            }
            // Если число заполненных линий достаточно, удалить каждую из них по очереди.
            if (willDestroy)
            {
                for (int k = 0; k < settings.LinesForDestroy; k++)
                {
                    // Вызвать событие удаления i-й линии у каждой фигуры на сцене.
                    LineDestroy?.Invoke(i);
                    // Сдвинуть ячейки в массиве заполненных ячеек.
                    Cells.ShiftLines(i);
                    // Вызвать событие сдвига линий выше i-й у каждой фигуры.
                    LinesShift?.Invoke(i);
                    player.Score++;
                }
                i--;
            }
        }
    }

    /// <summary>
    /// Обработать событие падения фигуры.
    /// </summary>
    private void OnFigureDroped()
    {
        // Проверить, не появились ли заполненные линии.
        CheckLines();
        // Если настал конец игры, завершить создание новых фигур.
        if (CheckGameOver())
        {
            return;
        }
        // Сгенерировать новую фигуру.
        SpawnNewFigure();
    }

    /// <summary>
    /// Перезагрузить текущую сцену.
    /// </summary>
    private void OnRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Проверить условие окончания игры.
    /// </summary>
    private bool CheckGameOver()
    {
        bool result = false;
        for (int i = 0; i < settings.ColumnCount; i++)
        {
            // Если хотя бы одна заполненная ячейка игрового поля оказалась выше видимой его части на 2 позиции, вызвать событие окончания игры.
            if (Cells[settings.RowCount + 1, i])
            {
                result = true;
                GameOver();
                break;
            }
        }
        return result;
    }
}
