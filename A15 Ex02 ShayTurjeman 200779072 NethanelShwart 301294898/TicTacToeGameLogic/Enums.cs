namespace TicTacToeGameLogic
{
    public class Enums
    {
        public enum eCellValue
        {
            Blank,
            X,
            O,
        }

        public enum eGameState
        {
            Tie,
            FirstPlayerWon,
            SecondPlayerWon,
        }

        public enum eGameType
        {
            PlayerVsPlayer = 1,
            PlayerVsComputer = 2,
        }

        public enum ePlayer
        {
            PlayerOne,
            PlayerTwo,
            Player,
            Computer,
        }
    }
}