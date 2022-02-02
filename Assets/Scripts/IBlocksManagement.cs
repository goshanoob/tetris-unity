namespace goshanoob.Tetris
{
    public interface IBlocksManagement
    {
        void FillBlocks();
        void OnLineDestroy(int lineNumber);
        void OnLinesShift(int lineNumber);
        void HideBlocks();
    }
}
