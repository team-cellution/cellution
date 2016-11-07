using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace cellution
{
    public class HighlightRing
    {
        public Sprite sprite;
        private Cell targetCell;

        public HighlightRing(Texture2D texture)
        {
            sprite = new Sprite(texture);
            sprite.color = Color.Black;
            Hide();
            targetCell = null;
        }

        public void Show()
        {
            sprite.visible = true;
        }

        public void Hide()
        {
            sprite.visible = false;
        }

        public void SetHighlightCell(Cell cell)
        {
            targetCell = cell;
            if (targetCell == null)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        public void Update(GameTime gameTime)
        {
            if (targetCell != null)
            {
                sprite.position = targetCell.sprite.position;
                sprite.Update(gameTime);
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (sprite.visible)
            {
                sprite.Draw(spriteBatch);
            }
        }
    }
}
