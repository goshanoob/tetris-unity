using System;
using goshanoob.Tetris;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///  Класс, управляющий игровым полем.
/// </summary>
public class SceneController : MonoBehaviour
{
    
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
    public static SceneController Instance { 
        get; 
        private set; 
    }

    private void Awake()
    {
        Instance = this;
        // Обработать смену игрвого режима и нажатие на "рестарт", перезапустив сцену.
        settings.GameModeChanged += mode => OnRestartClicked();
        gui.RestartClicked += OnRestartClicked;
        // Инициализировать массив логических значений, в котором true - заполненная ячейка.
        // Размерность с запасом из-за положения фигур над игровым полем до выпадания.
        Cells = new CellingField(settings.RowCount + 3, settings.ColumnCount);
    }

    private void Start()
    {
        // Настроить внешний вид игрового поля.
        if(settings.Mode == GameSettings.Modes.secondMode)
        {
            SetUpGround();
        }
        // Создать фигуры.
        CreateFigures();
        // Вызвать случайную фигуру на сцену.
        SpawnNewFigure(GetRandomFigure(), settings.SpawnPosition);

    }

    private void SetUpGround()
    {
        int columns = settings.ColumnCount;
        int rows = settings.RowCount;

        ground.transform.localScale = new Vector3(columns, rows, 1);
        ground.transform.position = new Vector3(columns / 2, rows / 2, 0);
        mainCamera.transform.position = new Vector3(columns / 2, rows / 2, -100);
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

    private GameObject GetRandomFigure()
    {
        return figures[figureRandoms.GetNextNumber()];
    }

    /// <summary>
    /// Метод создания случайной фигуры.
    /// </summary>
    public void SpawnNewFigure(GameObject figure, Vector3 position, bool isClonable = false)
    {
        GameObject newFigure = Instantiate(figure, position, Quaternion.identity);
        // Сообщить экземпляру фигуры о текущем контроллере.
        FigureController figureContoller = newFigure.GetComponent<FigureController>();
        if (isClonable)
        {
            figureContoller.isClone = true;
        }
        // Зарегистрировать обработчкик события падения фигуры на дно игрового поля.
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
        SpawnNewFigure(GetRandomFigure(), settings.SpawnPosition);
    }

    /// <summary>
    /// Проверить заполненность линий.
    /// </summary>
    private void CheckLines()
    {
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
    }

    public void SpawnNewFigure2(GameObject originFigure)
    {
        Vector3 newPosition = originFigure.transform.position + settings.ColumnCount * Vector3.left;
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
