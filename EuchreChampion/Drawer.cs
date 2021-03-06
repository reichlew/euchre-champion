﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EuchreChampion
{
    public class Drawer
    {
        private SpriteBatch _spriteBatch;
        private Board _board;

        private SpriteFont _font;

        public Drawer(SpriteBatch spriteBatch, Board board, SpriteFont font)
        {
            _spriteBatch = spriteBatch;
            _board = board;

            _font = font;
        }

        public void DrawScore(Score score)
        {
            DrawText($"USER SCORE: {score.UserScore}", _board.ScorePosition, Color.White);
            DrawText($"OPPONENT SCORE: {score.OpponentScore}", new Vector2(_board.ScorePosition.X, _board.ScorePosition.Y + 14.0f), Color.White);
        }

        public void DrawFlippedCard(Card flippedCard)
        {
            DrawCard(flippedCard.Front, _board.ScreenCenter, 0.0f);
        }

        public void DrawDealtCard(Player player)
        {
            if (player.DealtCard != null)
            {
                var position = _board.GetDealtCardDestination(player.Position);
                var rotation = _board.GetRotation(player.Position);

                var card = player.DealtCard;

                DrawCard(card.Front, position, rotation);
            }
        }

        public void DrawHand(Player player)
        {
            var positions = _board.GetHandDestinations(player.Position);
            var rotation = _board.GetRotation(player.Position);

            for (int i = 0; i < player.Hand.Count; i++)
            {
                var card = player.Hand[i];

                DrawCard(card.Front, positions[i], rotation);
            }

            if (player.PlayedCard != null)
            {
                var position = _board.GetDealtCardDestination(player.Position);

                DrawCard(player.PlayedCard.Front, position, rotation);
            }
        }

        private void DrawCard(Texture2D texture, Rectangle destination, float rotation)
        {
            _spriteBatch.Draw(texture, destination, null, Color.White, rotation, _board.CardCenter, SpriteEffects.None, 0.0f);
        }

        public void DrawText(string text, Vector2 location, Color color)
        {
            _spriteBatch.DrawString(_font, text, location, color);
        }

        public void DrawDebugInfo(InputManager manager, State state, int dealer, Suit? trump, int playerToAct)
        {
            DrawText(manager.PressedKeys(), new Vector2(0.0f, 0.0f), Color.White);
            DrawText($"STATE: {state}", new Vector2(0.0f, 14.0f), Color.White);
            DrawText($"DEALER : {(Position)dealer}", new Vector2(0.0f, 28.0f), Color.White);
            DrawText($"TRUMP: {trump}", new Vector2(0.0f, 42.0f), Color.White);
            DrawText($"PLAYER TO ACT: {(Position)playerToAct}", new Vector2(0.0f, 56.0f), Color.White);
        }
    }
}