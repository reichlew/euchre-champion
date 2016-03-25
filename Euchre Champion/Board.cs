using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EuchreChampion
{
    public class Board
    {
        private Viewport _viewport { get; set; }

        private int _cardWidth { get { return 768; } }
        private int _cardHeight { get { return 1063; } }

        private int _scaledCardWidth { get { return (int)(_cardWidth * Scale); } }
        private int _scaledCardHeight { get { return (int)(_cardHeight * Scale); } }

        public Rectangle ScreenCenter { get; private set; }
        public Vector2 CardCenter { get; private set; }
        public Vector2 ScorePosition { get; private set; }

        public float Scale { get { return _viewport.Width / _cardWidth / 10.0f; } }

        public Board(Viewport viewport)
        {
            _viewport = viewport;

            CardCenter = new Vector2(_cardWidth / 2.0f, _cardHeight / 2.0f);
            ScreenCenter = GetCardRectangle(_viewport.Width / 2.0f, _viewport.Height / 2.0f);
            ScorePosition = new Vector2(_viewport.Width - 200.0f, 0.0f);
        }

        public Point TransformPoint(Point location)
        {
            var x = location.X + _scaledCardWidth / 2;
            var y = location.Y + _scaledCardHeight / 2;

            return new Point(x, y);
        }

        public float GetRotation(Position position)
        {
            return (position == Position.East || position == Position.West) ? (float)Math.PI / 2.0f : 0f;
        }        

        public Rectangle GetDealtCardDestination(Position position)
        {
            switch (position)
            {
                case Position.North:
                    {
                        return GetCardRectangle(_viewport.Width / 2.0f, _viewport.Height / 2.0f - _cardHeight * Scale);
                    }
                case Position.East:
                    {
                        return GetCardRectangle(_viewport.Width / 2.0f + _cardHeight * Scale, _viewport.Height / 2.0f);
                    }
                case Position.South:
                    {
                        return GetCardRectangle(_viewport.Width / 2.0f, _viewport.Height / 2.0f + _cardHeight * Scale);
                    }
                case Position.West:
                    {
                        return GetCardRectangle(_viewport.Width / 2.0f - _cardHeight * Scale, _viewport.Height / 2.0f);
                    }
                default:
                    throw new Exception($"{position} is not a valid Position.");
            }
        }

        public List<Rectangle> GetHandDestinations(Position position)
        {
            var rectangles = new List<Rectangle>();

            switch (position)
            {
                case Position.North:
                    {
                        var yPosition = _cardHeight * Scale / 2.0f;
                        var centerX = _viewport.Width / 2.0f;
                        rectangles.Add(GetCardRectangle(centerX - _cardWidth * Scale * 2.0f, yPosition));
                        rectangles.Add(GetCardRectangle(centerX - _cardWidth * Scale, yPosition));
                        rectangles.Add(GetCardRectangle(centerX, yPosition));
                        rectangles.Add(GetCardRectangle(centerX + _cardWidth * Scale, yPosition));
                        rectangles.Add(GetCardRectangle(centerX + _cardWidth * Scale * 2.0f, yPosition));
                        break;
                    }
                case Position.East:
                    {
                        var xPosition = _viewport.Width - _cardHeight * Scale / 2.0f;
                        var centerY = _viewport.Height / 2.0f;
                        rectangles.Add(GetCardRectangle(xPosition, centerY - _cardWidth * Scale * 2.0f));
                        rectangles.Add(GetCardRectangle(xPosition, centerY - _cardWidth * Scale));
                        rectangles.Add(GetCardRectangle(xPosition, centerY));
                        rectangles.Add(GetCardRectangle(xPosition, centerY + _cardWidth * Scale));
                        rectangles.Add(GetCardRectangle(xPosition, centerY + _cardWidth * Scale * 2.0f));
                        break;
                    }
                case Position.South:
                    {
                        var yPosition = _viewport.Height - _cardHeight * Scale / 2.0f;
                        var centerX = _viewport.Width / 2.0f;
                        rectangles.Add(GetCardRectangle(centerX - _cardWidth * Scale * 2.0f, yPosition));
                        rectangles.Add(GetCardRectangle(centerX - _cardWidth * Scale, yPosition));
                        rectangles.Add(GetCardRectangle(centerX, yPosition));
                        rectangles.Add(GetCardRectangle(centerX + _cardWidth * Scale, yPosition));
                        rectangles.Add(GetCardRectangle(centerX + _cardWidth * Scale * 2.0f, yPosition));
                        break;
                    }
                case Position.West:
                    {
                        var xPosition = _cardHeight * Scale / 2.0f;
                        var centerY = _viewport.Height / 2.0f;
                        rectangles.Add(GetCardRectangle(xPosition, centerY + _cardWidth * Scale * 2.0f));
                        rectangles.Add(GetCardRectangle(xPosition, centerY + _cardWidth * Scale));
                        rectangles.Add(GetCardRectangle(xPosition, centerY));
                        rectangles.Add(GetCardRectangle(xPosition, centerY - _cardWidth * Scale));
                        rectangles.Add(GetCardRectangle(xPosition, centerY - _cardWidth * Scale * 2.0f));
                        break;
                    }
                default:
                    throw new Exception($"{position} is not a valid Position.");
            }

            return rectangles;
        }        

        private Rectangle GetCardRectangle(float x, float y)
        {
            return new Rectangle((int)x, (int)y, _scaledCardWidth, _scaledCardHeight);
        }
    }
}