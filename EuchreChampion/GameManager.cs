using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace EuchreChampion
{
    public class GameManager
    {
        private bool SHOW_DEBUG_INFO = true;        

        private List<Player> _players { get; set; }
        private Dealer _dealer { get; set; }
        private Drawer _drawer { get; set; }
        private InputManager _inputManager { get; set; }

        private Score _score { get; set; }
        private State _state { get; set; }
        
        private Card _flippedCard { get; set; }

        private int _dealerIndex { get; set; }
        private int _playerToAct { get; set; }
        private int _playerToSkip { get; set; }

        private Hand _currentHand { get; set; }    

        private bool _handOver { get { return (_players.Where(x => x.PlayedCard != null).ToList().Count == 4) || (_currentHand.IsLoner && _players.Where(x => x.PlayedCard != null).ToList().Count == 3); } }
        private bool _roundOver { get { return _players.All(x => !x.Hand.Any()); } }
        private bool _leading { get { return _players.All(x => x.PlayedCard == null); } }

        public GameManager(List<Player> players, Dealer dealer, Drawer drawer, InputManager inputManager)
        {
            _players = players;
            _dealer = dealer;
            _drawer = drawer;
            _inputManager = inputManager;            

            Initialize();            
        }

        private void Initialize()
        {
            _score = new Score(0, 0);
            _state = State.FirstGame;
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

                            _flippedCard = null;
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
            if (SHOW_DEBUG_INFO)
            {
                _drawer.DrawDebugInfo(_inputManager, _state, _dealerIndex, _currentHand?.Trump, _playerToAct);
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
            _currentHand = new Hand();
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
            _flippedCard = _dealer.GetTopCard();
            _state = State.CallingTrump;
        }

        private void GetDecision()
        {
            if (_flippedCard != null)
            {
                if (_inputManager.IsKeyPressed(Keys.Y))
                {
                    if (_inputManager.IsKeyDown(Keys.LeftShift) || _inputManager.IsKeyDown(Keys.RightShift))
                    {
                        GoAlone();
                    }
                    
                    _currentHand.Trump = _flippedCard.Suit;
                    _currentHand.UserCalledTrump = PositionHelper.IsUserTeam(_players[_playerToAct].Position);
                    _playerToAct = _dealerIndex;

                    _state = State.ChoosingCard;                    

                    if (_currentHand.IsLoner && _playerToAct == _playerToSkip)
                    {
                        _playerToAct = _playerToAct.NextPlayer();
                        _flippedCard = null;
                        _state = State.Playing;
                    }
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
                    if (_inputManager.IsKeyDown(Keys.LeftShift) || _inputManager.IsKeyDown(Keys.RightShift))
                    {
                        GoAlone();
                    }

                    SetupHand(Suit.Clubs);
                }
                else if (_inputManager.IsKeyPressed(Keys.D2))
                {
                    if (_inputManager.IsKeyDown(Keys.LeftShift) || _inputManager.IsKeyDown(Keys.RightShift))
                    {
                        GoAlone();
                    }

                    SetupHand(Suit.Diamonds);
                }
                else if (_inputManager.IsKeyPressed(Keys.D3))
                {
                    if (_inputManager.IsKeyDown(Keys.LeftShift) || _inputManager.IsKeyDown(Keys.RightShift))
                    {
                        GoAlone();
                    }

                    SetupHand(Suit.Hearts);
                }
                else if (_inputManager.IsKeyPressed(Keys.D4))
                {
                    if (_inputManager.IsKeyDown(Keys.LeftShift) || _inputManager.IsKeyDown(Keys.RightShift))
                    {
                        GoAlone();
                    }

                    SetupHand(Suit.Spades);
                }
            }
        }

        private void SetupHand(Suit trump)
        {
            _currentHand.Trump = trump;
            _currentHand.UserCalledTrump = PositionHelper.IsUserTeam(_players[_playerToAct].Position);

            _playerToAct = _dealerIndex.NextPlayer();

            if (_currentHand.IsLoner && _playerToAct == _playerToSkip)
            {
                _playerToAct = _playerToAct.NextPlayer();
                _flippedCard = null;
            }

            _state = State.Playing;
        }

        private void GoAlone()
        {
            _currentHand.IsLoner = true;

            var partnerIndex = PositionHelper.Partner(_playerToAct);
            _playerToSkip = partnerIndex;

            _players[_playerToSkip].Hand = new List<Card>();
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
            var hand = _players[_playerToAct].Hand;

            if (index >= hand.Count)
            {
                return;
            }

            var chosenCard = hand[index];

            if (_leading)
            {
                _currentHand.LeadSuit = CardHelper.IsLeftBower(chosenCard, _currentHand.Trump) ? CardHelper.OppositeSuit(chosenCard.Suit) : chosenCard.Suit;
            }
            else
            {
                var restOfHand = hand.Where(x => x != chosenCard);

                if (!CardHelper.CanPlayCard(chosenCard, restOfHand, _currentHand.Trump, _currentHand.LeadSuit))
                {
                    return;
                }
            }

            _players[_playerToAct].PlayedCard = chosenCard;
            _players[_playerToAct].Hand.RemoveAt(index);

            _playerToAct = _playerToAct.NextPlayer();

            if (_currentHand.IsLoner && _playerToAct == _playerToSkip)
            {
                _playerToAct = _playerToAct.NextPlayer();
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
            _playerToAct = _dealerIndex.NextPlayer();

            if (_currentHand.IsLoner && _playerToAct == _playerToSkip)
            {
                _playerToAct = _playerToAct.NextPlayer();
            }

            _state = State.Playing;
        }

        private void AwardTrick()
        {
            var winningCard = CardHelper.GetWinningCard(_players.Where(x => x.PlayedCard != null).Select(x => x.PlayedCard), _currentHand.Trump, _currentHand.LeadSuit);

            var winner = _players.Single(x => x.PlayedCard == winningCard);
            winner.Tricks++;
            _playerToAct = _players.IndexOf(winner);
        }

        private void UpdateScore()
        {
            var userTricks = _players.Where(x => x.Position == Position.North || x.Position == Position.South).Sum(x => x.Tricks);

            if (userTricks > 2)
            {
                if (_currentHand.IsLoner && userTricks == 5)
                {
                    _score.UserScore += 2;
                }
                if (!_currentHand.UserCalledTrump || userTricks == 5)
                {
                    _score.UserScore += 2;
                }
                else
                {
                    _score.UserScore += 1;
                }
            }
            else
            {
                var opponentTricks = _players.Where(x => x.Position == Position.East || x.Position == Position.West).Sum(x => x.Tricks);

                if (_currentHand.UserCalledTrump || opponentTricks == 5)
                {
                    _score.OpponentScore += 2;
                }
                else
                {
                    _score.OpponentScore += 1;
                }
            }
        }

        //private void HandleClick(Point location)
        //{
        //    var actualPoint = _board.TransformPoint(location);

        //    var rectangles = _board.GetHandDestinations(Position.South);
        //    for (int i = 0; i < _players[2].Hand.Count; i++)
        //    {
        //        if (rectangles[i].Contains(actualPoint))
        //        {
        //            var card = _players[2].Hand[i];
        //            _players[2].Hand.RemoveAt(i);
        //            _players[2].PlayedCard = card;
        //        }
        //    }
        //}
    }
}

