namespace goshanoob.TETRIS
{
    /// <summary>
    /// Интерфейс для работы с линиями из блоков фигур.
    /// </summary>
    public interface ILinesManagement
    {
        #region
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
        #endregion
    }
}
