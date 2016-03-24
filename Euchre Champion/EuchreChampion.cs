using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Microsoft.Xna.Framework.Input.Touch;

namespace EuchreChampion
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class EuchreChampion : Game
    {
        private GraphicsDeviceManager _graphics;

        private GameManager _game;
        private InputManager _inputManager;

        public EuchreChampion()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = (int)(1920 * .85);
            _graphics.PreferredBackBufferHeight = (int)(1080 * .85);

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;

            _inputManager = new InputManager();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            var spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);            

            var loader = new ContentLoader(Content);
            var cards = loader.LoadCards();

            var factory = new PlayerFactory();
            var players = factory.GetPlayers();

            var board = new Board(_graphics.GraphicsDevice.Viewport);

            var title = loader.LoadTitle();
            var fonts = loader.LoadFonts();

            _game = new GameManager(spriteBatch, cards, players, board, title, fonts, _inputManager);     
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (_inputManager.IsKeyPressed(Keys.Escape))
            {
                Exit();
            }

            _game.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGreen);

            _game.Draw();            

            base.Draw(gameTime);
        }
    }
}
