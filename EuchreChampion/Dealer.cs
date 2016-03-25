using System.Collections.Generic;
using System.Linq;

namespace EuchreChampion
{
    public class Dealer
    {
        private static Dictionary<DealType, int[]> NumberOfCardsToDeal = new Dictionary<DealType, int[]>()
        {
            {DealType.TwoThree, new int[] { 2,3,2,3,3,2,3,2} },
            {DealType.ThreeTwo, new int[] { 3,2,3,2,2,3,2,3} }
        };

        private List<Card> _fullDeck { get; set; }
        private List<Player> _players { get; set; }
        private DealType _dealType { get; set; }

        private List<Card> _activeDeck { get; set; }

        private int _dealerIndex { get; set; }
        private int _playerToDealTo { get; set; }
        private int _step { get; set; }

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

            _dealerIndex = dealerIndex;
            _playerToDealTo = _dealerIndex.NextPlayer();
            _step = 0;
        }

        public void DealSingle()
        {
            var card = _activeDeck.First();
            _players[_playerToDealTo].DealtCard = card;
            _activeDeck.Remove(card);

            _playerToDealTo = _playerToDealTo.NextPlayer();
        }

        public void DealNext()
        {
            var numCards = NumberOfCardsToDeal[_dealType][_step];
            _players[_playerToDealTo].Hand.AddRange(_activeDeck.Take(numCards));
            _activeDeck.RemoveRange(0, numCards);

            _playerToDealTo = _playerToDealTo.NextPlayer();
            _step++;
        }

        public Card FlipCard()
        {
            var card = _activeDeck.First();
            _activeDeck.Remove(card);
            return card;
        }
    }
}