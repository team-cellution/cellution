using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace cellution
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        public static World world;
        ResourceManager resourceManager;

        public KeyboardState previousKeyboardState;
        public MouseState previousMouseState;

        Cell cell;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

#if __MOBILE__
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
#else
            this.IsMouseVisible = true;
#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            world = new World(graphics);
            world.textureManager = new TextureManager(Content);

            resourceManager = new ResourceManager();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            world.textureManager.Load("Cell");
            world.textureManager.Load("a");
            world.textureManager.Load("c");
            world.textureManager.Load("g");
            world.textureManager.Load("t");
            cell = new Cell(world.textureManager["Cell"]);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released)
            {
                Vector2 transformedMouseState = Vector2.Transform(mouseState.Position.ToVector2(), world.cameras[world.CurrentCamera].Transform);
                cell.targetPosition = new Vector2(transformedMouseState.X, transformedMouseState.Y);
                cell.velocity = new Vector2(cell.targetPosition.X - cell.position.X, cell.targetPosition.Y - cell.position.Y);
                cell.velocity.Normalize();
                cell.velocity *= 5.0f;
            }

            cell.Update();
            resourceManager.Update();

            previousKeyboardState = keyboardState;
            previousMouseState = mouseState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            world.BeginDraw();
            world.Draw(resourceManager.Draw);
            world.Draw(cell.Draw);
            world.EndDraw();

            base.Draw(gameTime);
        }
    }
}
