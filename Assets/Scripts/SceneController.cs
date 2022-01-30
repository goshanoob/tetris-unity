using System;
using goshanoob.Tetris;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///  Класс, управляющий игровым полем.
/// </summary>
public class SceneController : MonoBehaviour
{
    // Количество сторок игрового поля.
    private int rowCount = 20;
    // Количество столбцов игрового поля.
    private int columnCount = 10;
    private int ectraColumnCount = 12;
    // Генаратор случайных фигур в соответствии с вероятностями выпадения.
    private Randomizer figureRandoms;

    [SerializeField] private GameSettings settings = null;
    [SerializeField] private GameObject ground = null;
    [SerializeField] private PlayerController palyer = null;
    [SerializeField] private GUIController gui = null;
    [Header("Префабы фигур")]
    [SerializeField] private GameObject[] figures = null;

    // Событие, вызывающее уничтожение заполненных линий.
    public event Action<int> LineDestroy;
    // Событие, вызывающее сдвиг линиц, находящихся выше уничтоженной.
    public event Action<int> LinesShift;
    public int RowCount
    {
        get => rowCount;
        set => rowCount = value;
    }
    public int ColumnCount
    {
        get => columnCount;
        set => columnCount = value;
    }
    // Место появления новых фигур.
    public Vector3 SpawnPosition
    {
        get;
        private set;
    }
    // Автоматическое свойство для работы с заполненными ячейками игрвого поля.
    public CellingField Cells
    {
        get;
        set;
    }
    public static SceneController Instance { 
        get; 
        private set; 
    }

    private void Awake()
    {
        
        settings.GameModeChanged += mode =>
        {
            OnRestartClicked();
            
        };
        //currentMode = mode;
        gui.RestartClicked += OnRestartClicked;
        Instance = this;
        Cells = new CellingField(RowCount + 3, ColumnCount);
        SpawnPosition = new Vector3(columnCount / 2, rowCount , 0);
    }

    private void Start()
    {
        // Расширить игровое поле.
        if(settings.Mode == GameSettings.Modes.secondMode)
        {
            SetWidth(ectraColumnCount);
            ColumnCount = ectraColumnCount;
        }
        // Создать фигуры.
        CreateFigures();
        // Сгенерировать фигуру на сцене.
        SpawnNewFigure();
    }

    private void SetWidth(int width)
    {
        ground.transform.localScale = new Vector3(width, rowCount, 1);
    }

    private void CreateFigures()
    {
        int figuresCount = (int)settings.Mode;
        double[] probabilities = new double[figuresCount];

        for (int i = 0; i < figuresCount; i++)
        {
            probabilities[i] = figures[i].GetComponent<Figure>().Probability;
        }

        if (settings.Mode == GameSettings.Modes.secondMode)
        {
            probabilities[6] = figures[6].GetComponent<Figure7>().SecondProbability;
        }
            // Получить экземпляр структуры для генерации случайных значений с учетом их вероятности.
            figureRandoms = new Randomizer(probabilities);
    }

    /// <summary>
    /// Метод создания случайной фигуры.
    /// </summary>
    private void SpawnNewFigure()
    {
        // Создать фигуру со случайным номером.
        int figureNumber = figureRandoms.GetNextNumber();
        GameObject newFigure = Instantiate(figures[figureNumber], SpawnPosition, Quaternion.identity);

        // Зарегистрировать обработчкик события падения фигуры на дно игрового поля.
        FigureController figureContoller = newFigure.GetComponent<FigureController>();
        figureContoller.FigureDroped += OnFigureDroped;
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
    /// Проверить заполненность линий.
    /// </summary>
    private void CheckLines()
    {
        for (int i = 0; i < RowCount; i++)
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
    }

    public void SpawnNewFigure2(GameObject originFigure)
    {
        Vector3 newPosition = originFigure.transform.position + columnCount * Vector3.left;
        GameObject newFigure = Instantiate(originFigure, newPosition, Quaternion.identity);
        // Сообщить экземпляру фигуры о текущем контроллере.
        FigureController figureContoller = newFigure.GetComponent<FigureController>();
        figureContoller.isClone = true;
    }

    private void OnRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
