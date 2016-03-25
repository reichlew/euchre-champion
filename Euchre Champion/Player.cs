using System.Collections.Generic;

namespace EuchreChampion
{
    public class Player
    {
        public Position Position { get; private set; }
        public List<Card> Hand { get; set; }
        public Card DealtCard { get; set; }
        public Card PlayedCard { get; set; }
        public int Tricks { get; set; }

        public Player(Position position)
        {
            Position = position;
            Hand = new List<Card>();
        }
    }
}