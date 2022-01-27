using System;

namespace goshanoob.Tetris
{
    /// <summary>
    ///  ласс, описывающий €чейки игрового пол€.
    /// </summary>
    public class CellingField
    {
        private bool[,] fillingCells;

        public int RowCount
        {
            get;
            set;
        }

        public int ColumnCount
        {
            get;
            set;
        }

        public CellingField(int rowCount = 20, int columnCount = 10)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
            fillingCells = new bool[rowCount, columnCount];
        }

        public void FillCell(int rowNumber, int columnNumber)
        {
            if (columnNumber >= ColumnCount || columnNumber < 0)
            {
                return;
            }
            fillingCells[rowNumber, columnNumber] = true;
        }

        public bool CheckCell(int rowNumber, int columnNumber)
        {
            if (columnNumber >= ColumnCount || columnNumber < 0)
            {
                return false;
            }

            if (fillingCells[rowNumber, columnNumber])
            {
                return true;
            }
            return false;
        }


        public void FillCells()
        {

        }

        


        public void ClearCell(int rowNumber, int columnNumber)
        {

        }

        public int CheckLine()
        {
            throw new NotImplementedException();
        }
    }
}
