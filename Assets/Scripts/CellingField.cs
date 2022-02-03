namespace goshanoob.TETRIS
{
    /// <summary>
    /// Ячейки игрового поля.
    /// </summary>
    public class CellingField
    {
        /// <summary>
        /// Заполненность ячеек игрового поля. true - ячейка заполнена, false - свободна.
        /// </summary>
        private bool[,] cells;
        /// <summary>
        /// Число строк игрового поля.
        /// </summary>
        private int rowCount = 0;
        /// <summary>
        /// Число столбцов игрового поля.
        /// </summary>
        private int columnCount = 0;

        /// <summary>
        /// Конструктор класса игрового поля.
        /// </summary>
        /// <param name="rowCount">Число строк игрового поля.</param>
        /// <param name="columnCount">Количество столбцов игрового поля.</param>
        public CellingField(int rowCount = 20, int columnCount = 10)
        {
            this.rowCount = rowCount;
            this.columnCount = columnCount;
            cells = new bool[rowCount, columnCount];
        }

        /// <summary>
        /// Доступ к значениям ячеек игрового поля.
        /// </summary>
        /// <param name="i">Строка</param>
        /// <param name="j">Столбец</param>
        /// <returns>Возвращает значение ячейки игрового поля.</returns>
        public bool this[int i, int j]
        {
            get
            {
                // Если запрашиваемый индекс ячейки не выходит за границы массива, вернуть ее значение, иначе - ячейку считать свободной.
                if (j >= columnCount || j < 0)
                {
                    return false;
                }
                return cells[i, j];
            }
            set
            {
                SetCell(i, j);
            }
        }

        /// <summary>
        /// Установить значение ячейки игрового поля.
        /// </summary>
        /// <param name="i">Номер строки.</param>
        /// <param name="j">Номер столбца.</param>
        public void SetCell(int i, int j)
        {
            // Если номер столбца не выходит за границы массива, отметить ячейку заполненной.
            if (j < columnCount && j >= 0)
            {
                cells[i, j] = true;
            }
        }

        /// <summary>
        /// Проверить заполненность линии игрового поля.
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <returns>Вернет истину, если линия полностью заполнена.</returns>
        public bool CheckLine(int lineNumber)
        {
            // Если номер линии выходит за границы массива, то линия игнорируется.
            if (lineNumber >= rowCount || lineNumber < 0)
            {
                return false;
            }

            bool isFill = true;
            for (int i = 0; i < columnCount; i++)
            {
                if (!cells[lineNumber, i])
                {
                    isFill = false;
                    break;
                }
            }
            return isFill;
        }

        /// <summary>
        /// Сдвинуть верхние линии на место освобожденной.
        /// </summary>
        /// <param name="lineNumber">Номер линии</param>
        public void ShiftLines(int lineNumber)
        {
            for (int i = lineNumber; i < rowCount - 1; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {

                    cells[i, j] = cells[i + 1, j];
                }
            }
        }
    }
}