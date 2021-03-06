using System;

namespace goshanoob.TETRIS
{
    /// <summary>
    /// Структура для работы со случайными величинами.
    /// </summary>
    public struct Randomizer
    {
        /// <summary>
        /// Массив вероятностей событий.
        /// </summary>
        private double[] chances;

        /// <summary>
        /// Создать экземпляр структуры, передав вероятности нескольких событий.
        /// </summary>
        /// <param name="probabilities">Вероятности событий</param>
        public Randomizer(double[] probabilities)
        {
            chances = new double[probabilities.Length];
            probabilities.CopyTo(chances, 0);
            CheckProbabilities();
        }

        /// <summary>
        /// Вернуть номер выпавшего события с учетом его вероятности.
        /// </summary>
        /// <returns>Номер случайного события</returns>
        public int GetNextNumber()
        {
            int result = 0;
            double randomValue = new Random().NextDouble();
            double sum = 0;

            // Найти отрезок, на который попала случайная величина.
            for (int i = 0, count = chances.Length; i < count; i++)
            {
                sum += chances[i];
                if (sum > randomValue)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Проверить, что сумма вероятностей равна 1.
        /// </summary>
        private void CheckProbabilities()
        {
            double sum = 0;
            foreach (double value in chances)
            {
                sum += value;
            }
            // Если сумма вероятностей отлична от 1, сгенерировать исключение.
            if (Math.Abs(sum - 1) > 1e-6)
            {
                throw new ArgumentException("The sum of the probabilities is not equal to one");
            }
        }

    }
}