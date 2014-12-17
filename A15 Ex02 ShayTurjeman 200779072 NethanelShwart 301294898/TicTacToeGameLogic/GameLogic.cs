using System;
using System.Collections.Generic;

namespace TicTacToeGameLogic
{
    internal class GameLogic
    {
        internal const Enums.eCellValue k_PlayerOneValue = Enums.eCellValue.X;
        internal const Enums.eCellValue k_PlayerTwoValue = Enums.eCellValue.O;
        internal const int k_MinimumBoardSize = 3;
        internal const int k_MaximumBoardSize = 9;

        private readonly BoardGame r_BoardGame;
        private readonly Enums.eGameType r_GameType;
        private readonly Random r_Random = new Random();
        private bool m_FirstPlayerStartedRound;

        internal GameLogic(int i_BoardSize, Enums.eGameType i_GameType)
        {
            r_BoardGame = new BoardGame(i_BoardSize);
            r_GameType = i_GameType;
        }

        internal Enums.eGameType GameType
        {
            get { return r_GameType; }
        }

        internal GameCell LastSelectedCell
        {
            get { return r_BoardGame.LastSelectedCell; }
        }

        internal int FirstPlayerPoints { get; private set; }

        internal int SecondePlayerPoints { get; private set; }

        internal bool IsFirstPlayerTurn { get; private set; }

        internal Enums.eGameState GameState { get; private set; }

        internal Enums.eGameState FinalGameState
        {
            get
            {
                Enums.eGameState finalGameState;
                if (FirstPlayerPoints > SecondePlayerPoints)
                {
                    finalGameState = Enums.eGameState.FirstPlayerWon;
                }
                else if (FirstPlayerPoints < SecondePlayerPoints)
                {
                    finalGameState = Enums.eGameState.SecondPlayerWon;
                }
                else
                {
                    finalGameState = Enums.eGameState.Tie;
                }
                return finalGameState;
            }
        }

        internal GameCell[,] BoradGameCells
        {
            get { return r_BoardGame.BoardGameCells; }
        }

        internal int BoradSize
        {
            get { return r_BoardGame.BoradSize; }
        }

        internal Enums.ePlayer CurrentPlayerTurn
        {
            get { return IsFirstPlayerTurn ? Enums.ePlayer.PlayerOne : Enums.ePlayer.PlayerTwo; }
        }

        internal void NextPlayerMove(GameCell i_GameCell, out bool o_RoundIsOver)
        {
            setCellState(i_GameCell);
            o_RoundIsOver = false;
            bool isPlayerLost = isPlayerEndedGame(i_GameCell, i_GameCell.Value);
            if (isPlayerLost)
            {
                currentPlayerLose();
                o_RoundIsOver = true;
            }
            else
            {
                if (r_BoardGame.IsThereNoMoreFreeCells)
                {
                    setTie(out o_RoundIsOver);
                }
                else
                {
                    if (GameType == Enums.eGameType.PlayerVsComputer)
                    {
                        computerPlayingLogic(ref o_RoundIsOver);
                    }
                    else
                    {
                        togglePlayers();
                    }
                }
            }
        }

        internal void UserQuit()
        {
            currentPlayerLose();
        }

        internal void InitializeRound()
        {
            r_BoardGame.ClearBorad();
            switch (GameType)
            {
                case Enums.eGameType.PlayerVsPlayer:
                    //toggle between players turn on each round start
                    IsFirstPlayerTurn = !m_FirstPlayerStartedRound;
                    m_FirstPlayerStartedRound = IsFirstPlayerTurn;

                    break;
                case Enums.eGameType.PlayerVsComputer:
                    IsFirstPlayerTurn = true;
                    if (m_FirstPlayerStartedRound)
                    {
                        bool roundIsOverFlag = false;
                        computerPlayingLogic(ref roundIsOverFlag);
                    }
                    m_FirstPlayerStartedRound = !m_FirstPlayerStartedRound;
                    break;
            }
        }

        private void setTie(out bool o_RoundIsOver)
        {
            o_RoundIsOver = true;
            GameState = Enums.eGameState.Tie;
        }

        private void computerPlayingLogic(ref bool io_RoundIsOver)
        {
            togglePlayers();
            GameCell computerSelectedCell = playComputerMove();
            bool isComputerLost = isPlayerEndedGame(computerSelectedCell, computerSelectedCell.Value);
            if (isComputerLost)
            {
                io_RoundIsOver = true;
                currentPlayerLose();
            }
            else
            {
                if (r_BoardGame.IsThereNoMoreFreeCells)
                {
                    setTie(out io_RoundIsOver);
                }
                else
                {
                    togglePlayers();
                }
            }
        }

        private void setCellState(GameCell i_GameCell)
        {
            Enums.eCellValue cellState = CurrentPlayerTurn == Enums.ePlayer.PlayerOne
                ? k_PlayerOneValue : k_PlayerTwoValue;
            r_BoardGame.SetCellState(i_GameCell, cellState);
        }

        private void togglePlayers()
        {
            IsFirstPlayerTurn = !IsFirstPlayerTurn;
        }

        private GameCell playComputerMove()
        {
            bool continueSearching = true;
            List<GameCell> selectedUnWantedCells = new List<GameCell>(r_BoardGame.FreeCellsList.Count);
            GameCell selectedCell, preferredSelecteCell = null;
            bool thisCellEndsGameForComputer, thisCellEndsGameForPlayer;
            do
            {
                selectedCell = getRandomFreeCell();
                thisCellEndsGameForComputer = isPlayerEndedGame(selectedCell, k_PlayerTwoValue);
                thisCellEndsGameForPlayer = isPlayerEndedGame(selectedCell, k_PlayerOneValue);

                //trying to get a cell that doesn't end the game for computer
                // and doesn't help the player
                if (thisCellEndsGameForComputer || thisCellEndsGameForPlayer)
                {
                    selectedUnWantedCells.Add(selectedCell);
                    r_BoardGame.FreeCellsList.Remove(selectedCell);

                    //prefer to select a cell that helps the player
                    //than one that end the game for computer
                    if (thisCellEndsGameForPlayer)
                    {
                        preferredSelecteCell = selectedCell;
                    }
                    if (r_BoardGame.IsThereNoMoreFreeCells)
                    {
                        if (preferredSelecteCell != null)
                        {
                            selectedCell = preferredSelecteCell;
                        }
                        continueSearching = false;
                    }
                }
                else
                {
                    continueSearching = false;
                }
                if (!continueSearching)
                {
                    r_BoardGame.FreeCellsList.AddRange(selectedUnWantedCells);
                }
            } while (continueSearching);

            setCellState(selectedCell);
            return selectedCell;
        }

        private GameCell getRandomFreeCell()
        {
            int randomValue = r_Random.Next(0, r_BoardGame.FreeCellsList.Count);
            GameCell randomGameCell = r_BoardGame.FreeCellsList[randomValue];
            return randomGameCell;
        }

        private bool isPlayerEndedGame(GameCell i_GameCell,
            Enums.eCellValue i_WantedCellState)
        {
            return rowEnded(i_GameCell, i_WantedCellState) ||
                   columnEnded(i_GameCell, i_WantedCellState) ||
                   diagonalEnded(i_GameCell, i_WantedCellState);
        }

        private bool diagonalEnded(GameCell i_GameCell, Enums.eCellValue i_WantedCellState)
        {
            bool firstDiagonalIsTrue = true, secondDiagonal = true;
            if (i_GameCell.RowIndex == i_GameCell.ColumnIndex)
            {
                for (int i = 0 ; i < BoradSize ; i++)
                {
                    if (i_GameCell != BoradGameCells[i, i] && BoradGameCells[i, i].Value != i_WantedCellState)
                    {
                        firstDiagonalIsTrue = false;
                        break;
                    }
                }
            }
            else
            {
                firstDiagonalIsTrue = false;
            }
            if (i_GameCell.RowIndex + i_GameCell.ColumnIndex == BoradSize - 1)
            {
                for (int i = 0 ; i < BoradSize ; i++)
                {
                    if (i_GameCell != BoradGameCells[i, BoradSize - 1 - i]
                        && BoradGameCells[i, BoradSize - 1 - i].Value != i_WantedCellState)
                    {
                        secondDiagonal = false;
                        break;
                    }
                }
            }
            else
            {
                secondDiagonal = false;
            }
            return firstDiagonalIsTrue || secondDiagonal;
        }

        private bool columnEnded(GameCell i_GameCell, Enums.eCellValue i_WantedCellState)
        {
            bool result = true;
            for (int i = 0 ; i < BoradSize ; i++)
            {
                if (i_GameCell != BoradGameCells[i_GameCell.RowIndex, i]
                    && BoradGameCells[i_GameCell.RowIndex, i].Value != i_WantedCellState)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        private bool rowEnded(GameCell i_GameCell, Enums.eCellValue i_WantedCellState)
        {
            bool result = true;
            for (int i = 0 ; i < BoradSize ; i++)
            {
                if (i_GameCell != BoradGameCells[i, i_GameCell.ColumnIndex]
                    && BoradGameCells[i, i_GameCell.ColumnIndex].Value != i_WantedCellState)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        private void currentPlayerLose()
        {
            if (IsFirstPlayerTurn)
            {
                SecondePlayerPoints++;
                GameState = Enums.eGameState.SecondPlayerWon;
            }
            else
            {
                FirstPlayerPoints++;
                GameState = Enums.eGameState.FirstPlayerWon;
            }
        }
    }
}