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
            Cell selectedCell = Game1.world.cellManager.selectedCell;
            // Insures that the selectedCell is always highLighted
            if (targetCell != selectedCell)
            {
                SetHighlightCell(selectedCell);
            }
            if (targetCell != null)
            {
                sprite.position = targetCell.sprite.position;
                sprite.scale = targetCell.sprite.scale;
                sprite.Update(gameTime);
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (sprite.visible && (targetCell.sprite.animations.active == false))
            {
                sprite.Draw(spriteBatch);
            }
        }
    }
}
