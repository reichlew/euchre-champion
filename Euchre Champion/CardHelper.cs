using System;
using System.Collections.Generic;
using System.Linq;

namespace EuchreChampion
{
    public static class CardHelper
    {
        public static Card GetWinningCard(IEnumerable<Card> cards, Suit trump, Suit leadSuit)
        {
            var trumpCards = cards.Where(x => IsTrump(x, trump));

            if (!trumpCards.Any())
            {
                return cards.Where(x => x.Suit == leadSuit).OrderByDescending(x => x.Value).First();
            }

            var rightBower = trumpCards.SingleOrDefault(x => IsRightBower(x, trump));

            if (rightBower != null)
            {
                return rightBower;
            }
            var leftBower = trumpCards.SingleOrDefault(x => IsLeftBower(x, trump));

            if (leftBower != null)
            {
                return leftBower;
            }

            return trumpCards.OrderByDescending(x => x.Value).First();
        }

        public static bool IsBlackJack(Card card)
        {
            return card.Value == CardValue.Jack && (card.Suit == Suit.Clubs || card.Suit == Suit.Spades);
        }

        private static bool IsTrump(Card card, Suit trump)
        {
            return card.Suit == trump || IsLeftBower(card, trump);
        }

        private static bool IsRightBower(Card card, Suit trump)
        {
            return card.Suit == trump && card.Value == CardValue.Jack;
        }

        private static bool IsLeftBower(Card card, Suit trump)
        {
            return card.Value == CardValue.Jack && card.Suit == GetOppositeSuit(trump);
        }

        private static Suit GetOppositeSuit(Suit suit)
        {
            switch (suit)
            {
                case Suit.Clubs:
                    return Suit.Spades;
                case Suit.Spades:
                    return Suit.Clubs;
                case Suit.Diamonds:
                    return Suit.Hearts;
                case Suit.Hearts:
                    return Suit.Diamonds;
                default:
                    throw new Exception($"{suit} is not a valid Suit.");
            }
        }
    }
}
