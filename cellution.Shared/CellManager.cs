using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
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
        List<Cell> cellsToDivide;
        List<Cell> cellsToKill;

        public CellManager(Texture2D cellTexture)
        {
            cells = new List<Cell>();
            cellsToDivide = new List<Cell>();
            cellsToKill = new List<Cell>();
            this.cellTexture = cellTexture;
            selectedCell = null;
        }

        public void CreateCell(GraphicsDeviceManager graphics)
        {
            //Cell cell = new Cell(graphics, new SpriteSheetInfo(726, 726));
            //cell.animations["divide"] = cell.animations.AddSpriteSheet(World.textureManager["Cell-Division"], 9, 3, 3, SpriteSheet.Directions.LeftToRight, 1000, true);
            //cell.animations.CurrentAnimationName = "divide";
            cells.Add(new Cell(cellTexture, 0, 0));
            //cells.Add(cell);
            cells[0].name = "one";
            cells.Add(new Cell(cellTexture, 100, 100));
            cells[1].name = "two";
        }

        public void Update(GameTime gameTime)
        {
            a = 0;
            c = 0;
            g = 0;
            t = 0;
            foreach (Cell cell in cells)
            {
                cell.Update(gameTime);
                if (cell.divide == true)
                {
                    cellsToDivide.Add(cell);
                    cell.divide = false;
                }
                if (cell.kill == true)
                {
                    cellsToKill.Add(cell);
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
            // Kill Step
            foreach (Cell cell in cellsToKill)
            {
                Console.WriteLine("Killed " + cell.id);
                killCell(cell);
            }
            cellsToDivide.Clear();
            cellsToKill.Clear();
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

        public void killCell(Cell cell)
        {
            Game1.world.cellManager.cells.Remove(cell);
        }
    }
}
