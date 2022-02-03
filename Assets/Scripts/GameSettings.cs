using System;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    /// <summary>
    /// Количество строк игрового поля.
    /// </summary>
    [SerializeField] private int rowCount = 20;
    /// <summary>
    /// Количество столбцов игрового поля.
    /// </summary>
    [SerializeField] private int columnCount = 10;
    /// <summary>
    /// Увеличенное число столбцов поля.
    /// </summary>
    [SerializeField] private int extraColumnCount = 12;
    /// <summary>
    /// Допустимое время неподвижности фигуры в секундах.
    /// </summary>
    [SerializeField] private float dropTime = 0.7f;
    /// <summary>
    /// Время неподвижности фигуры для ускоренного перемещения вниз.
    /// </summary>
    [SerializeField] private float extraDropTime = 0.1f;
    /// <summary>
    /// Экземпляр класса грфического интерфейса.
    /// </summary>
    [SerializeField] private GUIController gui = null;

    /// <summary>
    /// Событие смены режима игры.
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
    } = Modes.FirstMode;

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
            if (Mode == Modes.SecondMode)
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
            if (Mode == Modes.SecondMode)
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
            if (Mode == Modes.SecondMode)
            {
                postition = new Vector3(columnCount / 2 + 1, rowCount, 0); ;
            }
            return postition;
        }
    }

    /// <summary>
    /// Время неподвижности фигуры перед очередным перемещением вниз.
    /// </summary>
    public float DropTime
    {
        get => dropTime;
    }

    /// <summary>
    /// Время неподвижности фигуры перед очередным перемещением вниз в режиме ускоренного перемещения.
    /// </summary>
    public float ExtraDropTime
    {
        get => extraDropTime;
    }

    /// <summary>
    /// Режимы игры.
    /// </summary>
    public enum Modes
    {
        /// <summary>
        /// Режим с семью фигурами.
        /// </summary>
        FirstMode = 7,
        /// <summary>
        /// Режим с десятью фигурами.
        /// </summary>
        SecondMode = 10,
    }

    private void Awake()
    {
        // Инициализировать свойство для доступа к объекту данного класса из других классов.
        Instance = this;

        // Если ранее производился выбор режима игры, то восстановить данное значение.
        Mode = (Modes)Enum.Parse(typeof(Modes), PlayerPrefs.GetString("mode", "FirstMode"));

        // Зарегистрировать обработчик события графического интерфейса, возникающего при выборе первого режима игры.
        gui.FirstModeChecked += () =>
        {
            // Сохранить выбранный режим игры. Значение будет доступно при перезапуске игры.
            PlayerPrefs.SetString("mode", "FirstMode");
            Mode = Modes.FirstMode;
            // Сгенерировать событие изменения режима игры.
            GameModeChanged();
        };
        // Обработчик события выбора второго режима игры.
        gui.SecondModeChecked += () =>
        {
            PlayerPrefs.SetString("mode", "SecondMode");
            Mode = Modes.SecondMode;
            GameModeChanged();
        };
    }
}
