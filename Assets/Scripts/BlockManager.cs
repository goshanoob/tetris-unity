using goshanoob.Tetris;
using UnityEngine;

/// <summary>
/// Менеджер блоков фигур.
/// </summary>
public class BlockManager : MonoBehaviour, IBlocksManagement
{
    [SerializeField] private GameSettings settings = null;
    [SerializeField] private SceneController sceneController = null;
    [SerializeField] private GameObject rotator = null;

    public void FillBlocks()
    {
        foreach (Transform block in rotator.transform)
        {
            int rowNumber = Mathf.FloorToInt(block.position.y);
            int columnNumber = Mathf.FloorToInt(block.position.x);
            sceneController.Cells.SetCell(rowNumber, columnNumber);
        }
    }

    public void OnLineDestroy(int lineNumber)
    {
        foreach (Transform block in rotator.transform)
        {
            if ((int)block.position.y == lineNumber)
            {
                Destroy(block.gameObject);
            }
        }
    }

    public void OnLinesShift(int lineNumber)
    {
        foreach (Transform block in rotator.transform)
        {
            if ((int)block.position.y > lineNumber)
            {
                // »заменить абсолютные координаты фигуры.
                block.transform.position += Vector3.down;
            }
        }
    }

    public void HideBlocks()
    {
        foreach (Transform block in rotator.transform)
        {
            float positionX = block.position.x;
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
