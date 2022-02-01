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
    // Подключение экземпляра класса грфического интерфейса.
    [SerializeField] private GUIController gui = null;

    // Событие смены режима.
    public event Action GameModeChanged;

    /// <summary>
    /// Экземпляр игровых настроек.
    /// </summary>
    public static GameSettings Instance
    {
        get;
        private set;
    }

    /// <summary>
    /// Текущий режим игры.
    /// </summary>
    public Modes Mode
    {
        get;
        private set;
    } = Modes.firstMode;

    /// <summary>
    /// Число строк игрвого поля.
    /// </summary>
    public int RowCount
    {
        get => rowCount;
        set => rowCount = value;
    }

    /// <summary>
    /// Число солбцов игрового поля.
    /// </summary>
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

    /// <summary>
    /// Необходимое число подряд идущих собранных линий для их уничтожения. 
    /// </summary>
    public int LinesForDestroy
    {
        get
        {
            int count = 1;
            if (Mode == Modes.secondMode)
            {
                count = 2;
            }
            return count;
        }
    }

    /// <summary>
    /// Коориданты появления новых фигур.
    /// </summary>
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

    /// <summary>
    /// Перечисление режимов игры.
    /// </summary>
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
}
