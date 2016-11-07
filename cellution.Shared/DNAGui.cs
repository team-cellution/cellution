using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace cellution
{
    class DNAGui
    {
        private const float xOffset = 1700;
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
                BV1.position = position + new Vector2(xOffset, 30);
                BV2.position = position + new Vector2(xOffset, BV1.position.Y + BV1.TextSize.Y + 10);
                BV3.position = position + new Vector2(xOffset, BV2.position.Y + BV2.TextSize.Y + 10);
                BV4.position = position + new Vector2(xOffset, BV3.position.Y + BV3.TextSize.Y + 10);
                BV5.position = position + new Vector2(xOffset, BV4.position.Y + BV4.TextSize.Y + 10);
                BV6.position = position + new Vector2(xOffset, BV5.position.Y + BV5.TextSize.Y + 10);
                BV7.position = position + new Vector2(xOffset, BV6.position.Y + BV6.TextSize.Y + 10);
                BV8.position = position + new Vector2(xOffset, BV7.position.Y + BV7.TextSize.Y + 10);

            }
        }
        private bool doAdd;
        private Sprite background;
        private TextItem BV1;
        private TextItem BV2;
        private TextItem BV3;
        private TextItem BV4;
        private TextItem BV5;
        private TextItem BV6;
        private TextItem BV7;
        private TextItem BV8;
        private TextItem addButton;
        private CellManager cellManager;
        private DNA activeDNA;
        private Cell activeCell;

        private TextItem initialiseItem(TextItem t, Color c, SpriteFont spriteFont) {
            t = new TextItem(spriteFont, "0");
            //t.origin = Vector2.Zero;
            t.origin = new Vector2(t.TextSize.X / 2, t.TextSize.Y / 2);
            t.color = c;
            return t;
        }

        public DNAGui(Texture2D background, SpriteFont spriteFont, CellManager cellManager)
        {
            Random r = new Random();
            this.background = new Sprite(background);
            this.background.origin = Vector2.Zero;
            BV1 = initialiseItem(BV1, new Color(248, 215, 241), spriteFont);
            BV2 = initialiseItem(BV2, new Color(245, 248, 195), spriteFont);
            BV3 = initialiseItem(BV3, new Color(193, 250, 196), spriteFont);
            BV4 = initialiseItem(BV4, new Color(248, 215, 241), spriteFont);
            BV5 = initialiseItem(BV5, new Color(245, 248, 195), spriteFont);
            BV6 = initialiseItem(BV6, new Color(193, 250, 196), spriteFont);
            BV7 = initialiseItem(BV7, new Color(248, 215, 241), spriteFont);
            BV8 = initialiseItem(BV8, new Color(193, 250, 196), spriteFont);
            addButton = initialiseItem(addButton, new Color(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255)), spriteFont);
            this.cellManager = cellManager;
            this.activeCell = cellManager.selectedCell;
            this.activeDNA = cellManager.selectedCell?.dna;
            Position = this.background.position;
        }

        public void Update(GameTime gameTime)
        {
            MouseState currentState = Mouse.GetState();
            if (cellManager.selectedCell == null)
            {
                BV1.Text = "N/A";
                BV2.Text = "N/A";
                BV3.Text = "N/A";
                BV4.Text = "N/A";
                BV5.Text = "N/A";
                BV6.Text = "N/A";
                BV7.Text = "N/A";
                BV8.Text = "N/A";
            }
            else
            {
                BV1.Text = "EatA: " + Math.Round(activeDNA.genes[0].Item2, 3) * 100 + "%";
                BV2.Text = "EatC: " + Math.Round(activeDNA.genes[1].Item2, 3) * 100 + "%";
                BV3.Text = "EatT: " + Math.Round(activeDNA.genes[2].Item2, 3) * 100 + "%";
                BV4.Text = "EatG: " + Math.Round(activeDNA.genes[3].Item2, 3) * 100 + "%";
                BV5.Text = "Divide: " + Math.Round(activeDNA.genes[4].Item2, 3) * 100 + "%";
                BV6.Text = "Wander: " + Math.Round(activeDNA.genes[5].Item2, 3) * 100 + "%";
                BV7.Text = "Attack: " + Math.Round(activeDNA.genes[6].Item2, 3) * 100 + "%";
                BV8.Text = "Wait: " + Math.Round(activeDNA.genes[7].Item2, 3) * 100 + "%";
            }
            addButton.Text = "Add Probability";
            addButton.position = position + new Vector2(xOffset - addButton.TextSize.X - 10, 30);
            BV1.position = position + new Vector2(xOffset, 60);
            BV2.position = position + new Vector2(xOffset, BV1.position.Y + BV1.TextSize.Y + 10);
            BV3.position = position + new Vector2(xOffset, BV2.position.Y + BV2.TextSize.Y + 10);
            BV4.position = position + new Vector2(xOffset, BV3.position.Y + BV3.TextSize.Y + 10);
            BV5.position = position + new Vector2(xOffset, BV4.position.Y + BV1.TextSize.Y + 10);
            BV6.position = position + new Vector2(xOffset, BV5.position.Y + BV2.TextSize.Y + 10);
            BV7.position = position + new Vector2(xOffset, BV6.position.Y + BV3.TextSize.Y + 10);
            BV7.position = position + new Vector2(xOffset, BV6.position.Y + BV3.TextSize.Y + 10);
            BV8.position = position + new Vector2(xOffset, BV7.position.Y + BV3.TextSize.Y + 10);
            BV1.rectangle = generateSplit(0);
            BV2.rectangle = generateSplit(1);
            BV3.rectangle = generateSplit(2);
            BV4.rectangle = generateSplit(3);
            BV5.rectangle = generateSplit(4);
            BV6.rectangle = generateSplit(5);
            BV7.rectangle = generateSplit(6);
            BV8.rectangle = generateSplit(7);

            if (currentState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released && cellManager.selectedCell != null)
            {
                
                Vector2 transformedMouseState = Vector2.Transform(currentState.Position.ToVector2(), Game1.world.rooms.GetState("upgrade").cameras.CurrentState.InverseTransform);
                Console.WriteLine("Wednesday: " + transformedMouseState);
                Console.WriteLine("Weekdays" + addButton.position);
                Console.WriteLine("Thursday: " + BV1.rectangle);
                addButton.rectangle = generateRectangle(1481, 19, 1679, 39);
                if (addButton.rectangle.Contains(transformedMouseState))
                {
                    
                    Console.WriteLine("suh ");
                    doAdd = true;
                }
                else if (BV1.rectangle.Contains(transformedMouseState) && doAdd)
                {
               
                    activeDNA.influenceGene(0, 5.00);
                    doAdd = false;
                }
                else if (BV2.rectangle.Contains(transformedMouseState) && doAdd)
                {
                    activeDNA.influenceGene(1, 5.00);
                    doAdd = false;
                }
                else if (BV3.rectangle.Contains(transformedMouseState) && doAdd)
                {
                    activeDNA.influenceGene(2, 5.00);
                    doAdd = false;
                }
                else if (BV4.rectangle.Contains(transformedMouseState) && doAdd)
                {
                    activeDNA.influenceGene(3, 5.00);
                    doAdd = false;
                }
                else if (BV5.rectangle.Contains(transformedMouseState) && doAdd)
                {
                    activeDNA.influenceGene(4, 5.00);
                    doAdd = false;
                }
                else if (BV6.rectangle.Contains(transformedMouseState) && doAdd)
                {
                    activeDNA.influenceGene(5, 5.00);
                    doAdd = false;
                }
                else if (BV7.rectangle.Contains(transformedMouseState) && doAdd)
                {
                    activeDNA.influenceGene(6, 5.00);
                    doAdd = false;
                }
                else if (BV8.rectangle.Contains(transformedMouseState) && doAdd)
                {
                    activeDNA.influenceGene(7, 5.00);
                    doAdd = false;
                }
            }
            previousMouseState = currentState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);
            BV1.Draw(spriteBatch);
            BV2.Draw(spriteBatch);
            BV3.Draw(spriteBatch);
            BV4.Draw(spriteBatch);
            BV5.Draw(spriteBatch);
            BV6.Draw(spriteBatch);
            BV7.Draw(spriteBatch);
            BV8.Draw(spriteBatch);
            addButton.Draw(spriteBatch);
        }

        private Rectangle generateRectangle(int StartX, int StartY,int EndX, int EndY) {
            return new Rectangle(StartX, StartY, Math.Abs(EndX - StartX), Math.Abs(EndY - StartY));
        }

        private Rectangle generateSplit(int index) {
            return new Rectangle(1687, 46 + (45 * index), 168, 45);
        }
    }
}
