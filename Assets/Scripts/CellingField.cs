namespace goshanoob.Tetris
{
    /// <summary>
    ///  ласс, управл€ющий состо€нием €чеек игрового пол€.
    /// </summary>
    public class CellingField
    {
        // ѕоле дл€ хранени€ состо€ний €чеек игрового пол€.
        // true - €чейка заполнена, false - свободна.
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
        /// ћетод заполнени€ €чейки.
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <param name="columnNumber"></param>
        public void FillCell(int rowNumber, int columnNumber)
        {
            if (columnNumber >= columnCount || columnNumber < 0)
            {
                return;
            }
            cells[rowNumber, columnNumber] = true;
        }

        /// <summary>
        /// ћетод проверки €чейки на заполненность.
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        public bool CheckCell(int rowNumber, int columnNumber)
        {
            if (columnNumber >= columnCount || columnNumber < 0)
            {
                return false;
            }

            if (cells[rowNumber, columnNumber])
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// ћетод проверки заполненности линии игрового пол€.
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        public bool CheckLine(int lineNumber)
        {
            bool isFill = true;
            for (int i = 0; i < columnCount; i++)
            {
                if (!cells[lineNumber, i])
                {
                    isFill = false;
                }
            }
            return isFill;
        }

        /// <summary>
        /// ћетод сдвига верхних линий на место освобожденной линии.
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
