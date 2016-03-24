using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace EuchreChampion
{
    public class GameManager
    {
        private bool SHOW_DEBUG_CONTROLS = true;

        private List<Card> _cards { get; set; }
        private List<Player> _players { get; set; }
        private SpriteBatch _spriteBatch { get; set; }
        private Board _board { get; set; }
        private InputManager _inputManager { get; set; }
        private Drawer _drawer { get; set; }
        private Dealer _dealer { get; set; }

        private int _dealerIndex { get; set; }
        private int _playerToAct { get; set; }
        private Suit _suitLead { get; set; }

        private Score _score { get; set; }
        private DealType _dealType { get; set; }
        private State _state { get; set; }
        private Suit _trump { get; set; }
        private Card _flippedCard { get; set; }

        private bool _handOver { get { return _players.All(x => x.PlayedCard != null); } }
        private bool _roundOver { get { return _players.All(x => !x.Hand.Any()); } }

        private bool _leading { get { return _handOver; } }

        public GameManager(SpriteBatch spriteBatch, List<Card> cards, List<Player> players, Board board, SpriteFont font, InputManager inputManager)
        {
            _spriteBatch = spriteBatch;
            _cards = cards;
            _players = players;
            _board = board;
            _inputManager = inputManager;

            _drawer = new Drawer(_spriteBatch, _board, font);

            Initialize();

            LoadUserSettings();

            SetRandomDealerIndex();

            _dealer = new Dealer(_cards, _players, _dealType, _dealerIndex);
        }

        public void Update()
        {
            _inputManager.Update(Keyboard.GetState(), Mouse.GetState(), TouchPanel.GetState());

            switch (_state)
            {
                case State.FirstGame:
                    {
                        GetDealer();
                        break;
                    }
                case State.DealerFound:
                case State.Dealing:
                    {
                        DealNext();
                        break;
                    }
                case State.Dealt:
                    {
                        FlipCard();
                        break;
                    }
                case State.CallingTrump:
                    {
                        GetDecision();
                        break;
                    }
                case State.ChoosingCard:
                    {
                        CheckIfCardChosen();
                        break;
                    }
                case State.TrumpCalled:
                    {

                        if (_handOver)
                        {
                            AwardTrick();
                            ClearBoard();
                            if (_roundOver)
                            {
                                UpdateScore();
                                _dealerIndex = (_dealerIndex + 1) % 4;
                                _playerToAct = (_dealerIndex + 1) % 4;
                                _players.ForEach(x => x.Tricks = 0);
                                _state = State.DealerFound;
                            }
                        }
                        else
                        {
                            CheckIfCardPlayed();
                        }
                        break;
                    }
            }
        }

        private void AwardTrick()
        {
            var playedTrump = _players.Where(x => IsTrump(x.PlayedCard));

            if (playedTrump == null)
            {
                _players.Where(x => x.PlayedCard.Suit == _suitLead).OrderByDescending(x => x.PlayedCard.Value).First().Tricks++;
            }
            else
            {
                
            }
        }

        private void UpdateScore()
        {

        }

        private bool IsTrump(Card playedCard)
        {
            return playedCard.Suit == _trump || IsLeftBower(playedCard);
        }

        private bool IsLeftBower(Card playedCard)
        {
            return playedCard.Value == CardValue.Jack && playedCard.Suit == OppositeSuit(_trump);
        }

        private Suit OppositeSuit(Suit suit)
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

        private void ClearBoard()
        {
            _players.ForEach(x => x.PlayedCard = null);
        }

        private void CheckIfCardPlayed()
        {
            if (_inputManager.IsKeyPressed(Keys.D1))
            {
                PlayCard(0);
            }
            if (_inputManager.IsKeyPressed(Keys.D2))
            {
                PlayCard(1);
            }
            else if (_inputManager.IsKeyPressed(Keys.D3))
            {
                PlayCard(2);
            }
            else if (_inputManager.IsKeyPressed(Keys.D4))
            {
                PlayCard(3);
            }
            else if (_inputManager.IsKeyPressed(Keys.D5))
            {
                PlayCard(4);
            }
        }

        private void PlayCard(int index)
        {
            var chosenCard = _players[_playerToAct].Hand[index];
            _players[_playerToAct].PlayedCard = chosenCard;
            _players[_playerToAct].Hand.RemoveAt(index);

            _playerToAct = (_playerToAct + 1) % 4;

            if (_leading)
            {
                _suitLead = chosenCard.Suit;
            }
        }

        private void CheckIfCardChosen()
        {
            if (_inputManager.IsKeyPressed(Keys.D1))
            {
                SwapCard(0);
            }
            if (_inputManager.IsKeyPressed(Keys.D2))
            {
                SwapCard(1);
            }
            else if (_inputManager.IsKeyPressed(Keys.D3))
            {
                SwapCard(2);
            }
            else if (_inputManager.IsKeyPressed(Keys.D4))
            {
                SwapCard(3);
            }
            else if (_inputManager.IsKeyPressed(Keys.D5))
            {
                SwapCard(4);
            }
        }

        private void SwapCard(int index)
        {
            _players[_playerToAct].Hand[index] = _flippedCard;
            _flippedCard = null;
            _state = State.TrumpCalled;
            _playerToAct = (_dealerIndex + 1) % 4;
        }

        private void GetDecision()
        {
            if (_flippedCard != null)
            {
                if (_inputManager.IsKeyPressed(Keys.Y))
                {
                    _trump = _flippedCard.Suit;
                    _state = State.ChoosingCard;
                    _playerToAct = _dealerIndex;

                }
                else if (_inputManager.IsKeyPressed(Keys.N))
                {
                    if (_playerToAct == _dealerIndex)
                    {
                        _flippedCard = null;
                    }
                    _playerToAct = (_playerToAct + 1) % 4;
                }
            }
            else
            {
                if (_inputManager.IsKeyPressed(Keys.N))
                {
                    if (_playerToAct == _dealerIndex)
                    {
                        return;
                    }
                    _playerToAct = (_playerToAct + 1) % 4;
                }
                if (_inputManager.IsKeyPressed(Keys.D1))
                {
                    _trump = Suit.Clubs;
                    _state = State.TrumpCalled;
                    _playerToAct = (_dealerIndex + 1) % 4;
                }
                else if (_inputManager.IsKeyPressed(Keys.D2))
                {
                    _trump = Suit.Diamonds;
                    _state = State.TrumpCalled;
                    _playerToAct = (_dealerIndex + 1) % 4;
                }
                else if (_inputManager.IsKeyPressed(Keys.D3))
                {
                    _trump = Suit.Hearts;
                    _state = State.TrumpCalled;
                    _playerToAct = (_dealerIndex + 1) % 4;
                }
                else if (_inputManager.IsKeyPressed(Keys.D4))
                {
                    _trump = Suit.Spades;
                    _state = State.TrumpCalled;
                    _playerToAct = (_dealerIndex + 1) % 4;
                }
            }
        }

        public void Draw()
        {
            _spriteBatch.Begin();

            if (SHOW_DEBUG_CONTROLS)
            {
                _drawer.DrawDebugInfo(_inputManager, _state, _dealerIndex, _trump, _playerToAct);
            }

            foreach (var player in _players)
            {
                if (_state == State.FirstGame || _state == State.DealerFound)
                {
                    _drawer.DrawDealtCard(player);
                }
                else
                {
                    _drawer.DrawHand(player);
                }
            }

            if (_flippedCard != null)
            {
                _drawer.DrawFlippedCard(_flippedCard);
            }

            _spriteBatch.End();
        }

        private void Initialize()
        {
            _score = new Score(0, 0);
            _state = State.FirstGame;
        }

        private void LoadUserSettings()
        {
            _dealType = DealType.TwoThree;
        }

        private void SetRandomDealerIndex()
        {
            var random = new Random();
            _dealerIndex = random.Next(0, _players.Count - 1);
        }

        private void DealNext()
        {
            if (_state == State.DealerFound)
            {
                _dealer.Reset(_dealerIndex);
                _state = State.Dealing;
            }

            if (_dealer.DealNext())
            {
                _state = State.Dealt;
            }
        }

        private void GetDealer()
        {
            _dealer.DealSingle();

            var player = _players.SingleOrDefault(x => x.DealtCard != null && x.DealtCard.IsBlackJack);
            if (player != null)
            {
                _dealerIndex = _players.IndexOf(player);
                _state = State.DealerFound;
                _playerToAct = (_dealerIndex + 1) % 4;
            }
        }

        private void FlipCard()
        {
            _flippedCard = _dealer.FlipCard();
            _state = State.CallingTrump;
        }

        private void HandleClick(Point location)
        {
            var actualPoint = _board.TransformPoint(location);

            var rectangles = _board.GetHandDestinations(Position.South);
            for (int i = 0; i < _players[2].Hand.Count; i++)
            {
                if (rectangles[i].Contains(actualPoint))
                {
                    var card = _players[2].Hand[i];
                    _players[2].Hand.RemoveAt(i);
                    _players[2].PlayedCard = card;
                }
            }
        }
    }
}

