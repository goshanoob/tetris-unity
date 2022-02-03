using System;

namespace goshanoob.TETRIS
{
    /// <summary>
    /// Графический интерфейс.
    /// </summary>
    public interface IUIManager
    {
        #region
        /// <summary>
        /// Событие перезапуска игры.
        /// </summary>
        event Action RestartClicked;

        /// <summary>
        /// Обработать изменение счета игры.
        /// </summary>
        /// <param name="score"></param>
        void OnScoreChanged(int score);

        /// <summary>
        /// Обработать событие перезапуска игры.
        /// </summary>
        void OnRestart();

        /// <summary>
        /// Обработать событие завершения игры.
        /// </summary>
        void OnGameOver();
        #endregion
    }
}
