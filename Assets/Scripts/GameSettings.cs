using System;
using UnityEngine;

internal class GameSettings : MonoBehaviour
{
   
    // Количество строк игрового поля.
    private int rowCount = 20;
    // Количество столбцов игрового поля.
    private int columnCount = 10;
    // Увеличенное число столбцов поля.
    private int extraColumnCount = 12;
    // Текущий режим игры.
    private Modes currnetMode = Modes.firstMode;
    // Подключение экземпляра класса грфического интерфейса.
    [SerializeField] private GUIController gui = null;

    // Событие смены режима.
    public event Action GameModeChanged;

    public static GameSettings Instance
    {
        get;
        private set;
    }

    public Modes Mode
    {
        get;
        private set;
    } = Modes.firstMode;

    public int RowCount
    {
        get => rowCount;
        set => rowCount = value;
    }
    
    public int ColumnCount
    {
        get
        {
            if(Mode == Modes.secondMode)
            {
                return extraColumnCount;
            }
            return columnCount;
        }
    }

    // Место появления новых фигур.
    public Vector3 SpawnPosition
    {
        get
        {
            Vector3 postition = new Vector3(columnCount / 2, rowCount, 0);
            if (Mode == Modes.secondMode)
            {
                postition = new Vector3(columnCount / 2 + 1, rowCount, 0); ;
            }
            return postition;
        }
    }

    private void Awake()
    {
        Instance = this;

        Mode = (Modes)Enum.Parse(typeof(Modes), PlayerPrefs.GetString("mode", "firstMode"));
        gui.FirstModeChecked += () =>
        {
            PlayerPrefs.SetString("mode", "firstMode");
            Mode = Modes.firstMode;
            GameModeChanged();
        };
        gui.SecondModeChecked += () =>
        {
            PlayerPrefs.SetString("mode", "secondMode");
            Mode = Modes.secondMode;
            GameModeChanged();
        };
    }


    public enum Modes
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
}
