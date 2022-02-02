using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����� ��� ������ � ����������� �����������.
/// </summary>
public class GUIController : MonoBehaviour
{
    [SerializeField] private Text scoreLabel = null; // ������� ��������� �����
    [SerializeField] private PlayerController player = null; // ������ ������
    [SerializeField] private SceneController sceneController = null; // ������ ����������� �����
    [SerializeField] private ResultDialog resultDialog = null; // ������ ���� � ����������� ����

    /// <summary>
    /// ������� ������� ������ ������������ ����.
    /// </summary>
    public event Action RestartClicked;

    /// <summary>
    /// ������� ������ ������� ������ ����.
    /// </summary>
    public event Action FirstModeChecked;

    /// <summary>
    /// ������� ������ ������� ������ ����.
    /// </summary>
    public event Action SecondModeChecked;

    private void Awake()
    {
        // �������� ���������� � ������ ������� ������, ����������� ��� ��������� �����.
        player.ScoreChanged += OnScoreChanged;
        // �������� ���������� � �������, ����������� � ����������� ����� ��� ��������� ����.
        sceneController.GameOver += OnGameOver;
    }

    /// <summary>
    /// ���������� ��������� ����� ����.
    /// </summary>
    /// <param name="score">���������� �����.</param>
    private void OnScoreChanged(int score)
    {
        scoreLabel.text = score.ToString();
    }

    /// <summary>
    /// ���������� ������� ��������� ����.
    /// </summary>
    private void OnGameOver()
    {
        resultDialog.Open(player.Score);
    }

    /// <summary>
    /// ���������� ������� ������� ������ �������� ����.
    /// </summary>
    public void OnRestart()
    {
        // ������� ����������� ������� ������� ������ ��� ������� ������ �������� ��� ���������� ��������-�����������.
        RestartClicked?.Invoke();
    }

    /// <summary>
    /// ���������� ������� ������� ������ ������ ������� ������ ����.
    /// </summary>
    public void OnFirstModeChanged()
    {
        FirstModeChecked?.Invoke();
    }

    /// <summary>
    /// ���������� ������� ������� ������ ������ ������� ������ ����.
    /// </summary>
    public void OnSecondModeChanged()
    {
        SecondModeChecked?.Invoke();
    }
}
