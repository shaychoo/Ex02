using System;
using System.Collections.Generic;

namespace TicTacToeGameLogic
{
    public class Board
    {
        private readonly Random r_Random = new Random();
        private List<BoardGameCell> m_FreeCellsList;

        internal Board(int i_Size)
        {
            BoradSize = i_Size;
        }

        internal BoardGameCell[,] BoardGameCells { get; private set; }
        internal int BoradSize { get; private set; }

        internal bool IsBoardFull
        {
            get { return m_FreeCellsList.Count == 0; }
        }

        internal void ClearBorad()
        {
            BoardGameCells = new BoardGameCell[BoradSize, BoradSize];
            m_FreeCellsList = new List<BoardGameCell>(BoradSize * BoradSize);

            for (int row = 0 ; row < BoradSize ; row++)
            {
                for (int column = 0 ; column < BoradSize ; column++)
                {
                    BoardGameCells[row, column] = new BoardGameCell(row, column);
                    m_FreeCellsList.Add(BoardGameCells[row, column]);
                }
            }
        }

        internal void SetCellState(BoardGameCell i_BoardGameCell, Enums.eBoardCellStateValue i_TheState)
        {
            i_BoardGameCell.Value = i_TheState;
            if (i_TheState == Enums.eBoardCellStateValue.Blank)
            {
                if (!m_FreeCellsList.Contains(i_BoardGameCell))
                {
                    m_FreeCellsList.Add(i_BoardGameCell);
                }
            }
            else
            {
                m_FreeCellsList.Remove(i_BoardGameCell);
            }
        }

        internal BoardGameCell GetRandomFreeCell()
        {
            int randomValue = r_Random.Next(0, m_FreeCellsList.Count - 1);
            BoardGameCell randomBoardGameCell = m_FreeCellsList[randomValue];
            return randomBoardGameCell;
        }

        public class BoardGameCell
        {
            internal BoardGameCell(int i_RowIndex, int i_ColumnIndex)
            {
                RowIndex = i_RowIndex;
                ColumnIndex = i_ColumnIndex;
            }

            internal int RowIndex { get; private set; }
            internal int ColumnIndex { get; private set; }

            public bool IsFree
            {
                get { return Value == Enums.eBoardCellStateValue.Blank; }
            }

            public Enums.eBoardCellStateValue Value { get; internal set; }
        }
    }
}