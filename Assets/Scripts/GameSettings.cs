using System;
using UnityEngine;

internal class GameSettings : MonoBehaviour
{
    // Текущий режим игры.
    private Modes currentMode = Modes.firstMode;
    // Событие смены режима.
    public event Action<Modes> GameModeChanged;

    [SerializeField] private GUIController gui = null;

    public static GameSettings Instance
    {
        get;
        private set;
    }

    public Modes Mode
    {
        get => currentMode;
        private set
        {
            // Установить режим и оповестить слушателей.
            currentMode = value;
            GameModeChanged?.Invoke(currentMode);
        }
    }

    private void Awake()
    {
        Instance = this;

        currentMode = (Modes)Enum.Parse(typeof(Modes), PlayerPrefs.GetString("mode", "firstMode"));
        gui.FirstModeChecked += () =>
        {
            PlayerPrefs.SetString("mode", "firstMode");
            Mode = Modes.firstMode;
        };
        gui.SecondModeChecked += () =>
        {
            PlayerPrefs.SetString("mode", "secondMode");
            Mode = Modes.secondMode;
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
