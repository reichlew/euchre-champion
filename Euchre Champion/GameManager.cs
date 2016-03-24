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

        private Score _score { get; set; }
        private DealType _dealType { get; set; }        
        private State _state { get; set; }
        private Suit Trump { get; set; }
        private Card _flippedCard { get; set; }

        public GameManager(SpriteBatch spriteBatch, List<Card> cards, List<Player> players, Board board, Texture2D title, List<SpriteFont> fonts, InputManager inputManager)
        {
            _spriteBatch = spriteBatch;
            _cards = cards;
            _players = players;
            _board = board;
            _inputManager = inputManager;

            _drawer = new Drawer(_spriteBatch, _board, title, fonts);

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
                        if (_inputManager.IsKeyPressed(Keys.Enter))
                        {
                            GetDealer();
                        }
                        break;
                    }
                case State.DealerFound:
                case State.Dealing:
                    {
                        if (_inputManager.IsKeyPressed(Keys.Enter))
                        {
                            DealNext();
                        }
                        break;
                    }
                case State.Dealt:
                    {
                        if (_inputManager.IsKeyPressed(Keys.Enter))
                        {
                            FlipCard();
                        }
                        break;
                    }
                case State.CallingTrump:
                    {
                        if (_inputManager.IsMouseClicked())
                        {
                            HandleClick(_inputManager.GetClickedLocation());
                        }
                        break;
                    }
            }
        }

        public void Draw()
        {
            _spriteBatch.Begin();

            if (SHOW_DEBUG_CONTROLS)
            {
                _drawer.DrawDebugInfo(_inputManager);
            }

            _drawer.DrawScore(_score);



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

            if (_state == State.CallingTrump)
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

            var rectangles = _board.GetDestinationRectangles(Position.South);
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

