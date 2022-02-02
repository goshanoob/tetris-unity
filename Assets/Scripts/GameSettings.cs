using System;
using UnityEngine;

internal class GameSettings : MonoBehaviour
{
    [SerializeField] private int rowCount = 20; // количество строк игрового поля
    [SerializeField] private int columnCount = 10; // количество столбцов игрового поля
    [SerializeField] private int extraColumnCount = 12; // увеличенное число столбцов поля
    [SerializeField] private float dropTime = 0.7f; // допустимое время неподвижности фигуры в секундах
    [SerializeField] private float extraDropTime = 0.1f; // время неподвижности фигуры для ускоренного перемещения вниз
    [SerializeField] private GUIController gui = null; // экземпляр класса грфического интерфейса

    /// <summary>
    /// Событие смены режима.
    /// </summary>
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
    /// Число столбцов игрового поля.
    /// </summary>
    /// <remarks>
    /// Возвращается количество столбцов в зависимости от выбранного режима.
    /// </remarks>
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
    /// <remarks>
    /// Возвращается число строк минимально достаточное для их удаления в зависимости от выбранного режима.
    /// </remarks>
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
    /// <remarks>
    /// Возвращается вектор координат точки возникновения фигуры в зависимости от выбранного режима в соответствии с шириной игрового поля.
    /// </remarks>
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
        // Инициализировать свойство для доступа к объекту данного класса из других классов.
        Instance = this;

        // Если ранее производился выбор режима игры, то восстановить данное значение.
        Mode = (Modes)Enum.Parse(typeof(Modes), PlayerPrefs.GetString("mode", "firstMode"));

        // Зарегистрировать обработчик события графического интерфейса, возникающего при выборе первого режима игры.
        gui.FirstModeChecked += () =>
        {
            // Сохранить выбранный режим игры. Значение будет доступно при перезапуске игры.
            PlayerPrefs.SetString("mode", "firstMode");
            Mode = Modes.firstMode;
            // Сгенерировать событие изменения режима игры.
            GameModeChanged();
        };
        // Обработчик события выбора второго режима игры.
        gui.SecondModeChecked += () =>
        {
            PlayerPrefs.SetString("mode", "secondMode");
            Mode = Modes.secondMode;
            GameModeChanged();
        };
    }
}
