using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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

        const string UpgradeRoom = "upgrade";

        Cell cell1;
        Cell cell2;

        SpriteFont scoreFont;
        StatsGUI statsGUI;
        Background background;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

#if __MOBILE__
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
#elif WINDOWS
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            IsMouseVisible = true;
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
            world.rooms.AddState(UpgradeRoom, new Room(graphics));

            resourceManager = new ResourceManager(graphics.GraphicsDevice.Viewport);

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
            world.textureManager.Load("BG-Layer");
            world.textureManager.Load("helix-resource");
            scoreFont = Content.Load<SpriteFont>("ScoreFont");

            background = new Background(world.textureManager["BG-Layer"], graphics.GraphicsDevice.Viewport);
            cell1 = new Cell(world.textureManager["Cell"], 0, 0);
            cell2 = new Cell(world.textureManager["Cell"], 100, 100);
            cell1.name = "one";
            cell2.name = "two";
            world.cells.Add(cell1);
            world.cells.Add(cell2);
            statsGUI = new StatsGUI(world.textureManager["helix-resource"], scoreFont, cell1);

            world.rooms.CurrentState.AddUpdate(resourceManager.Update);
            world.rooms.CurrentState.AddUpdate(statsGUI.Update);
            world.rooms.CurrentState.AddDraw(background.Draw);
            world.rooms.CurrentState.AddDraw(resourceManager.Draw);
            foreach (Cell cell in world.cells)
            {
                world.rooms.CurrentState.AddUpdate(cell.Update);
                world.rooms.CurrentState.AddDraw(cell.Draw);
            }
            world.rooms.CurrentState.AddDraw(statsGUI.Draw);

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

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (mouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released)
            {
                foreach (Cell cell in world.cells)
                {
                    Vector2 transformedMouseState = Vector2.Transform(mouseState.Position.ToVector2(), world.rooms.CurrentState.cameras.CurrentState.InverseTransform);
                    if (cell.rectange.Contains(transformedMouseState.ToPoint()))
                    {
                        world.selectedId = cell.id;
                    }
                }
            }

            if (mouseState.RightButton == ButtonState.Pressed &&
                previousMouseState.RightButton == ButtonState.Released)
            {
                foreach (Cell cell in world.cells)
                {
                    Vector2 transformedMouseState = Vector2.Transform(mouseState.Position.ToVector2(), world.rooms.CurrentState.cameras.CurrentState.InverseTransform);
                    if (cell.id == world.selectedId)
                    {
                        cell.targetPosition = new Vector2(transformedMouseState.X, transformedMouseState.Y);
                        cell.velocity = new Vector2(cell.targetPosition.X - cell.position.X, cell.targetPosition.Y - cell.position.Y);
                        cell.velocity.Normalize();
                        cell.velocity *= 5.0f;
                    }
                }
            }

            if (keyboardState.IsKeyDown(Keys.Space) &&
                previousKeyboardState.IsKeyUp(Keys.Space))
            {
                if (world.rooms.CurrentName == "game")
                {
                    world.rooms.CurrentName = UpgradeRoom;
                }
                else if (world.rooms.CurrentName == UpgradeRoom)
                {
                    world.rooms.CurrentName = "game";
                }
            }

            world.Update();

            List<Resource> resourcesToRemove = new List<Resource>();
            foreach (Resource resource in resourceManager.resources)
            {
                foreach (Cell cell in world.cells)
                {
                    if (cell.rectange.Contains(resource.sprite.rectange))
                    {
                        if (resource.resourceType == Resource.ResourceTypes.A)
                        {
                            cell.a++;
                        }
                        else if (resource.resourceType == Resource.ResourceTypes.C)
                        {
                            cell.c++;
                        }
                        else if (resource.resourceType == Resource.ResourceTypes.G)
                        {
                            cell.g++;
                        }
                        else if (resource.resourceType == Resource.ResourceTypes.T)
                        {
                            cell.t++;
                        }
                        resourcesToRemove.Add(resource);
                        resourceManager.currentResources--;
                    }
                }
            }
            foreach (Resource resource in resourcesToRemove)
            {
                resourceManager.resources.Remove(resource);
            }
            resourcesToRemove.Clear();

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
            world.Draw();
            world.EndDraw();

            base.Draw(gameTime);
        }
    }
}
