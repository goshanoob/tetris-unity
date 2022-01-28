using goshanoob.Tetris;
using System;
using UnityEngine;

/// <summary>
///  Класс, управляющий игровым полем.
/// </summary>
public class SceneController : MonoBehaviour
{
    // Количество сторок игрового поля.
    private int rowCount = 20;
    // Количество столбцов игрового поля.
    private int columnCount = 10;
    // Генаратор случайных фигур в соответствии с вероятностями выпадения.
    private Randomizer figureRandoms;
    // Текущий режим игры.
    private Modes currentMode = Modes.secondMode;
    // Массив для добавления префабов фигур в редакторе.
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

    /// <summary>
    /// Перечисление с режимами игры.
    /// </summary>
    private enum Modes
    {
        /// <summary>
        /// Режим с семью фигурами.
        /// </summary>
        firstMode = 7,
        /// <summary>
        /// Режим с десятью фигурами.
        /// </summary>
        secondMode = 10,
    }
   
    private void Awake()
    {
        Instance = this;
        Cells = new CellingField(RowCount + 2, ColumnCount);
        SpawnPosition = new Vector3(columnCount / 2, rowCount + 1f, 0);
    }

    private void Start()
    {
        // Создать фигуры.
        CreateFigures();
        // Сгенерировать фигуру на сцене.
        SpawnNewFigure();
    }

    private void CreateFigures()
    {
        int figuresCount = (int)currentMode;
        double[] probabilities = new double[figuresCount];
        for (int i = 0; i < figuresCount; i++)
        {
            probabilities[i] = figures[i].GetComponent<Figure>().Probability;
        }

        if (currentMode == Modes.secondMode)
        {
            figures[6].GetComponent<Figure>().Probability = 0.05;
        }

        // Получить экземпляр структуры для генерации случайных значений с учетом их вероятности.
        figureRandoms = new Randomizer(probabilities);
    }

    /// <summary>
    /// Метод создания случайной фигуры.
    /// </summary>
    private void SpawnNewFigure()
    {
        // Получить случайное значение.
        int figureNumber = figureRandoms.GetNextNumber();
        // Создать случайную фигуру.
        GameObject newFigure = Instantiate(figures[figureNumber]);
        // Сообщить экземпляру фигуры о текущем контроллере.
        FigureController figureContoller = newFigure.GetComponent<FigureController>();
        // Зарегистрировать обработчкик события падения фигуры на дно игрового поля.
        figureContoller.FigureDroped += OnFigureDroped;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFigureDroped(object sender, EventArgs e)
    {
        // Проверить, не появились ли заполненные линии.
        CheckLines();
        // Сгенерировать новую фигуру.
        SpawnNewFigure();
    }

    /// <summary>
    /// 
    /// </summary>
    private void CheckLines()
    {
        for (int i = 0; i < RowCount; i++)
        {
            // Если линия полностью заполнена, выполнить действия.
            if (Cells.CheckLine(i))
            {
                // Вызвать событие для удаления строки у всех фигур.
                LineDestroy?.Invoke(i);
                // Сдвинуть верхние строки на место удаленной.
                Cells.ShiftLines(i);
                i--;
                // Вызвать событие сдига блоков у каждой фигуры.
                LinesShift?.Invoke(i);
            }

        }
    }

    public void SpawnNewFigure2(GameObject originFigure)
    {
        GameObject newFigure = Instantiate(originFigure, originFigure.transform.position - new Vector3(10, 0, 0), new Quaternion());
        // Сообщить экземпляру фигуры о текущем контроллере.
        FigureController figureContoller = newFigure.GetComponent<FigureController>();
        //newFigure.transform.position += new Vector3(-10, 0, 0);
    }
}
