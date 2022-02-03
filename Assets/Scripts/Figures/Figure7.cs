namespace goshanoob.TETRIS
{
    /// <summary>
    /// Класс, описывающий T-образную фигуру.
    /// </summary>
    public class Figure7 : Figure
    {
        /// <summary>
        /// Альтернативная вероятность выпадения фигуры.
        /// </summary>
        public float SecondProbability
        {
            get => 0.05f;
        }
        public Figure7()
        {
            Probability = 0.2;
        }
    }
}
