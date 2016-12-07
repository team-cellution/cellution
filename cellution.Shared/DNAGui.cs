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
            eatGui = new EatGui();
            eatGui.Position = new Vector2(307, 24);
        }

        public void SetActiveCell(Cell cell)
        {
            if (cell != null) { 
                activeCell = cell;
                activeDNA = cell.dna;
                bars.UpdateDnaValues(activeDNA);
             }
        }

        public void Update(GameTime gameTime)
        {
            MouseState currentState = Mouse.GetState();
            bars.Update(gameTime);
            eatGui.Update(gameTime);
            Position = position;
            previousMouseState = currentState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);
            bars.Draw(spriteBatch);
            eatGui.Draw(spriteBatch);
        }
    }
}
