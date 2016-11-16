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
        private const int barMax = 100;
        private int value;
        private Sprite bar;
        private TextItem title;
        private TextItem percentage;
        private TextItem letter;
        private Sprite addButton;
        private TextItem increaseText;
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
                title.position = position + new Vector2(bar.drawRect.Width / 2 - title.TextSize.X / 2, 0);
                Vector2 barCorner = position + new Vector2(0, title.TextSize.Y + 20);
                bar.position = barCorner + new Vector2(0, barMax - bar.drawRect.Height);
                percentage.position = barCorner + new Vector2(bar.drawRect.Width / 2 - percentage.TextSize.X / 2, barMax / 2 - percentage.TextSize.Y / 2);
                letter.position = barCorner + new Vector2(bar.drawRect.Width / 2 - letter.TextSize.X / 2, barMax + 20);
                addButton.position = new Vector2(barCorner.X, letter.position.Y + letter.TextSize.Y + 20);
                increaseText.position = addButton.position + new Vector2(addButton.tex.Width + 20, 0);
            }
        }
        private MouseState previousMouseState;
        private DebugRectangle debugRect;

        public UpgradeRoomBar(GraphicsDeviceManager graphics)
        {
            bar = new Sprite(graphics);
            bar.drawRect.Width = 100;
            value = 0;
            title = new TextItem(World.fontManager["InfoFont"], "");
            percentage = new TextItem(World.fontManager["InfoFont"], "");
            percentage.color = Color.Black;
            letter = new TextItem(World.fontManager["InfoFont"], "");
            addButton = GenerateUpArrow();
            addButton.origin = Vector2.Zero;
            increaseText = new TextItem(World.fontManager["InfoFont"], "1%");
            increaseText.color = Color.Green;
            debugRect = new DebugRectangle(graphics);
            debugRect.SetColor(Color.Black);
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

        public void SetColor(Color color)
        {
            title.color = color;
            bar.color = color;
            letter.color = color;
        }

        public void SetTitle(string title)
        {
            this.title.Text = title;
        }

        public void SetLetter(string letter)
        {
            this.letter.Text = letter.ToUpper();
        }

        public void SetValue(int value)
        {
            this.value = MathHelper.Clamp(value, 0, barMax);
            bar.drawRect.Height = this.value;
            percentage.Text = $"{((float)this.value / (float)barMax) * 100}%";
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            Position = position;
            bar.Update(gameTime);
            title.Update();
            percentage.Update();
            letter.Update();
            addButton.Update(gameTime);
            increaseText.Update();

            debugRect.UpdateRectangle(addButton.rectangle);
            Vector2 transformedMouseState = Vector2.Transform(mouseState.Position.ToVector2(), Game1.world.rooms.CurrentState.cameras.CurrentState.InverseTransform);
            if (addButton.rectangle.Contains(transformedMouseState) &&
                mouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released)
            {
                SetValue(this.value + 1);
            }
            previousMouseState = mouseState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            bar.DrawRect(spriteBatch);
            title.Draw(spriteBatch);
            percentage.Draw(spriteBatch);
            letter.Draw(spriteBatch);
            addButton.Draw(spriteBatch);
            increaseText.Draw(spriteBatch);
            debugRect.Draw(spriteBatch);
        }
    }
}
