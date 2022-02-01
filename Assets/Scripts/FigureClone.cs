using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureClone : MonoBehaviour
{
    private bool isMovableClone(Vector3 movement)
    {
        bool result = true;
        if (Clone != null)
        {
            result = Clone.isCorrectMove(movement);
            if (!result)
            {
                isDroped = true;
                FillBlocks();
                FigureDroped?.Invoke();
            }
        }
        return result;
    }

    private void MoveToSide(float xPosition)
    {
        if (xPosition >= 2 * settings.ColumnCount)
        {
            transform.Translate(2 * settings.ColumnCount * Vector3.left);
        }
        else
        {
            transform.Translate(2 * settings.ColumnCount * Vector3.right);
        }
    }
}
