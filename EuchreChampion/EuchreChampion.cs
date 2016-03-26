using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EuchreChampion
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class EuchreChampion : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private GameManager _game;
        private ContentLoader _contentLoader;
        private PlayerFactory _playerFactory;
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

            _contentLoader = new ContentLoader(Content);
            _playerFactory = new PlayerFactory();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice); 

            var drawer = new Drawer(_spriteBatch, new Board(_graphics.GraphicsDevice.Viewport), _contentLoader.LoadFont());
            
            var players = _playerFactory.GetPlayers();
            
            var dealer = new Dealer(_contentLoader.LoadCards(), players, DealType.TwoThree);

            _game = new GameManager(players, dealer, drawer, _inputManager);  
            
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

            _spriteBatch.Begin();

            _game.Draw();

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
