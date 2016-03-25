using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace EuchreChampion
{
    public class InputManager
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private TouchCollection _currentTouchState;
        private TouchCollection _previousTouchState;

        public void Update(KeyboardState keyboardState, MouseState mouseState, TouchCollection touchState)
        {
            _previousKeyboardState = _currentKeyboardState;
            _previousMouseState = _currentMouseState;
            _previousTouchState = _currentTouchState;

            _currentKeyboardState = keyboardState;
            _currentMouseState = mouseState;
            _currentTouchState = touchState;
        }

        public bool IsKeyPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }

        public bool IsMouseClicked()
        {
            return _currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
        }

        public Point GetClickedLocation()
        {
            return new Point(_currentMouseState.X, _currentMouseState.Y);
        }

        public string PressedKeys()
        {
            var keyString = string.Empty;

            var pressedKeys = _currentKeyboardState.GetPressedKeys();                        
            if (pressedKeys.Any())
            {
                keyString =  String.Join(" + ", pressedKeys);
            }

            return keyString;
        }
    }
}