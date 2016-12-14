using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace cellution
{
    public class UpgradeRoomBar
    {
        private GraphicsDeviceManager graphics;

        private const int barMax = 100;
        private int value;
        private Sprite bar;
        private Sprite rect;
        private TextItem title;
        private TextItem percentage;
        private Sprite addButton;
        private TextItem increaseText;
        private int decreaseValue;
        private TextItem decreaseText;
        private Cell currentCell;
        private DNA.Genes attachedGene;
        private string attachedLetter;
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
                title.position = position + new Vector2(bar.tex.Width / 2 - title.TextSize.X / 2, -(title.TextSize.Y + 10));
                percentage.position = position + new Vector2(bar.tex.Width / 2 - percentage.TextSize.X / 2, barMax / 2 - percentage.TextSize.Y / 2);
                addButton.position = position + new Vector2(0, bar.tex.Height + 10);
                increaseText.position = addButton.position + new Vector2(addButton.tex.Width + increaseText.TextSize.X, increaseText.TextSize.Y / 2);
                decreaseText.position = new Vector2(position.X + bar.tex.Width / 2 - decreaseText.TextSize.X / 2, increaseText.position.Y + increaseText.TextSize.Y + 10);
            }
        }
        private MouseState previousMouseState;
        private DebugRectangle debugRect;

        public UpgradeRoomBar(GraphicsDeviceManager graphics, Vector2 position, Color color, DNA.Genes attachedGene, string attachedLetter)
        {
            this.graphics = graphics;
            bar = LoadBar(position, color);
            rect = LoadRectangle(position);
            value = 0;
            title = new TextItem(World.fontManager["Impact-36"], "");
            title.color = Color.Black;
            percentage = new TextItem(World.fontManager["Impact-36"], "");
            percentage.color = Color.Black;
            addButton = GenerateUpArrow();
            addButton.origin = Vector2.Zero;
            increaseText = new TextItem(World.fontManager["Impact-36"], "1%");
            increaseText.color = Color.Green;
            decreaseValue = 1;
            decreaseText = new TextItem(World.fontManager["Impact-36"], $"-{decreaseValue}");
            decreaseText.color = color;
            currentCell = null;
            this.attachedGene = attachedGene;
            this.attachedLetter = attachedLetter;
            debugRect = new DebugRectangle(graphics);
            debugRect.SetColor(Color.Black);
            Position = position;
        }

        private Sprite LoadBar(Vector2 position, Color color)
        {
            Sprite bar = new Sprite(World.textureManager["filled_bar"]);
            bar.origin = Vector2.Zero;
            bar.position = position;
            bar.color = color;
            return bar;
        }

        private Sprite LoadRectangle(Vector2 position)
        {
            Sprite rectangle = new Sprite(graphics);
            rectangle.origin = Vector2.Zero;
            rectangle.position = position;
            // 0% == 442 height, 100% == 0 height
            rectangle.drawRect = new Rectangle((int)position.X, (int)position.Y, 162, 442);
            return rectangle;
        }

        private Sprite GenerateUpArrow()
        {
            Sprite arrow = new Sprite(World.textureManager["arrow"]);
            arrow.color = Color.Green;
            return arrow;
        }

        private Sprite GenerateDownArrow()
        {
            Sprite arrow = new Sprite(World.textureManager["arrow"]);
            arrow.color = Color.Red;
            return arrow;
        }

        public void SetCurrentCell(Cell cell)
        {
            currentCell = cell;
            if (cell == null)
            {
                SetValueWithPercentage(0);
            }
            else
            {
                SetValueWithPercentage((float)currentCell.dna.genes[attachedGene]);
            }
        }

        public void SetColor(Color color)
        {
            title.color = color;
            bar.color = color;
        }

        public void SetTitle(string title)
        {
            this.title.Text = title;
        }

        public void SetValueWithPercentage(float value)
        {
            value = MathHelper.Clamp(value, 0.0f, 1.0f);
            SetValue((int)(barMax * value));
        }

        public void SetValue(int value)
        {
            this.value = MathHelper.Clamp(value, 0, barMax);
            float tmpValue = 1.0f - this.value / 100.0f;
            rect.drawRect.Height = (int)(tmpValue * 442);
            percentage.Text = $"{((float)this.value / (float)barMax) * 100}%";
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            Position = position;
            bar.Update(gameTime);
            title.Update();
            percentage.Update();
            addButton.Update(gameTime);
            increaseText.Update();
            decreaseText.Text = $"-{decreaseValue}";
            decreaseText.Update();

            debugRect.UpdateRectangle(addButton.rectangle);
            Vector2 transformedMouseState = Vector2.Transform(mouseState.Position.ToVector2(), Game1.world.rooms.CurrentState.cameras.CurrentState.InverseTransform);
            if (addButton.rectangle.Contains(transformedMouseState) &&
                mouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released)
            {
                if (currentCell != null)
                {
                    if (attachedLetter == "a" && currentCell.a >= decreaseValue)
                    {
                        currentCell.a -= decreaseValue;
                        IncreasePercentage();
                    }
                    else if (attachedLetter == "c" && currentCell.c >= decreaseValue)
                    {
                        currentCell.c -= decreaseValue;
                        IncreasePercentage();
                    }
                    else if (attachedLetter == "g" && currentCell.g >= decreaseValue)
                    {
                        currentCell.g -= decreaseValue;
                        IncreasePercentage();
                    }
                    else if (attachedLetter == "t" && currentCell.t >= decreaseValue)
                    {
                        currentCell.t -= decreaseValue;
                        IncreasePercentage();
                    }
                }
            }
            previousMouseState = mouseState;
        }

        private void IncreasePercentage()
        {
            decreaseValue++;
            currentCell.dna.genes[attachedGene] += 0.01;
            currentCell.dna.RecalcEpigenes();
            SetValueWithPercentage((float)currentCell.dna.genes[attachedGene]);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            bar.Draw(spriteBatch);
            rect.DrawRect(spriteBatch);
            title.Draw(spriteBatch);
            percentage.Draw(spriteBatch);
            addButton.Draw(spriteBatch);
            increaseText.Draw(spriteBatch);
            decreaseText.Draw(spriteBatch);
            //debugRect.Draw(spriteBatch);
        }
    }
}
