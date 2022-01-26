using System;
using UnityEngine;
using XD.Additions;
using XD.Tetris;

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
        firstMode = 7,
        secondMode = 10,
    }
    private Modes currentMode = Modes.firstMode;

    private GameObject[] figures;
    private Randomizer figureRandoms;

    public static Vector3 spawnPosition = new Vector3(0, rowCount / 2, 0);
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
        CreateFigures();

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
}
