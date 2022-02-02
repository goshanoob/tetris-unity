using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// ����� ��� ������ � ����������� �����������.
/// </summary>
internal class GUIController : MonoBehaviour
{
    [SerializeField] private Text scoreLabel = null; // ������� ��������� �����
    [SerializeField] private PlayerController player = null; // ������ ������

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
