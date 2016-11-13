using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace cellution
{
    class DNAGui
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
                addButton.position = position + new Vector2(xOffset - addButton.TextSize.X - 10, 30);
                BVs[0].PositionBelow(addButton);
                BVs[1].PositionBelow(BVs[0]);
                BVs[2].PositionBelow(BVs[1]);
                BVs[3].PositionBelow(BVs[2]);
                BVs[4].PositionBelow(BVs[3]);
                BVs[5].PositionBelow(BVs[4]);
                BVs[6].PositionBelow(BVs[5]);
                BVs[7].PositionBelow(BVs[6]);

            }
        }
        private bool doAdd;
        private Sprite background;
        private List<TextItem> BVs;
        private TextItem addButton;
        private CellManager cellManager;
        private DNA activeDNA;
        private Cell activeCell;

        public DNAGui(GraphicsDeviceManager graphics, Texture2D background, CellManager cellManager)
        {
            this.graphics = graphics;
            xOffset = graphics.GraphicsDevice.Viewport.Width - 300;
            BVs = new List<TextItem>();
            this.background = new Sprite(background);
            this.background.origin = Vector2.Zero;
            BVs.Add(InitializeItem(new Color(248, 215, 241)));
            BVs.Add(InitializeItem(new Color(245, 248, 195)));
            BVs.Add(InitializeItem(new Color(193, 250, 196)));
            BVs.Add(InitializeItem(new Color(248, 215, 241)));
            BVs.Add(InitializeItem(new Color(245, 248, 195)));
            BVs.Add(InitializeItem(new Color(193, 250, 196)));
            BVs.Add(InitializeItem(new Color(248, 215, 241)));
            BVs.Add(InitializeItem(new Color(193, 250, 196)));
            addButton = InitializeItem(new Color(World.Random.Next(0, 255),
                World.Random.Next(0, 255),
                World.Random.Next(0, 255)));
            this.cellManager = cellManager;
            this.activeCell = cellManager.selectedCell;
            this.activeDNA = cellManager.selectedCell?.dna;
            Position = this.background.position;
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
            if (cellManager.selectedCell == null)
            {
                foreach (var item in BVs)
                {
                    item.Text = "N/A";
                }
            }
            else
            {
                BVs[0].Text = "EatA: " + Math.Round(activeDNA.genes[0].Item2, 3) * 100 + "%";
                BVs[1].Text = "EatC: " + Math.Round(activeDNA.genes[1].Item2, 3) * 100 + "%";
                BVs[2].Text = "EatT: " + Math.Round(activeDNA.genes[2].Item2, 3) * 100 + "%";
                BVs[3].Text = "EatG: " + Math.Round(activeDNA.genes[3].Item2, 3) * 100 + "%";
                BVs[4].Text = "Divide: " + Math.Round(activeDNA.genes[4].Item2, 3) * 100 + "%";
                BVs[5].Text = "Wander: " + Math.Round(activeDNA.genes[5].Item2, 3) * 100 + "%";
                BVs[6].Text = "Attack: " + Math.Round(activeDNA.genes[6].Item2, 3) * 100 + "%";
                BVs[7].Text = "Wait: " + Math.Round(activeDNA.genes[7].Item2, 3) * 100 + "%";
            }
            addButton.Text = "Add Probability";
            Position = position;
            addButton.Update();
            foreach (var item in BVs)
            {
                item.Update();
            }

            if (currentState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released && cellManager.selectedCell != null)
            {
                
                Vector2 transformedMouseState = Vector2.Transform(currentState.Position.ToVector2(), Game1.world.rooms.GetState("upgrade").cameras.CurrentState.InverseTransform);
                Console.WriteLine("Wednesday: " + transformedMouseState);
                Console.WriteLine("Weekdays" + addButton.position);
                Console.WriteLine("Thursday: " + BVs[0].rectangle);
                if (addButton.rectangle.Contains(transformedMouseState))
                {
                    
                    Console.WriteLine("suh ");
                    doAdd = true;
                }
                else
                {
                    for (int i = 0; i < BVs.Count; i++)
                    {
                        if (BVs[i].rectangle.Contains(transformedMouseState) && doAdd)
                        {
                            activeDNA.InfluenceGene(i, 5);
                            doAdd = false;
                            break;
                        }
                    }
                }
            }
            previousMouseState = currentState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);
            foreach (var item in BVs)
            {
                item.Draw(spriteBatch);
            }
            addButton.Draw(spriteBatch);
        }
    }
}
