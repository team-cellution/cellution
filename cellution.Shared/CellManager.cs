using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace cellution
{
    public class CellManager
    {
        private Texture2D cellTexture;
        public List<Cell> cells;
        public int a;
        public int c;
        public int g;
        public int t;
        public Cell selectedCell;

        public CellManager(Texture2D cellTexture)
        {
            cells = new List<Cell>();
            this.cellTexture = cellTexture;
            selectedCell = null;
        }

        public void CreateCell()
        {
            cells.Add(new Cell(cellTexture, 0, 0));
            cells.Add(new Cell(cellTexture, 100, 100));
        }

        public void Update()
        {
            a = 0;
            c = 0;
            g = 0;
            t = 0;
            foreach (Cell cell in cells)
            {
                cell.Update();
                a += cell.a;
                c += cell.c;
                g += cell.g;
                t += cell.t;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Cell cell in cells)
            {
                cell.Draw(spriteBatch);
            }
        }

        public void DrawSelected(SpriteBatch spriteBatch)
        {
            if (selectedCell != null)
            {
                selectedCell.Draw(spriteBatch);
            }
            
        }
    }
}
