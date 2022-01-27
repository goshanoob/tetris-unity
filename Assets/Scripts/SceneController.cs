using System;
using UnityEngine;
using goshanoob.Tetris;

/// <summary>
///  Класс, управляющий игровым полем.
/// </summary>
public class SceneController : MonoBehaviour
{
    // Количество сторок игрового поля.
    private static int rowCount = 20;
    // Количество столбцов игрового поля.
    private static int columnCount = 10;
    // Режимы игры.
    private enum Modes
    {
        firstMode = 3,
        secondMode = 10,
    }
    private Modes currentMode = Modes.firstMode;

    [SerializeField] private GameObject[] figures;
    private Randomizer figureRandoms;

    public static Vector3 spawnPosition = new Vector3(columnCount / 2, rowCount + 1f, 0);

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

    private void Start()
    {
        Ground = new CellingField(RowCount + 2, ColumnCount);

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

        // Получить экземпляр структуры для генерации случайных значений с учетом их вероятности.
        figureRandoms = new Randomizer(probabilities);
    }

    private void Figure_FigureDroped(object sender, EventArgs e)
    {
        SpawnNewFigure();
    }

    private void Update()
    {

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
        figureContoller.FigureDroped += Figure_FigureDroped;
    }

    public void SpawnNewFigure(GameObject originFigure)
    {

        GameObject newFigure = Instantiate(originFigure, originFigure.transform.position - new Vector3(10, 0, 0), new Quaternion());
        // Сообщить экземпляру фигуры о текущем контроллере.
        FigureController figureContoller = newFigure.GetComponent<FigureController>();
        figureContoller.SceneController = this;
        //newFigure.transform.position += new Vector3(-10, 0, 0);
        int a = 5;

    }
}
