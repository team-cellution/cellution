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

        public KeyboardState previousKeyboardState;
        public MouseState previousMouseState;

        const string UpgradeRoom = "upgrade";

        HighlightRing highlightRing;

        StatsGUI statsGUI;
        Background background;
        DNAGui dnaGui;

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
            World.textureManager = new ContentManager<Texture2D>(Content);
            World.fontManager = new ContentManager<SpriteFont>(Content);
            world.rooms.AddState(UpgradeRoom, new Room(graphics));

            world.resourceManager = new ResourceManager(graphics.GraphicsDevice.Viewport);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            World.textureManager.Load("Cell");
            World.textureManager.Load("new_cell");
            World.textureManager.Load("fancy_cell_greyscale");
            World.textureManager.Load("a");
            World.textureManager.Load("c");
            World.textureManager.Load("g");
            World.textureManager.Load("t");
            World.textureManager.Load("BG-Layer");
            World.textureManager.Load("helix-resource");
            World.textureManager.Load("Cell-Division");
            World.textureManager.Load("highlight_ring");

            World.fontManager.Load("ScoreFont");
            World.fontManager.Load("InfoFont");

            // create 1x1 texture for line drawing
            world.oneByOne = new Texture2D(GraphicsDevice, 1, 1);
            world.oneByOne.SetData<Color>(
                new Color[] { Color.White });// fill the texture with white

            highlightRing = new HighlightRing(World.textureManager["highlight_ring"]);

            background = new Background(World.textureManager["BG-Layer"], graphics.GraphicsDevice.Viewport);
            world.cellManager = new CellManager(World.textureManager["fancy_cell_greyscale"], graphics);
            world.cellManager.SpawnCell();

            dnaGui = new DNAGui(graphics, World.textureManager["helix-resource"], world.cellManager);

            statsGUI = new StatsGUI(World.textureManager["helix-resource"], world.cellManager);

            world.rooms.CurrentState.AddUpdate(world.resourceManager.Update);
            world.rooms.CurrentState.AddUpdate(statsGUI.Update);
            world.rooms.CurrentState.AddDraw(background.Draw);
            world.rooms.CurrentState.AddDraw(world.resourceManager.Draw);
            world.rooms.CurrentState.AddUpdate(world.cellManager.Update);
            world.rooms.CurrentState.AddUpdate(highlightRing.Update);
            world.rooms.CurrentState.AddDraw(world.cellManager.Draw);
            world.rooms.CurrentState.AddDraw(highlightRing.Draw);
            world.rooms.CurrentState.AddDraw(statsGUI.Draw);
            world.rooms.GetState(UpgradeRoom).AddDraw(world.cellManager.DrawSelected);
            world.rooms.GetState(UpgradeRoom).AddDraw(highlightRing.Draw);
            world.rooms.GetState(UpgradeRoom).AddDraw(statsGUI.Draw);
            world.rooms.GetState(UpgradeRoom).AddUpdate(statsGUI.Update);
            world.rooms.GetState(UpgradeRoom).AddDraw(dnaGui.Draw);
            world.rooms.GetState(UpgradeRoom).AddUpdate(dnaGui.Update);
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

            // press esc to exit
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // left click on a player cell to select it
            if (mouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released)
            {
                bool playerHasACell = false;

                if (world.rooms.GetState(UpgradeRoom) != world.rooms.CurrentState)
                {
                    world.cellManager.selectedCell = null;
                    highlightRing.SetHighlightCell(null);
                }
                

                Vector2 transformedMouseState = Vector2.Transform(mouseState.Position.ToVector2(), world.rooms.CurrentState.cameras.CurrentState.InverseTransform);

                foreach (Cell cell in world.cellManager.cells)
                {
                    cell.selected = false;
                    if (cell.sprite.color == world.cellManager.playerColor)
                    {
                        if (cell.sprite.rectangle.Contains(transformedMouseState))
                        {
                            cell.selected = true;
                            world.cellManager.selectedCell = cell;
                            highlightRing.SetHighlightCell(cell);
                        }
                        playerHasACell = true;
                    }
                }
                if (!playerHasACell)
                {
                    world.cellManager.cells.Add(world.cellManager.CreatePlayerCell(transformedMouseState));
                }
            }

            // if a cell is selected, right click to move it to the mouse's position
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                foreach (Cell cell in world.cellManager.cells)
                {
                    Vector2 transformedMouseState = Vector2.Transform(mouseState.Position.ToVector2(), world.rooms.CurrentState.cameras.CurrentState.InverseTransform);
                    if (cell == world.cellManager.selectedCell)
                    {
                        cell.GoTo(new Vector2(transformedMouseState.X, transformedMouseState.Y));
                        cell.behavior = -2;
                    }
                }
            }

            // space bar toggles upgrade room
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

            // Press 'D' to try to divide your selected cell
            if (world.cellManager.selectedCell != null  && keyboardState.IsKeyDown(Keys.D) &&
                previousKeyboardState.IsKeyUp(Keys.D))
            {
                Cell selectedCell = world.cellManager.selectedCell;
                if (selectedCell.a >= 10 && selectedCell.c >= 10 && selectedCell.g >= 10 && selectedCell.t >= 10)
                {
                    selectedCell.divide = true;
                }
            }

            // Press 'A' to try to have the selected cell attack the nearest enemy cell
            if (world.cellManager.selectedCell != null && keyboardState.IsKeyDown(Keys.A) &&
                previousKeyboardState.IsKeyUp(Keys.A))
            {
                Cell selectedCell = world.cellManager.selectedCell;
                selectedCell.behavior = 6;
            }

            // Press 'W' to try to have the selected cell wander
            if (world.cellManager.selectedCell != null && keyboardState.IsKeyDown(Keys.W) &&
                previousKeyboardState.IsKeyUp(Keys.W))
            {
                Cell selectedCell = world.cellManager.selectedCell;
                selectedCell.Wander();
                selectedCell.behavior = -3;
            }

            // Press 'S' to try to toggle the selected cell's auto behavior
            if (world.cellManager.selectedCell != null && keyboardState.IsKeyDown(Keys.S) &&
                previousKeyboardState.IsKeyUp(Keys.S))
            {
                Cell selectedCell = world.cellManager.selectedCell;
                if (selectedCell.autoSelected == false)
                {
                    selectedCell.behavior = -1;
                    selectedCell.autoSelected = true;
                }
                else
                {
                    selectedCell.autoSelected = false;
                }
            }

            // Press Arrows to move the selected cell
            if (world.cellManager.selectedCell != null && (keyboardState.IsKeyDown(Keys.Up) ||
                keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.Right) || 
                keyboardState.IsKeyDown(Keys.Left)))
            {
                Cell selectedCell = world.cellManager.selectedCell;
                selectedCell.behavior = -3;
                Vector2 newTarget = selectedCell.sprite.position;
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    newTarget.Y -= selectedCell.sprite.tex.Width * selectedCell.sprite.scale / 2 + 5;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    newTarget.Y += selectedCell.sprite.tex.Width * selectedCell.sprite.scale / 2 + 5;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    newTarget.X += selectedCell.sprite.tex.Width * selectedCell.sprite.scale / 2 + 5;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    newTarget.X -= selectedCell.sprite.tex.Width * selectedCell.sprite.scale / 2 + 5;
                }
                selectedCell.GoTo(newTarget);
            }

            world.Update(gameTime);

            List<Resource> resourcesToRemove = new List<Resource>();
            foreach (Resource resource in world.resourceManager.resources)
            {
                foreach (Cell cell in world.cellManager.cells)
                {
                    if (!resourcesToRemove.Contains(resource))
                    {
                        if (cell.sprite.rectangle.Contains(resource.sprite.rectangle))
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
                        }
                    }
                }
            }
            foreach (Resource resource in resourcesToRemove)
            {
                world.resourceManager.resources.Remove(resource);
                world.resourceManager.currentResources--;
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
            if (world.rooms.CurrentName == UpgradeRoom)
            {
                GraphicsDevice.Clear(Color.Black);
            }
            else
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
            }

            world.BeginDraw();
            world.Draw();
            world.EndDraw();

            base.Draw(gameTime);
        }
    }
}
