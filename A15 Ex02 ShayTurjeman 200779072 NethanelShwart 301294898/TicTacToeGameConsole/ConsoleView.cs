using System;
using System.Text;
using Ex02.ConsoleUtils;
using TicTacToeGameLogic;

namespace TicTacToeGameConsole
{
    public class ConsoleView
    {
        private const string k_StopKey = "q";
        private const string k_PlayerOneName = "Player 1";
        private const string k_PlayerTwoName = "Player 2";

        private readonly GameManager r_GameManager;

        public ConsoleView()
        {
            r_GameManager = new GameManager();
            clearScreen();
            start();
        }

        private string firstPlayerName
        {
            get
            {
                return r_GameManager.GameType == Enums.eGameType.PlayerVsPlayer
                    ? k_PlayerOneName : Enums.ePlayer.Player.ToString();
            }
        }

        private string secondPlayerName
        {
            get
            {
                return r_GameManager.GameType == Enums.eGameType.PlayerVsPlayer
                    ? k_PlayerTwoName : Enums.ePlayer.Computer.ToString();
            }
        }

        private void start()
        {
            getGameStartParameters();
            playGame();
        }

        private void getGameStartParameters()
        {
            string errorMessage;
            do
            {
                errorMessage = null;
                int boardSize = getBoardSizeFromUser();
                Enums.eGameType gameType = getGameType();
                r_GameManager.InitializeGame(boardSize, gameType, ref errorMessage);
                if (errorMessage != null)
                {
                    showErrorMessage(errorMessage);
                }
                clearScreen();
            } while (errorMessage != null);
            clearScreen();
        }

        private string getCurrentPlayerName()
        {
            string playerName = r_GameManager.CurrentPlayerTurn == Enums.ePlayer.PlayerOne
                ? firstPlayerName : secondPlayerName;
            return playerName;
        }

        private string getOtherPlayerName()
        {
            string playerName = r_GameManager.CurrentPlayerTurn != Enums.ePlayer.PlayerOne
                ? firstPlayerName : secondPlayerName;
            return playerName;
        }

        private void playGame()
        {
            bool playAnotherRound;
            do
            {
                playRound();
                showEndRound(out playAnotherRound);
            } while (playAnotherRound);
            showEndGame();
        }

        private void playRound()
        {
            bool roundIsOver;
            r_GameManager.InitializeRound();
            string errorMessage;
            do
            {
                errorMessage = null;
                clearScreen();
                showRound();
                getNextMove(ref errorMessage, out roundIsOver);
                if (errorMessage != null && !roundIsOver)
                {
                    showErrorMessage(errorMessage);
                }
            } while (!roundIsOver);
        }

        private void clearScreen()
        {
            Screen.Clear();
        }

        private void showRound()
        {
            showBoard();
            showScore();
        }

        private void getNextMove(ref string io_ErrorMessage, out bool o_RoundIsOver)
        {
            if (r_GameManager.LastSelectedCell != null)
            {
                char row = (char)(r_GameManager.LastSelectedCell.RowIndex + 'A');
                string column = (r_GameManager.LastSelectedCell.ColumnIndex + 1).ToString();
                Console.WriteLine(string.Format("{0} played: {1} , {2}{3} ", getOtherPlayerName(), row, column,
                    Environment.NewLine));
            }
            Console.WriteLine(string.Format("{0} turn: ", getCurrentPlayerName()));
            Console.WriteLine("What's your move? (letter and number)");
            string userInput = Console.ReadLine();
            if (userInput.ToLower() == k_StopKey)
            {
                r_GameManager.UserQuit();
                o_RoundIsOver = true;
            }
            else
            {
                handleUserInput(userInput, ref io_ErrorMessage, out o_RoundIsOver);
            }
        }

        private void handleUserInput(string i_UserInput, ref string io_ErrorMessage, out bool o_RoundIsOver)
        {
            string choosenLetter = null;
            string choosenNumber = null;

            foreach (char currentChar in i_UserInput)
            {
                if (char.IsDigit(currentChar))
                {
                    if (choosenNumber == null)
                    {
                        choosenNumber = currentChar.ToString();
                    }
                    else
                    {
                        choosenNumber = null;
                        break;
                    }
                }
                if (char.IsLetter(currentChar))
                {
                    if (choosenLetter == null)
                    {
                        choosenLetter = char.ToUpper(currentChar).ToString();
                    }
                    else
                    {
                        choosenLetter = null;
                        break;
                    }
                }
            }

            if (choosenLetter == null || choosenNumber == null)
            {
                io_ErrorMessage = "input is invalid";
                o_RoundIsOver = false;
            }
            else
            {
                r_GameManager.SetNextMove(int.Parse((choosenLetter[0] - 'A').ToString()),
                    int.Parse(choosenNumber) - 1,
                    ref io_ErrorMessage, out o_RoundIsOver);
            }
        }

        private void showErrorMessage(string i_ErrorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(i_ErrorMessage);
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Press enter To continue..");
            Console.ReadLine();
        }

        private void showEndRound(out bool o_PlayAnotherRound)
        {
            clearScreen();
            showBoard();
            switch (r_GameManager.GameState)
            {
                case Enums.eGameState.Tie:
                    Console.WriteLine("Round ended with a tie!");
                    break;
                case Enums.eGameState.FirstPlayerWon:
                case Enums.eGameState.SecondPlayerWon:
                    string winner = r_GameManager.GameState == Enums.eGameState.FirstPlayerWon
                        ? secondPlayerName : firstPlayerName;
                    Console.WriteLine("Round ended - " + winner + " won this round!");
                    break;
            }

            showScore();
            string userInput = null;
            bool validInput = false;
            do
            {
                Console.WriteLine("Do you want to play another round? y/n");
                userInput = Console.ReadLine();
                validInput = userInput.ToLower() == "n" || userInput.ToLower() == "y";
                if (!validInput)
                {
                    showErrorMessage("input is not valid");
                }
            } while (!validInput);

            o_PlayAnotherRound = userInput.ToLower() == "y";
        }

        private void showEndGame()
        {
            clearScreen();
            Console.WriteLine("Game Over!");
            showScore();
            switch (r_GameManager.FinalGameState)
            {
                case Enums.eGameState.Tie:
                    Console.WriteLine("Game ended with a tie!");
                    break;
                case Enums.eGameState.FirstPlayerWon:
                    if (r_GameManager.GameType == Enums.eGameType.PlayerVsComputer)
                    {
						Console.WriteLine("You Lose  :( ");
                    }
                    else
                    {
                        Console.WriteLine(firstPlayerName + " Won!");
                    }
                    break;
                case Enums.eGameState.SecondPlayerWon:
                    if (r_GameManager.GameType == Enums.eGameType.PlayerVsComputer)
                    {
						Console.WriteLine("You Won !! :)");
                    }
                    else
                    {
                        Console.WriteLine(secondPlayerName + " Won!");
                    }
                    break;
            }

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }

        private int getBoardSizeFromUser()
        {
            int boardSize;
            Console.WriteLine("Please enter wanted board size between 3 to 9 (3x3 - 9x9)");
            string userInput = Console.ReadLine();
            int.TryParse(userInput, out boardSize);
            return boardSize;
        }

        private Enums.eGameType getGameType()
        {
            Enums.eGameType gameType;
            Console.WriteLine(@"
Please select game type:

1 - Player vs Player 
2 - Player vs Computer");
            string userInput = Console.ReadLine();
            int intGameType;
            int.TryParse(userInput, out intGameType);
            gameType = (Enums.eGameType)intGameType;
            return gameType;
        }

        private void showBoard()
        {
            GameCell[,] boardCells = r_GameManager.BoardGameCells;

            StringBuilder gameBoardStringBuilder = new StringBuilder(" ");
            for (int i = 0 ; i < r_GameManager.BoardSize ; i++)
            {
                gameBoardStringBuilder.AppendFormat(" {0} ", i + 1);
            }

            gameBoardStringBuilder.Append(Environment.NewLine);

            for (int i = 0 ; i < r_GameManager.BoardSize ; i++)
            {
                gameBoardStringBuilder.AppendFormat("{0}| ", (char)(i + 'A'));
                for (int j = 0 ; j < r_GameManager.BoardSize ; j++)
                {
                    gameBoardStringBuilder.AppendFormat("{0}| ",
                        (boardCells[i, j].Value == Enums.eCellValue.Blank) ?
                            " " :
                            boardCells[i, j].Value.ToString());
                }
                gameBoardStringBuilder.Append(Environment.NewLine);
                gameBoardStringBuilder.AppendLine("  " + new string('=', r_GameManager.BoardSize * 3));
            }

            Console.WriteLine(gameBoardStringBuilder);
        }

        private void showScore()
        {
            string result = string.Format(
                @"
Total Points:

{2} - {3}
   {0}    :   {1}
", r_GameManager.PlayerOnePoints, r_GameManager.PlayerTwoPoints, secondPlayerName, firstPlayerName);

            Console.WriteLine(result);
        }
    }
}