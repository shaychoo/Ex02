namespace TicTacToeGameLogic
{
    public class GameLogic
    {
        public const Enums.eBoardCellStateValue k_PlayerOneValue = Enums.eBoardCellStateValue.X;
        public const Enums.eBoardCellStateValue k_PlayerTwoValue = Enums.eBoardCellStateValue.O;
        public const int k_MinimumBoardSize = 3;
        public const int k_MaximumBoardSize = 9;

        private readonly Board r_Board;
        private readonly Enums.eGameType r_GameType;
        private bool m_FirstPlayerStartedRound;

        public GameLogic(int i_BoardSize, Enums.eGameType i_GameType)
        {
            r_Board = new Board(i_BoardSize);
            r_GameType = i_GameType;
        }

        public Enums.eGameType GameType
        {
            get { return r_GameType; }
        }

        public int FirstPlayerPoints { get; private set; }

        public int SecondePlayerPoints { get; private set; }

        public bool IsFirstPlayerTurn { get; private set; }

        public Enums.eGameFinishState GameState { get; private set; }

        public Enums.eGameFinishState FinalGameState
        {
            get
            {
                Enums.eGameFinishState finalGameState;
                if (FirstPlayerPoints > SecondePlayerPoints)
                {
                    finalGameState = Enums.eGameFinishState.FirstPlayerWon;
                }
                else if (FirstPlayerPoints < SecondePlayerPoints)
                {
                    finalGameState = Enums.eGameFinishState.SecondPlayerWon;
                }
                else
                {
                    finalGameState = Enums.eGameFinishState.Tie;
                }
                return finalGameState;
            }
        }

        public Board.BoardGameCell[,] BoradCells
        {
            get { return r_Board.BoardGameCells; }
        }

        public int BoradSize
        {
            get { return r_Board.BoradSize; }
        }

        public Enums.ePlayer CurrentPlayerTurn
        {
            get { return IsFirstPlayerTurn ? Enums.ePlayer.PlayerOne : Enums.ePlayer.PlayerTwo; }
        }

        internal void NextPlayerMove(Board.BoardGameCell i_BoardGameCell, out bool o_RoundIsOver)
        {
            setCellState(i_BoardGameCell);
            o_RoundIsOver = false;
            bool isPlayerLost = isPlayerEndedGame(i_BoardGameCell, i_BoardGameCell.Value);
            if (isPlayerLost)
            {
                currentPlayerLose();
                o_RoundIsOver = true;
            }
            else
            {
                if (r_Board.IsBoardFull)
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
            r_Board.ClearBorad();
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
            GameState = Enums.eGameFinishState.Tie;
        }

        private void computerPlayingLogic(ref bool io_RoundIsOver)
        {
            togglePlayers();
            Board.BoardGameCell computerSelectedCell = playComputerMove();
            bool isComputerLost = isPlayerEndedGame(computerSelectedCell, computerSelectedCell.Value);
            if (isComputerLost)
            {
                io_RoundIsOver = true;
                currentPlayerLose();
            }
            else
            {
                if (r_Board.IsBoardFull)
                {
                    setTie(out io_RoundIsOver);
                }
                else
                {
                    togglePlayers();
                }
            }
        }

        private void setCellState(Board.BoardGameCell i_BoardGameCell)
        {
            Enums.eBoardCellStateValue cellState = CurrentPlayerTurn == Enums.ePlayer.PlayerOne
                ? k_PlayerOneValue : k_PlayerTwoValue;
            r_Board.SetCellState(i_BoardGameCell, cellState);
        }

        private void togglePlayers()
        {
            IsFirstPlayerTurn = !IsFirstPlayerTurn;
        }

        private Board.BoardGameCell playComputerMove()
        {
            Board.BoardGameCell selectedCell = r_Board.GetRandomFreeCell();
            setCellState(selectedCell);
            return selectedCell;
        }

        private bool isPlayerEndedGame(Board.BoardGameCell i_BoardGameCell,
            Enums.eBoardCellStateValue i_WantedCellState)
        {
            return rowEnded(i_BoardGameCell, i_WantedCellState) ||
                   columnEnded(i_BoardGameCell, i_WantedCellState) ||
                   diagonalEnded(i_BoardGameCell, i_WantedCellState);
        }

        private bool diagonalEnded(Board.BoardGameCell i_BoardGameCell, Enums.eBoardCellStateValue i_WantedCellState)
        {
            bool firstDiagonalIsTrue = true, secondDiagonal = true;
            if (i_BoardGameCell.RowIndex == i_BoardGameCell.ColumnIndex)
            {
                for (int i = 0 ; i < BoradSize ; i++)
                {
                    if (i_BoardGameCell != BoradCells[i, i] && BoradCells[i, i].Value != i_WantedCellState)
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
            if (i_BoardGameCell.RowIndex + i_BoardGameCell.ColumnIndex == BoradSize - 1)
            {
                for (int i = 0 ; i < BoradSize ; i++)
                {
                    if (i_BoardGameCell != BoradCells[i, BoradSize - 1 - i]
                        && BoradCells[i, BoradSize - 1 - i].Value != i_WantedCellState)
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

        private bool columnEnded(Board.BoardGameCell i_BoardGameCell, Enums.eBoardCellStateValue i_WantedCellState)
        {
            bool result = true;
            for (int i = 0 ; i < BoradSize ; i++)
            {
                if (i_BoardGameCell != BoradCells[i_BoardGameCell.RowIndex, i]
                    && BoradCells[i_BoardGameCell.RowIndex, i].Value != i_WantedCellState)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        private bool rowEnded(Board.BoardGameCell i_BoardGameCell, Enums.eBoardCellStateValue i_WantedCellState)
        {
            bool result = true;
            for (int i = 0 ; i < BoradSize ; i++)
            {
                if (i_BoardGameCell != BoradCells[i, i_BoardGameCell.ColumnIndex]
                    && BoradCells[i, i_BoardGameCell.ColumnIndex].Value != i_WantedCellState)
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
                GameState = Enums.eGameFinishState.SecondPlayerWon;
            }
            else
            {
                FirstPlayerPoints++;
                GameState = Enums.eGameFinishState.FirstPlayerWon;
            }
        }
    }
}