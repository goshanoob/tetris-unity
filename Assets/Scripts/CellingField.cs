namespace goshanoob.Tetris
{
    /// <summary>
    ///  Класс, управляющий состоянием ячеек игрового поля.
    /// </summary>
    public class CellingField
    {
        // Поле для хранения состояний ячеек игрового поля.
        // true - ячейка заполнена, false - свободна.
        private bool[,] cells;
        private int rowCount = 0;
        private int columnCount = 0;

        public CellingField(int rowCount = 20, int columnCount = 10)
        {
            this.rowCount = rowCount;
            this.columnCount = columnCount;
            cells = new bool[rowCount, columnCount];
        }

        /// <summary>
        /// Индексатор для чтения и записи состояния ячейки.
        /// </summary>
        /// <param name="i">Строка</param>
        /// <param name="j">Столбец</param>
        /// <returns></returns>
        public bool this[int i, int j]
        {
            get
            {
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

        public void SetCell(int i, int j)
        {
            if (j < columnCount && j >= 0)
            {
                cells[i, j] = true;
            }
        }

        /// <summary>
        /// Метод проверки заполненности линии игрового поля.
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        public bool CheckLine(int lineNumber)
        {
            if(lineNumber >= rowCount || lineNumber < 0)
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
        /// Метод сдвига верхних линий на место освобожденной линии.
        /// </summary>
        /// <param name="lineNumber"></param>
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
