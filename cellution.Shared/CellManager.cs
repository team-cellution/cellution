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
        private GraphicsDeviceManager graphics;
        public List<Cell> cells;
        public int a;
        public int c;
        public int g;
        public int t;
        public Cell selectedCell;
        List<Cell> cellsToDivide;
        List<Cell> cellsToKill;
        public int cellCap;

        public CellManager(Texture2D cellTexture, GraphicsDeviceManager graphics)
        {
            this.cellTexture = cellTexture;
            this.graphics = graphics;
            cells = new List<Cell>();
            cellsToDivide = new List<Cell>();
            cellsToKill = new List<Cell>();
            selectedCell = null;
            cellCap = 15;
        }

        public void SpawnCell()
        {
            cells.Add(CreateCell(Vector2.Zero));
            cells[0].name = "one";
            cells.Add(CreateCell(new Vector2(100, 100)));
            cells[1].name = "two";
        }

        private Cell CreateCell(Vector2 position)
        {
            Cell cell = new Cell(position, cellTexture, graphics, new SpriteSheetInfo(120, 120));
            cell.animations["divide"] = cell.animations.AddSpriteSheet(World.textureManager["Cell-Division"], 9, 3, 3, SpriteSheet.Directions.LeftToRight, 250, false);
            cell.animations.CurrentAnimationName = null;
            cell.animations.SetFrameAction("divide", 8, cell.SetDoneDividing);
            return cell;
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
                if (cell.DoneDividing)
                {
                    cell.DoneDividing = false;
                    DivideCell(cell);
                }
            }
            // Divide Step
            foreach (Cell cell in cellsToDivide)
            {
<<<<<<< HEAD
                if (cells.Count <= cellCap)
                {
                    divideCell(cell);
                }
=======
                StartCellDivision(cell);
>>>>>>> origin/master
            }
            // Kill Step
            foreach (Cell cell in cellsToKill)
            {
                Console.WriteLine("Killed " + cell.id);
                KillCell(cell);
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

        public void StartCellDivision(Cell cell)
        {
            cell.animations.CurrentAnimationName = "divide";
        }

        public void DivideCell(Cell cell)
        {
            cell.a = cell.a / 2;
            cell.c = cell.c / 2;
            cell.g = cell.g / 2;
            cell.t = cell.t / 2;
<<<<<<< HEAD
            cells.Add(new Cell(cellTexture, (int) cell.position.X, (int) cell.position.Y, cell.dna));
=======
            cells.Add(CreateCell(cell.position));
>>>>>>> origin/master
            Cell newCell = cells[cells.Count-1];
            newCell.a = cell.a;
            newCell.c = cell.c;
            newCell.g = cell.g;
            newCell.t = cell.t;
        }

        public void KillCell(Cell cell)
        {
            Game1.world.cellManager.cells.Remove(cell);
        }
    }
}
