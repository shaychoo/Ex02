namespace TicTacToeGameLogic
{
    public class Enums
    {
        public enum eBoardCellStateValue
        {
            Blank,
            X,
            O,
        }

        public enum eGameFinishState
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