using Microsoft.Xna.Framework.Graphics;    

namespace EuchreChampion
{
    public class Card
    {
        public Texture2D Front { get; private set; }
        public Texture2D Back { get; private set; }
        public Suit Suit { get; private set; }
        public CardValue Value { get; private set; }
        
        public Card(Texture2D front, Texture2D back, Suit suit, CardValue value)
        {
            Front = front;
            Back = back;
            Suit = suit;
            Value = value;
        }
    }
}
