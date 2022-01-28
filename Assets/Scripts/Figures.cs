using System;
using UnityEngine;

namespace goshanoob.Tetris
{
    internal abstract class Figure: MonoBehaviour
    {
        [SerializeField] private double probability = 0;
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
