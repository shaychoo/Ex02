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

        private string getPlayerName()
        {
            string playerName = r_GameManager.CurrentPlayerTurn == Enums.ePlayer.PlayerOne
                ? k_PlayerOneName : k_PlayerTwoName;
            return playerName;
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
            Console.WriteLine(string.Format("{0} turn: ", getPlayerName()));
            Console.WriteLine("please insert row and column in format 'i,j' where i is the row and j is the column");
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
            Console.WriteLine("Game ended with the result:" + r_GameManager.GameState);
            Console.WriteLine(string.Format("First player points - {0},Second player points - {1}.",
                r_GameManager.PlayerOnePoints, r_GameManager.PlayerTwoPoints));
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

            //if(r_GameManager.GameState == 
            //from manager eGameState i_RoundResult, int i_PlayerOnePoints, int i_PlayerTwoPoints,
            //ask for new round
        }

        private void showEndGame()
        {
            //clear board
            //eGameState i_GameResult, int i_PlayerOnePoints, int i_PlayerTwoPoints
            //goodbye!
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
            Console.WriteLine(@"Please select game type:
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
            Board.BoardGameCell[,] boardCells = r_GameManager.BoardCells;

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
                        (boardCells[i, j].Value == Enums.eBoardCellStateValue.Blank) ?
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
            int playerOnePoints = r_GameManager.PlayerOnePoints, playerTwoPoints = r_GameManager.PlayerTwoPoints;
            //todo: Console.WriteLine("");
        }

        private void start()
        {
            getGameStartParameters();
            playGame();
            Console.ReadLine();
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
    }
}