namespace EuchreChampion
{
    public class Score
    {
        public int UserScore { get; set; }
        public int OpponentScore { get; set; }

        public Score(int userScore, int opponentScore)
        {
            UserScore = userScore;
            OpponentScore = opponentScore;
        }
    }
}