using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace cellution
{
    public class DNAGui
    {
        private int xOffset = 1700;
        private GraphicsDeviceManager graphics;
        public MouseState previousMouseState;
        private Vector2 position;
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                background.position = position;
            }
        }
        private bool doAdd;
        private Sprite background;
        private UpgradeRoomBarGui bars;
        private CellManager cellManager;
        private DNA activeDNA;
        private Cell activeCell;

        private UpgradeRoomBar waitBar;
        private UpgradeRoomBar wanderBar;
        private UpgradeRoomBar attackBar;
        private UpgradeRoomBar speedBar;

        private EatGui eatGui;

        public DNAGui(GraphicsDeviceManager graphics, Texture2D background, CellManager cellManager)
        {
            this.graphics = graphics;
            xOffset = graphics.GraphicsDevice.Viewport.Width - 300;
            this.background = new Sprite(background);
            bars = new UpgradeRoomBarGui(graphics);
            this.background.origin = Vector2.Zero;
            this.cellManager = cellManager;
            this.activeCell = cellManager.selectedCell;
            this.activeDNA = cellManager.selectedCell?.dna;
            Position = this.background.position;
            SetUpBars(graphics);
            eatGui = new EatGui();
            eatGui.Position = new Vector2(200);
        }

        private void SetUpBars(GraphicsDeviceManager graphics)
        {
            waitBar = new UpgradeRoomBar(graphics);
            waitBar.Position = new Vector2(400);
            waitBar.SetTitle("Wait");
            waitBar.SetValue(100);
            waitBar.SetColor(World.Yellow);
            waitBar.SetLetter("C");

            wanderBar = new UpgradeRoomBar(graphics);
            wanderBar.Position = new Vector2(600, 400);
            wanderBar.SetTitle("Wander");
            wanderBar.SetValue(50);
            wanderBar.SetColor(World.Green);
            wanderBar.SetLetter("G");

            attackBar = new UpgradeRoomBar(graphics);
            attackBar.Position = new Vector2(800, 400);
            attackBar.SetTitle("Attack");
            attackBar.SetValue(25);
            attackBar.SetColor(World.Red);
            attackBar.SetLetter("A");

            speedBar = new UpgradeRoomBar(graphics);
            speedBar.Position = new Vector2(1000, 400);
            speedBar.SetTitle("Speed");
            speedBar.SetValue(5);
            speedBar.SetColor(World.Blue);
            speedBar.SetLetter("T");
        }

        private TextItem InitializeItem(Color c)
        {
            TextItem t = new TextItem(World.fontManager["InfoFont"], "0");
            t.color = c;
            return t;
        }

        public void Update(GameTime gameTime)
        {
            MouseState currentState = Mouse.GetState();
            waitBar.Update(gameTime);
            wanderBar.Update(gameTime);
            attackBar.Update(gameTime);
            speedBar.Update(gameTime);
            eatGui.Update(gameTime);
            Position = position;
            previousMouseState = currentState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);
            bars.Draw(spriteBatch);
            waitBar.Draw(spriteBatch);
            wanderBar.Draw(spriteBatch);
            attackBar.Draw(spriteBatch);
            speedBar.Draw(spriteBatch);
            eatGui.Draw(spriteBatch);
        }
    }
}
