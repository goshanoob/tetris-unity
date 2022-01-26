using System;
using UnityEngine;

namespace XD.Tetris
{
    internal abstract class Figure: MonoBehaviour
    {
        private double probability = 0;
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
                    throw new ArgumentException("Probability can't be negative");
                }
            }
        }
    }
        

}
