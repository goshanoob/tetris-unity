namespace goshanoob.TETRIS
{
    /// <summary>
    /// Диалоговое окно.
    /// </summary>
    public interface IDialogWindow
    {
        #region
        /// <summary>
        /// Открыть окно.
        /// </summary>
        /// <param name="message">Текстовое сообщение.</param>
        void Open(string message);

        /// <summary>
        /// Закрыть окно.
        /// </summary>
        void Close();
        #endregion
    }
}
