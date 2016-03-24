using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EuchreChampion
{
    public class ContentLoader
    {
        private ContentManager _content { get; set; }

        public ContentLoader(ContentManager content)
        {
            _content = content;
        }

        public List<Card> LoadCards()
        {
            var cards = new List<Card>();

            var back = _content.Load<Texture2D>("Backs\\Card-Back-01");

            foreach (var suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (var value in Enum.GetValues(typeof(CardValue)))
                {
                    var cardLocation = $"Cards\\{value}Of{suit}";
                    var front = _content.Load<Texture2D>(cardLocation);
                    var card = new Card(front, back, (Suit)suit, (CardValue)value);
                    card.IsFaceUp = true;
                    cards.Add(card);
                }
            }

            return cards;
        }

        public SpriteFont LoadFont()
        {
            return _content.Load<SpriteFont>("Consolas");
        }
    }
}