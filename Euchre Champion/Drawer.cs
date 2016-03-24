using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
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

        public void DrawFlippedCard(Card flippedCard)
        {
            DrawCard(flippedCard.Front, _board.screenCenter, 0.0f);
        }

        public void DrawDealtCard(Player player)
        {
            if (player.DealtCard != null)
            {
                var position = _board.GetDealtPlayedDestination(player.Position);
                var rotation = _board.GetRotation(player.Position);

                var card = player.DealtCard;

                DrawCard(card.ActiveTexture, position, rotation);
            }
        }

        public void DrawHand(Player player)
        {
            var positions = _board.GetHandDestinations(player.Position);
            var rotation = _board.GetRotation(player.Position);

            for (int i = 0; i < player.Hand.Count; i++)
            {
                var card = player.Hand[i];

                DrawCard(card.ActiveTexture, positions[i], rotation);
            }

            if (player.PlayedCard != null)
            {
                var position = _board.GetDealtPlayedDestination(player.Position);

                DrawCard(player.PlayedCard.ActiveTexture, position, rotation);
            }
        }

        private void DrawCard(Texture2D texture, Rectangle destination, float rotation)
        {
            _spriteBatch.Draw(texture, destination, null, Color.White, rotation, _board.cardCenter, SpriteEffects.None, 0.0f);
        }

        public void DrawText(string text, Vector2 location, Color color)
        {
            _spriteBatch.DrawString(_font, text, location, color);
        }

        public void DrawDebugInfo(InputManager manager, State state, int dealer, Suit trump, int playerToAct)
        {
            DrawText(manager.PressedKeys(), new Vector2(0.0f, 0.0f), Color.White);
            DrawText($"STATE: {state}", new Vector2(0.0f, 14.0f), Color.White);
            DrawText($"DEALER : {(Position)dealer}", new Vector2(0.0f, 28.0f), Color.White);
            DrawText($"TRUMP: {trump}", new Vector2(0.0f, 42.0f), Color.White);
            DrawText($"PLAYER TO ACT: {(Position)playerToAct}", new Vector2(0.0f, 56.0f), Color.White);
        }
    }
}