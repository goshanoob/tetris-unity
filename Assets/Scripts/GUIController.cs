using UnityEngine;
using UnityEngine.UI;
using System;

internal class GUIController : MonoBehaviour
{
    [SerializeField] private Text scoreLabel = null;
    [SerializeField] private PlayerController player = null;

    public event Action RestartClicked;
    public event Action FirstModeChecked;
    public event Action SecondModeChecked;

    private void Awake()
    {
        player.ScoreChanged += OnScoreChanged;
    }

    private void OnScoreChanged(int score)
    {
        scoreLabel.text = score.ToString();
    }

    public void OnRestart()
    {
        RestartClicked?.Invoke();
    }

    public void OnFirstModeChanged()
    {
        FirstModeChecked?.Invoke();
    }

    public void OnSecondModeChanged()
    {
        SecondModeChecked?.Invoke();
    }
}
