namespace TicTacToeGameLogic
{
    public class GameCell
    {
        internal GameCell(int i_RowIndex, int i_ColumnIndex)
        {
            RowIndex = i_RowIndex;
            ColumnIndex = i_ColumnIndex;
        }

        public int RowIndex { get; private set; }
        public int ColumnIndex { get; private set; }

        internal bool IsFree
        {
            get { return Value == Enums.eCellValue.Blank; }
        }

        public Enums.eCellValue Value { get; internal set; }
    }
}