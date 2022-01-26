using System;
using System.Collections.Generic;
using UnityEngine;
using XD.Additions;

/// <summary>
///  Класс, управляющий игровым полем.
/// </summary>
public class SceneController : MonoBehaviour
{
    // Количество сторок игрового поля.
    private static int rowCount = 20;
    // Количество столбцов игрового поля.
    private static int columnCount = 10;
    // Вероятности выпадения фигур.
    private static double[] probabilities = new double[] { 0.1, 0.15, 0.15, 0.15, 0.15, 0.1, 0.2, 0.05 , 0.05 , 0.05 };
    // Режимы игры.
    private enum Modes
    {
        firstMode = 7,
        secondMode = 10,
    }
    private Modes currentMode = Modes.firstMode;
    private Randomizer figureRandoms;

    [Header("Префабы фигур")]
    [SerializeField] private GameObject[] figures;

    [SerializeField] public static Vector3 spawnPosition = new Vector3(0, rowCount / 2, 0);
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
        GenerateFigure();
    }

    private void CreateFigures()
    {
        // Количество фигур согласно режиму игры.
        double[] modeProbabilities = new double[(int)Modes.firstMode];
        switch (currentMode)
        {
            case Modes.firstMode:
                Array.Copy(probabilities, modeProbabilities, (int)Modes.firstMode);
                break;

            case Modes.secondMode:
                Array.Copy(probabilities, modeProbabilities, (int)Modes.secondMode);
                break;
        }
        // Получить экземпляр структуры для генерации случайных значений с учетом их вероятности.
        figureRandoms = new Randomizer(modeProbabilities);
    }
    private void Figure_FigureDroped(object sender, EventArgs e)
    {
        GenerateFigure();
    }

    private void Update()
    {

    }

    /// <summary>
    /// Метод создания случайной фигуры с необходимыми компонентами и обработчиками.
    /// </summary>
    private void GenerateFigure()
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
