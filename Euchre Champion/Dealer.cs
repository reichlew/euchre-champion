using System.Collections.Generic;
using System.Linq;

namespace EuchreChampion
{
    public class Dealer
    {
        private List<Card> _fullDeck { get; set; }
        private List<Player> _players { get; set; }
        private DealType _dealType { get; set; }

        private List<Card> _activeDeck { get; set; }
        private int _dealer { get; set; }
        private int _step { get; set; }

        private int _nextPlayer { get { return (_dealer + _step + 1) % 4; } }

        public Dealer(List<Card> cards, List<Player> players, DealType dealType, int dealerIndex)
        {
            _fullDeck = cards;
            _players = players;
            _dealType = dealType;            

            Reset(dealerIndex);
        }

        public void Reset(int dealerIndex)
        {
            _activeDeck = _fullDeck.Shuffle().ToList();
            _step = 0;

            foreach(var player in _players)
            {
                player.Hand = new List<Card>();
                player.DealtCard = null;
            }

            _dealer = dealerIndex;
        }

        public void DealSingle()
        {
            var card = _activeDeck.First();
            _players[_nextPlayer].DealtCard = card;
            _activeDeck.Remove(card);
            _step++;
        }

        public bool DealNext()
        {
            var numCards = NumCardsToDeal();
            _players[_nextPlayer].Hand.AddRange(_activeDeck.Take(numCards));
            _activeDeck.RemoveRange(0, numCards);

            if (_step == 7)
            {
                return true;
            }
            else
            {
                _step++;
                return false;
            }
        }

        public Card FlipCard()
        {
            var card = _activeDeck.First();
            _activeDeck.Remove(card);
            return card;
        }

        private int NumCardsToDeal()
        {
            var isFirstCycle = _step < 4;
            var isEven = _step % 2 == 0;
            switch (_dealType)
            {
                case DealType.TwoThree:
                    if (isFirstCycle)
                    {
                        if (isEven)
                        {
                            return 2;
                        }
                        else
                        {
                            return 3;
                        }
                    }
                    else
                    {
                        if (isEven)
                        {
                            return 3;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                default:
                    return 0;
            }
        }

        
    }
}