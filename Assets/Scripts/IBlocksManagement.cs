namespace goshanoob.TETRIS
{
    /// <summary>
    /// Интерфейс для работы с блоками фигур.
    /// </summary>
    public interface IBlocksManagement
    {
        #region
        /// <summary>
        /// Заполнить блоки.
        /// </summary>
        void FillBlocks();

        /// <summary>
        /// Спрятать блоки.
        /// </summary>
        void HideBlocks();
        #endregion
    }
}
