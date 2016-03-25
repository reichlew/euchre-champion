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
        private bool SHOW_DEBUG_INFO = true;

        private List<Player> _players { get; set; }
        private SpriteBatch _spriteBatch { get; set; }
        private Board _board { get; set; }
        private InputManager _inputManager { get; set; }
        private Drawer _drawer { get; set; }
        private Dealer _dealer { get; set; }

        private int _dealerIndex { get; set; }
        private int _playerToAct { get; set; }
        private Suit _leadSuit { get; set; }

        private Score _score { get; set; }
        private DealType _dealType { get; set; }
        private State _state { get; set; }
        private Suit _trump { get; set; }
        private Card _flippedCard { get; set; }

        private bool _handOver { get { return _players.All(x => x.PlayedCard != null); } }
        private bool _roundOver { get { return _players.All(x => !x.Hand.Any()); } }
        private bool _leading { get { return _players.All(x => x.PlayedCard == null); } }
        private bool _userCalledTrump { get; set; }

        public GameManager(SpriteBatch spriteBatch, List<Card> cards, List<Player> players, Board board, SpriteFont font, InputManager inputManager)
        {
            _spriteBatch = spriteBatch;
            _players = players;
            _board = board;
            _inputManager = inputManager;

            _drawer = new Drawer(_spriteBatch, _board, font);

            Initialize();

            LoadUserSettings();

            SetRandomDealerIndex();

            _dealer = new Dealer(cards, _players, _dealType, _dealerIndex);
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
                    {
                        PrepareToDeal();
                        break;
                    }
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
                case State.Playing:
                    {

                        if (!_handOver)
                        {
                            CheckIfCardPlayed();
                            break;
                        }
                        else
                        {
                            AwardTrick();
                            _players.ForEach(x => x.PlayedCard = null);
                        }

                        if (_roundOver)
                        {
                            UpdateScore();

                            _dealerIndex = _dealerIndex.NextPlayer();
                            _playerToAct = _dealerIndex.NextPlayer();
                            _players.ForEach(x => x.Tricks = 0);

                            if (_score.UserScore >= 10 || _score.OpponentScore >= 10)
                            {
                                _state = State.GameOver;
                            }
                            else
                            {
                                _state = State.DealerFound;
                            }
                        }
                        break;
                    }
            }
        }


        public void Draw()
        {
            _spriteBatch.Begin();

            if (SHOW_DEBUG_INFO)
            {
                _drawer.DrawDebugInfo(_inputManager, _state, _dealerIndex, _trump, _playerToAct);
            }

            _drawer.DrawScore(_score);

            if (_flippedCard != null)
            {
                _drawer.DrawFlippedCard(_flippedCard);
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

            _spriteBatch.End();
        }

        private void GetDealer()
        {
            _dealer.DealSingle();

            var player = _players.SingleOrDefault(x => x.DealtCard != null && CardHelper.IsBlackJack(x.DealtCard));
            if (player != null)
            {
                _dealerIndex = _players.IndexOf(player);
                _playerToAct = _dealerIndex.NextPlayer();

                _state = State.DealerFound;
            }
        }

        private void PrepareToDeal()
        {
            _dealer.Reset(_dealerIndex);
            _players.ForEach(x => x.DealtCard = null);
            _state = State.Dealing;
        }

        private void DealNext()
        {
            _dealer.DealNext();

            if (_players.All(x => x.Hand.Count == 5))
            {
                _state = State.Dealt;
            }
        }

        private void FlipCard()
        {
            _flippedCard = _dealer.FlipCard();
            _state = State.CallingTrump;
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
                    _userCalledTrump = IsUserTeam(_players[_playerToAct].Position);

                }
                else if (_inputManager.IsKeyPressed(Keys.N))
                {
                    if (_playerToAct == _dealerIndex)
                    {
                        _flippedCard = null;
                    }
                    _playerToAct = _playerToAct.NextPlayer();
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
                    _playerToAct = _playerToAct.NextPlayer();
                }
                if (_inputManager.IsKeyPressed(Keys.D1))
                {
                    _trump = Suit.Clubs;
                    _state = State.Playing;
                    _userCalledTrump = IsUserTeam(_players[_playerToAct].Position);
                    _playerToAct = _dealerIndex.NextPlayer();
                }
                else if (_inputManager.IsKeyPressed(Keys.D2))
                {
                    _trump = Suit.Diamonds;
                    _state = State.Playing;
                    _userCalledTrump = IsUserTeam(_players[_playerToAct].Position);
                    _playerToAct = _dealerIndex.NextPlayer();
                }
                else if (_inputManager.IsKeyPressed(Keys.D3))
                {
                    _trump = Suit.Hearts;
                    _state = State.Playing;
                    _userCalledTrump = IsUserTeam(_players[_playerToAct].Position);
                    _playerToAct = _dealerIndex.NextPlayer();
                }
                else if (_inputManager.IsKeyPressed(Keys.D4))
                {
                    _trump = Suit.Spades;
                    _state = State.Playing;
                    _userCalledTrump = IsUserTeam(_players[_playerToAct].Position);
                    _playerToAct = _dealerIndex.NextPlayer();
                }
            }
        }

        private bool IsUserTeam(Position position)
        {
            return position == Position.North || position == Position.South;
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

            if (_leading)
            {
                _leadSuit = chosenCard.Suit;
            }

            _players[_playerToAct].PlayedCard = chosenCard;
            _players[_playerToAct].Hand.RemoveAt(index);

            _playerToAct = _playerToAct.NextPlayer();
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
            _state = State.Playing;
            _playerToAct = _dealerIndex.NextPlayer();
        }

        private void AwardTrick()
        {
            var winningCard = CardHelper.GetWinningCard(_players.Select(x => x.PlayedCard), _trump, _leadSuit);

            var winner = _players.Single(x => x.PlayedCard == winningCard);
            winner.Tricks++;
            _playerToAct = _players.IndexOf(winner);
        }

        private void UpdateScore()
        {            
            var userTricks = _players.Where(x => x.Position == Position.North || x.Position == Position.South).Sum(x => x.Tricks);


            if (userTricks > 2)
            {
                if (userTricks == 5)
                {
                    _score.UserScore += 2;
                }
                else
                {
                    _score.UserScore++;

                    if (!_userCalledTrump)
                    {
                        _score.UserScore++;
                    }
                }
            }
            else
            {
                var opponentTricks = _players.Where(x => x.Position == Position.East || x.Position == Position.West).Sum(x => x.Tricks);

                if (opponentTricks == 5)
                {
                    _score.OpponentScore += 2;
                }
                else
                {
                    _score.OpponentScore += 1;

                    if (_userCalledTrump)
                    {
                        _score.OpponentScore++;
                    }
                }
            }
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

