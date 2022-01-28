using goshanoob.Tetris;
using System;
using UnityEngine;

/// <summary>
///  Класс, управляющий игровым полем.
/// </summary>
internal class SceneController : MonoBehaviour
{
    // Количество сторок игрового поля.
    private int rowCount = 20;
    // Количество столбцов игрового поля.
    private int columnCount = 10;
    // Режимы игры.
    private enum Modes
    {
        firstMode = 7,
        secondMode = 10,
    }
    private Modes currentMode = Modes.secondMode;

    [SerializeField] private GameObject[] figures;
    private Randomizer figureRandoms;

    public static Vector3 spawnPosition ;
    // Событие, уничтожающее линию.
    public event Action<int> LineDestroy;
    public event Action<int> LinesShift;
    public CellingField Ground
    {
        get;
        set;
    }

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

    private void Awake()
    {
        Ground = new CellingField(RowCount + 2, ColumnCount);
    }

    private void Start()
    {
        spawnPosition = new Vector3(columnCount / 2, rowCount + 1f, 0);
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

    private void OnFigureDroped(object sender, EventArgs e)
    {
        // Проверить, не появились ли заполненные линии.
        CheckLines();
        // Сгенерировать новую фигуру.
        SpawnNewFigure();
    }

    /// <summary>
    /// Метод создания случайной фигуры с необходимыми компонентами и обработчиками.
    /// </summary>
    private void SpawnNewFigure()
    {
        // Получить случайное значение.
        int figureNumber = figureRandoms.GetNextNumber();
        // Создать случайную фигуру.
        GameObject newFigure = Instantiate(figures[figureNumber]);
        // Сообщить экземпляру фигуры о текущем контроллере.
        FigureController figureContoller = newFigure.GetComponent<FigureController>();
        figureContoller.SceneController = this;
        // Зарегистрировать обработчкик события падения фигуры на дно игрового поля.
        figureContoller.FigureDroped += OnFigureDroped;
    }

    private void CheckLines()
    {
        for (int i = 0; i < RowCount; i++)
        {
            Debug.Log("Ground.CheckLine(i) = " + Ground.CheckLine(i) + ";  i = " + i);
            // Если линия полность заполнена, выполнить действия.
            if (Ground.CheckLine(i))
            {
                // Вызвать событие для удаления строки.
                LineDestroy?.Invoke(i);
                // Сдвинуть верхние строки на место удаленной.
                Ground.ShiftLines(i);
                // Вызвать событие сдига блоков фигур.
                LinesShift?.Invoke(i);
            }

        }
    }



    public void SpawnNewFigure(GameObject originFigure)
    {
        GameObject newFigure = Instantiate(originFigure, originFigure.transform.position - new Vector3(10, 0, 0), new Quaternion());
        // Сообщить экземпляру фигуры о текущем контроллере.
        FigureController figureContoller = newFigure.GetComponent<FigureController>();
        figureContoller.SceneController = this;
        //newFigure.transform.position += new Vector3(-10, 0, 0);
    }
}
