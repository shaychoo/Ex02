using System.Collections.Generic;

namespace TicTacToeGameLogic
{
    internal class BoardGame
    {
        internal BoardGame(int i_Size)
        {
            BoradSize = i_Size;
            initializeBoardGameCells();
            FreeCellsList = new List<GameCell>(BoradSize * BoradSize);
        }

        internal List<GameCell> FreeCellsList { get; set; }
        internal GameCell[,] BoardGameCells { get; private set; }
        internal int BoradSize { get; private set; }
        internal GameCell LastSelectedCell { get; private set; }

        internal bool IsThereNoMoreFreeCells
        {
            get { return FreeCellsList.Count == 0; }
        }

        internal void ClearBorad()
        {
            LastSelectedCell = null;
            foreach (GameCell boardGameCell in BoardGameCells)
            {
                SetCellState(boardGameCell, Enums.eCellValue.Blank);
            }
        }

        internal void SetCellState(GameCell i_GameCell, Enums.eCellValue i_TheState)
        {
            i_GameCell.Value = i_TheState;
            if (i_TheState == Enums.eCellValue.Blank)
            {
                if (!FreeCellsList.Contains(i_GameCell))
                {
                    FreeCellsList.Add(i_GameCell);
                }
            }
            else
            {
                LastSelectedCell = i_GameCell;
                FreeCellsList.Remove(i_GameCell);
            }
        }

        private void initializeBoardGameCells()
        {
            BoardGameCells = new GameCell[BoradSize, BoradSize];

            for (int row = 0 ; row < BoradSize ; row++)
            {
                for (int column = 0 ; column < BoradSize ; column++)
                {
                    BoardGameCells[row, column] = new GameCell(row, column);
                }
            }
        }
    }
}