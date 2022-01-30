using System;
using UnityEngine;

internal class GameSettings : MonoBehaviour
{
    // ������� ����� ����.
    private Modes currentMode = Modes.firstMode;
    // ������� ����� ������.
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
            // ���������� ����� � ���������� ����������.
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
        /// ����� � ����� ��������.
        /// </summary>
        firstMode = 7,
        /// <summary>
        /// ����� � ������� ��������.
        /// </summary>
        secondMode = 10,
    }
}
