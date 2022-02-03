using System;
using UnityEngine;

namespace goshanoob.TETRIS
{
    /// <summary>
    /// Абстарктный класс для определения фигур игры.
    /// </summary>
    public abstract class Figure : MonoBehaviour
    {
        /// <summary>
        /// Вероятность выпадения фигуры.
        /// </summary>
        [SerializeField] private double probability = 0;

        /// <summary>
        /// Вероятность выпадения фигуры.
        /// </summary>
        public double Probability
        {
            get
            {
                return probability;
            }
            set
            {
                if (value >= 0)
                {
                    probability = value;
                }
                else
                {
                    throw new ArgumentException("A probability can't be negative");
                }
            }
        }
    }


}
