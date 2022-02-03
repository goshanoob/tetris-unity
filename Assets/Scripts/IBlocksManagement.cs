namespace goshanoob.Tetris
{
    /// <summary>
    /// Интерфейс для работы с блоками фигур.
    /// </summary>
    public interface IBlocksManagement
    {
        /// <summary>
        /// Пометить блоки заполненными.
        /// </summary>
        void FillBlocks();

        /// <summary>
        /// Спрятать блоки.
        /// </summary>
        void HideBlocks();

        /// <summary>
        /// Убрать блоки линии.
        /// </summary>
        /// <param name="lineNumber">Номер убираемой линии.</param>
        void OnLineDestroy(int lineNumber);

        /// <summary>
        /// Сдвинуть блоки линий.
        /// </summary>
        /// <param name="lineNumber">Номер линии, к которой произвести сдвиг верхних линий.</param>
        void OnLinesShift(int lineNumber);
    }
}
