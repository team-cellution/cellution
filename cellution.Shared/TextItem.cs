using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace cellution
{
    public class TextItem : SpriteBase
    {
        public SpriteFont font;
        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                TextSize = font.MeasureString(text);
            }
        }
        public Vector2 TextSize { get; private set; }

        public TextItem(SpriteFont loadedFont, string spriteText = "")
        {
            font = loadedFont;
            Text = spriteText;
            position = Vector2.Zero;
            velocity = Vector2.Zero;
            UpdateRectangle();
            visible = true;
            color = Color.White;
            alpha = 1.0f;
            rotation = 0.0f;
            scale = 1.0f;
            origin = new Vector2(TextSize.X / 2, TextSize.Y / 2);
        }

        public override void Update()
        {
            position += velocity;
            UpdateRectangle();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, position, color * alpha, MathHelper.ToRadians(rotation), origin, scale, SpriteEffects.None, 0);
        }

        private void UpdateRectangle()
        {
            rectange = new Rectangle((int)Math.Round(position.X), (int)Math.Round(position.Y), (int)Math.Round(TextSize.X), (int)Math.Round(TextSize.Y));
        }
    }
}
