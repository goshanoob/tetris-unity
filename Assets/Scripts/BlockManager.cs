using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class BlockManager : FigureController
{
    [SerializeField] private FigureController figure = null;

    private void FillBlocks()
    {
        foreach (Transform block in figure.rotator.transform)
        {
            int rowNumber = Mathf.FloorToInt(block.position.y);
            int columnNumber = Mathf.FloorToInt(block.position.x);
            sceneController.Cells.SetCell(rowNumber, columnNumber);
        }
    }

    private void OnLineDestroy(int lineNumber)
    {
        foreach (Transform block in rotator.transform)
        {
            if ((int)block.position.y == lineNumber)
            {
                Destroy(block.gameObject);
            }
        }
    }

    private void OnLinesShift(int lineNumber)
    {
        foreach (Transform block in rotator.transform)
        {
            if ((int)block.position.y > lineNumber)
            {
                // Изаменить абсолютные координаты фигуры.
                block.transform.position += Vector3.down;
            }
        }
    }

    private void HideBlocks()
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
