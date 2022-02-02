using UnityEngine;
using UnityEngine.UI;

public class ResultDialog : MonoBehaviour
{
    [SerializeField] private Text resultLabel = null; // ������� � ����������� ����

    private void Start()
    {
        // ������� ���� ��� ������.
        Close();
    }

    /// <summary>
    /// ������� ���������� ����  � ������������ ����.
    /// </summary>
    public void Open(int score)
    {
        resultLabel.text = $"��� ���������: {score} �����(-��)";
        gameObject.SetActive(true);
    }

    /// <summary>
    /// ������� ���������� ����.
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
