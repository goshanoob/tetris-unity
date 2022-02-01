using goshanoob.Tetris;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///  Класс, управляющий игровым полем.
/// </summary>
public class SceneController : MonoBehaviour
{
    private int counter = 0;
    // Генаратор случайных фигур в соответствии с вероятностями выпадения.
    private Randomizer figureRandoms;

    [SerializeField] private GameSettings settings = null;
    [SerializeField] private GameObject ground = null;
    [SerializeField] private PlayerController palyer = null;
    [SerializeField] private GUIController gui = null;
    [SerializeField] private Camera mainCamera = null;
    [Header("Префабы фигур")]
    [SerializeField] private GameObject[] figures = null;

    // Событие, вызывающее уничтожение заполненных линий.
    public event Action<int> LineDestroy;
    // Событие, вызывающее сдвиг линиц, находящихся выше уничтоженной.
    public event Action<int> LinesShift;

    // Автоматическое свойство для работы с заполненными ячейками игрвого поля.
    public CellingField Cells
    {
        get;
        set;
    }

    public static SceneController Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        Instance = this;
        // Обработать смену игрвого режима и нажатие на "рестарт", перезапустив сцену.
        settings.GameModeChanged += OnRestartClicked;
        gui.RestartClicked += OnRestartClicked;

    }

    private void Start()
    {
        // Инициализировать массив логических значений, в котором true - заполненная ячейка.
        // Размерность с запасом из-за положения фигур над игровым полем до выпадания.
        Cells = new CellingField(settings.RowCount + 3, settings.ColumnCount);
        // Настроить внешний вид игрового поля.
        if (settings.Mode == GameSettings.Modes.secondMode)
        {
            SetUpGround();
        }
        // Создать экземпляры фигур в памяти.
        CreateFigures();
        // Вызвать случайную фигуру на сцену.
        SpawnNewFigure();
    }

    /// <summary>
    /// Метод настройки размеров и положения игрового поля в зависимости от выбранного режима.
    /// </summary>
    private void SetUpGround()
    {
        int columns = settings.ColumnCount;
        int rows = settings.RowCount;
        float halfColumns = columns / 2;
        float halfRows = rows / 2;
        ground.transform.localScale = new Vector3(columns, rows, 1);
        ground.transform.position = new Vector3(halfColumns, halfRows, 0);
        // Поместить камеру над центром поля.
        mainCamera.transform.position = new Vector3(halfColumns, halfRows, -100);
    }

    /// <summary>
    /// 
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
            probabilities[6] = figures[6].GetComponent<Figure7>().SecondProbability;
        }
        // Получить экземпляр структуры для генерации случайных значений с учетом их вероятности.
        figureRandoms = new Randomizer(probabilities);
    }

    private GameObject GetRandomFigure() => figures[figureRandoms.GetNextNumber()];

    /// <summary>
    /// Метод создания случайной фигуры.
    /// </summary>
    public void SpawnNewFigure()
    {
        GameObject figure = GetRandomFigure();
        GameObject newFigure = Instantiate(figure, settings.SpawnPosition, Quaternion.identity);
        // Вынести две строки ниже в отдельный метод.
        // Сообщить экземпляру фигуры о текущем контроллере.
        FigureController figureContoller = newFigure.GetComponent<FigureController>();
        // Зарегистрировать обработчкик события падения фигуры на дно игрового поля.
        figureContoller.FigureDroped += OnFigureDroped;
        // Если выбран второй режим игры, создать фигуру, дублирующую выпавшую.
        if (settings.Mode == GameSettings.Modes.secondMode)
        {
            Vector3 clonePosition = settings.SpawnPosition + Vector3.left * settings.ColumnCount;
            GameObject figureClone = Instantiate(figure, clonePosition, Quaternion.identity);
            FigureController figureCloneController = figureClone.GetComponent<FigureController>();
            figureContoller.Clone = figureCloneController;
        }
    }

    /// <summary>
    /// Проверить заполненность линий.
    /// </summary>
    private void CheckLines()
    {

        /*
        for (int i = 0; i < settings.RowCount; i++)
        {
            // Если линия полностью заполнена, выполнить действия.
            if (Cells.CheckLine(i))
            {
                // Вызвать у всех фигур событие для удаления строки.
                LineDestroy?.Invoke(i);
                // Сдвинуть верхние строки на место удаленной.
                Cells.ShiftLines(i);
                i--;
                // Вызвать событие сдига блоков у каждой фигуры.
                LinesShift?.Invoke(i);
                palyer.Score++;
            }

        }
        */

        for (int i = 0; i < settings.RowCount; i++)
        {

            bool willDestroy = true;
            for (int k = 0; k < settings.LinesForDestroy; k++)
            {
                willDestroy = willDestroy && Cells.CheckLine(i + k);
            }

            if (willDestroy)
            {
                for (int k = 0; k < settings.LinesForDestroy; k++)
                {
                    LineDestroy?.Invoke(i);
                    Cells.ShiftLines(i);
                    LinesShift?.Invoke(i);
                    palyer.Score++;
                }
                i--;
            }
            
        }
    }

    /// <summary>
    /// Обработчик падения фигуры.
    /// </summary>
    private void OnFigureDroped()
    {
        // Проверить, не появились ли заполненные линии.
        CheckLines();
        // Сгенерировать новую фигуру.
        SpawnNewFigure();
    }

    /// <summary>
    /// Обработчик перезапуска игры.
    /// </summary>
    private void OnRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
