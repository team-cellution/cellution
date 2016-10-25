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
            cells[0].name = "one";
            cells.Add(new Cell(cellTexture, 100, 100));
            cells[1].name = "two";
        }

        public void Update()
        {
            a = 0;
            c = 0;
            g = 0;
            t = 0;
            List<Cell> cellsToDivide = new List<Cell>();
            foreach (Cell cell in cells)
            {
                cell.Update();
                if (cell.divide == true)
                {
                    cellsToDivide.Add(cell);
                    cell.divide = false;
                }
                a += cell.a;
                c += cell.c;
                g += cell.g;
                t += cell.t;
            }
            // Divide Step
            foreach (Cell cell in cellsToDivide)
            {
                divideCell(cell);
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Cell cell in cells)
            {
                cell.Draw(spriteBatch);
            }
        }

        public void divideCell(Cell cell)
        {
            cell.a = cell.a / 2;
            cell.c = cell.c / 2;
            cell.g = cell.g / 2;
            cell.t = cell.t / 2;
            cells.Add(new Cell(cellTexture, (int) cell.position.X, (int) cell.position.Y));
            Cell newCell = cells[cells.Count-1];
            newCell.a = cell.a;
            newCell.c = cell.c;
            newCell.g = cell.g;
            newCell.t = cell.t;
        }
    }
}
