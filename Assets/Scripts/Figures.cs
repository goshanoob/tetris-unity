using System;
using UnityEngine;

namespace goshanoob.Tetris
{
    /// <summary>
    /// Абстарктный класс для определения фигур игры.
    /// </summary>
    public abstract class Figure : MonoBehaviour
    {
        [SerializeField] private double probability = 0; // вероятность выпадения фигуры

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
