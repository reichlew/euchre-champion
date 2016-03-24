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

        private Texture2D title;
        private List<SpriteFont> fonts;

        public Drawer(SpriteBatch spriteBatch, Board board, Texture2D title, List<SpriteFont> fonts)
        {
            _spriteBatch = spriteBatch;
            _board = board;
            this.title = title;
            this.fonts = fonts;
        }

        public void DrawFlippedCard(Card flippedCard)
        {
            DrawCard(flippedCard.Front, _board.screenCenter, 0.0f);
        }

        public void DrawDealtCard(Player player)
        {
            if (player.DealtCard != null)
            {
                var position = _board.GetDestinationRectangle(player.Position);
                var rotation = _board.GetRotation(player.Position);

                var card = player.DealtCard;

                DrawCard(card.ActiveTexture, position, rotation);
            }
        }

        public void DrawHand(Player player)
        {
            var positions = _board.GetDestinationRectangles(player.Position);
            var rotation = _board.GetRotation(player.Position);

            for (int i = 0; i < player.Hand.Count; i++)
            {
                var card = player.Hand[i];

                DrawCard(card.ActiveTexture, positions[i], rotation);
            }

            if (player.PlayedCard != null)
            {
                var position = _board.GetDestinationRectangle(player.Position);

                DrawCard(player.PlayedCard.ActiveTexture, position, rotation);
            }
        }

        private void DrawCard(Texture2D texture, Rectangle destination, float rotation)
        {
            _spriteBatch.Draw(texture, destination, null, Color.White, rotation, _board.cardCenter, SpriteEffects.None, 0.0f);
        }

        public void DrawScore(Score _score)
        {
            //throw new NotImplementedException();
        }

        public void DrawOrderUpDecision()
        {
            _spriteBatch.Draw(title, _board.GetOrderUpDecision(), Color.White);
        }

        public void DrawPickUpDecision()
        {
        }

        public void DrawText(string font, string text, Vector2 location, Color color)
        {
            _spriteBatch.DrawString(fonts.Last(), text, location, color);
        }

        public void DrawDebugInfo(InputManager manager)
        {
            DrawText(string.Empty, manager.PressedKeys(), new Vector2(0.0f, 0.0f), Color.White);
        }
    }
}