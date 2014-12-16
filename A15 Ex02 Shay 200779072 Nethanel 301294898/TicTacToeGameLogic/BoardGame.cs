using System;
using System.Collections.Generic;

namespace TicTacToeGameLogic
{
    internal class BoardGame
    {
        private readonly Random r_Random = new Random();
        private List<GameCell> m_FreeCellsList;

        internal BoardGame(int i_Size)
        {
            BoradSize = i_Size;
            initializeBoardGameCells();
            m_FreeCellsList = new List<GameCell>(BoradSize * BoradSize);
        }

        internal GameCell[,] BoardGameCells { get; private set; }
        internal int BoradSize { get; private set; }

        internal bool IsBoardFull
        {
            get { return m_FreeCellsList.Count == 0; }
        }

        internal void ClearBorad()
        {
            foreach (GameCell boardGameCell in BoardGameCells)
            {
                SetCellState(boardGameCell,Enums.eCellValue.Blank);
            }
        }

        internal void SetCellState(GameCell i_GameCell, Enums.eCellValue i_TheState)
        {
            i_GameCell.Value = i_TheState;
            if (i_TheState == Enums.eCellValue.Blank)
            {
                if (!m_FreeCellsList.Contains(i_GameCell))
                {
                    m_FreeCellsList.Add(i_GameCell);
                }
            }
            else
            {
                m_FreeCellsList.Remove(i_GameCell);
            }
        }

        internal GameCell GetRandomFreeCell()
        {
            int randomValue = r_Random.Next(0, m_FreeCellsList.Count - 1);
            GameCell randomGameCell = m_FreeCellsList[randomValue];
            return randomGameCell;
        }

        private void initializeBoardGameCells()
        {
            BoardGameCells = new GameCell[BoradSize, BoradSize];
            
            for (int row = 0; row < BoradSize; row++)
            {
                for (int column = 0; column < BoradSize; column++)
                {
                    BoardGameCells[row, column] = new GameCell(row, column);
                }
            }
        }
    }
}