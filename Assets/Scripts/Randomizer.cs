using System;

namespace goshanoob.Tetris
{
    /// <summary>
    /// Структура для работы со случайными величинами.
    /// </summary>
    public struct Randomizer
    {
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
        /// Метод возвращает номер выпавшего события с учетом его вероятности.
        /// </summary>
        /// <returns>Номер случайного события</returns>
        public int GetNextNumber()
        {
            int result = 0;
            double randomValue = new Random().NextDouble();
            double sum = 0;
            for(int i = 0, count = chances.Length; i < count; i++)
            {
                sum += chances[i];
                if(sum > randomValue)
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
            if (Math.Abs(sum - 1) > 1e-6)
            {
                throw new ArgumentException("Сумма вероятностей не равна 1");
            }
        }

    }
}