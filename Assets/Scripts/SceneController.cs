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
    // Список возможных в игре фигур.
    private List<GameObject> figures;

    [Header("Префабы фигур")]
    [SerializeField] private GameObject figure1;
    [SerializeField] private GameObject figure2;
    [SerializeField] private GameObject figure3;
    [SerializeField] private GameObject figure4;
    [SerializeField] private GameObject figure5;
    [SerializeField] private GameObject figure6;
    [SerializeField] private GameObject figure7;
    [SerializeField] private GameObject figure8;
    [SerializeField] private GameObject figure9;
    [SerializeField] private GameObject figure10;

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
        

        figures = new List<GameObject>();
        if(figure1 != null)
        {
            figures.Add(figure1);
        }
       
        figures.Add(figure2);
        figures.Add(figure3);

        GenerateFigure();

    }

    private void Figure_FigureDroped(object sender, EventArgs e)
    {
        GenerateFigure();
    }

    private void Update()
    {

    }

    private void GenerateFigure()
    {/*
        Randomizer random = new Randomizer();
        Debug.Log(random.GetNextNumber());

        Randomizer figureRandoms = new Randomizer(new double[] { 0.1, 0.15, 0.15, 0.15, 0.15, 0.1, 0.2 });
        int figureNumber = figureRandoms.GetNextNumber();*/
        GameObject newFigure = Instantiate(figures[0]);
        FigureController figureContoller = newFigure.GetComponent<FigureController>();
        figureContoller.sceneController = this;
        figureContoller.FigureDroped += Figure_FigureDroped;
    }
}
