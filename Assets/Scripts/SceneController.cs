using UnityEngine;

/// <summary>
///  Класс, управляющий игровым полем.
/// </summary>
public class SceneController : MonoBehaviour
{
    // Количество сторок игрового поля.
    private static int rowCount = 20;
    // Количество столбцов игрового поля.
    private static int columnCount = 10;

    [Header("Префабы фигур")]
    [SerializeField] private GameObject figure1;
    [SerializeField] private GameObject figure2;
    [SerializeField] private GameObject figure3;
    [SerializeField] private GameObject figure4;
    [SerializeField] private GameObject figure5;
    [SerializeField] private GameObject figure6;
    [SerializeField] private GameObject figure7;
    [SerializeField] private GameObject figure8;
    [SerializeField] private GameObject figure9;
    [SerializeField] private GameObject figure10;

    [SerializeField] public static Vector3 spawnPosition = new Vector3(0, rowCount / 2, 0);
    public int RowCount
    {
        get => rowCount;
        set => rowCount = value;
    }
    public int ColumnCount
    {
        get => columnCount;
        set => columnCount = value;
    }

    private void Start()
    {
    }

    private void Update()
    {
        
    }



}
