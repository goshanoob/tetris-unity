using goshanoob.TETRIS;
using UnityEngine;

/// <summary>
/// Менеджер блоков фигуры.
/// </summary>
public class BlockManager : MonoBehaviour, IBlocksManagement, ILinesManagement
{
    /// <summary>
    /// Настройки игры.
    /// </summary>
    private GameSettings settings = null;
    /// <summary>
    /// Контроллер сцены.
    /// </summary>
    private SceneController sceneController = null;
    /// <summary>
    /// Родительская система координат блоков фигуры.
    /// </summary>
    [SerializeField] private GameObject parent = null;

    private void Awake()
    {
        // Установить ссылки на необходимые экземпляры классов для взаимодействия с ними.

        sceneController = SceneController.Instance;
        settings = GameSettings.Instance;
    }

    /// <summary>
    /// Заполнить ячейки игрвого поля значениями true в местах блоков упавшей фигуры.
    /// </summary>
    public void FillBlocks()
    {
        foreach (Transform block in parent.transform)
        {
            int rowNumber = Mathf.FloorToInt(block.position.y);
            int columnNumber = Mathf.FloorToInt(block.position.x);
            sceneController.Cells.SetCell(rowNumber, columnNumber);
        }
    }

    /// <summary>
    /// Удалить линию с определенным номером.
    /// </summary>
    /// <param name="lineNumber">Линия для удаления.</param>
    public void OnLineDestroy(int lineNumber)
    {
        // Перебрать все блоки фигуры.
        foreach (Transform block in parent.transform)
        {
            // Если блок расположен в линии для удаления, уничтожить его.
            if (Mathf.FloorToInt(block.position.y) == lineNumber)
            {
                Destroy(block.gameObject);
            }
        }
    }

    /// <summary>
    /// Передвинуть все блоки фигуры, расположенные выше определенной линии
    /// </summary>
    /// <param name="lineNumber">Номер линии, ближе к которой сместятся блоки выше нее.</param>
    public void OnLinesShift(int lineNumber)
    {
        foreach (Transform block in parent.transform)
        {
            // Если блок расположен выше линии, то сдвинуть его на единицу вниз.
            if (Mathf.FloorToInt(block.position.y) > lineNumber)
            {
                block.transform.position += Vector3.down;
            }
        }
    }

    /// <summary>
    /// Скрыть блоки вне игрового поля.
    /// </summary>
    public void HideBlocks()
    {
        foreach (Transform block in parent.transform)
        {
            float positionX = block.position.x;
            // Если позиция блока фигуры выходит за границы игрового поля по горизонтали, сделать его невидимым, иначе - сделать видимым.
            if (positionX >= settings.ColumnCount ||
                positionX < 0)
            {
                block.gameObject.SetActive(false);
            }
            else
            {
                block.gameObject.SetActive(true);
            }
        }
    }
}
