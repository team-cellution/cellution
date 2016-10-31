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
        
        public int cellCap;

        public CellManager(Texture2D cellTexture, GraphicsDeviceManager graphics)
        {
            this.cellTexture = cellTexture;
            this.graphics = graphics;
            cells = new List<Cell>();
            selectedCell = null;
            cellCap = 15;
        }

        public void SpawnCell()
        {
            cells.Add(CreateCell(Vector2.Zero));
            //cells[0].name = "one";
            cells.Add(CreateCell(new Vector2(100, 100)));
            //cells[1].name = "two";
            cells.Add(CreateCell(new Vector2(200, 200)));
            cells.Add(CreateCell(new Vector2(500, 500)));
            cells.Add(CreateCell(new Vector2(250, 200)));
            cells.Add(CreateCell(new Vector2(550, 500)));
            cells.Add(CreateCell(new Vector2(200, 100)));
            cells.Add(CreateCell(new Vector2(900, 500)));
        }

        private Cell CreateCell(Vector2 position)
        {
            Cell cell = new Cell(position, cellTexture, graphics, new SpriteSheetInfo(120, 120));
            cell.sprite.animations["divide"] = cell.sprite.animations.AddSpriteSheet(World.textureManager["Cell-Division"], 9, 3, 3, SpriteSheet.Directions.LeftToRight, 250, false);
            cell.sprite.animations.CurrentAnimationName = null;
            cell.sprite.animations.SetFrameAction("divide", 8, cell.SetDoneDividing);
            int cellColor = World.Random.Next(5);
            if (cellColor == 0)
            {
                // red
                cell.sprite.color = new Color(219, 107, 94);
            }
            else if (cellColor == 1)
            {
                // yellow
                cell.sprite.color = new Color(224, 227, 87);
            }
            else if (cellColor == 2)
            {
                // green
                cell.sprite.color = new Color(109, 221, 101);
            }
            else if (cellColor == 3)
            {
                // blue
                cell.sprite.color = new Color(75, 209, 239);
            }
            else if (cellColor == 4)
            {
                // purple
                cell.sprite.color = new Color(176, 93, 232);
            }
            cell.sprite.alpha = 0.8f;
            return cell;
        }

        public void Update(GameTime gameTime)
        {
            a = 0;
            c = 0;
            g = 0;
            t = 0;

            //Console.WriteLine("\n");
            List<Cell> cellsToDivide = new List<Cell>();
            List<Cell> cellsToKill = new List<Cell>();
            List<Cell> cellsToCreate = new List<Cell>();
            foreach (Cell cell in cells)
            {
                //Console.WriteLine(cell.id + " X:" + cell.sprite.position.X + " Y:" + cell.sprite.position.Y);
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
                    cellsToCreate.Add(cell);
                }
            }
            // Divide Step
            foreach (Cell cell in cellsToDivide)
            {
                if (cells.Count <= cellCap)
                {
                    StartCellDivision(cell);
                }
            }
            foreach (Cell cell in cellsToCreate)
            {
                DivideCell(cell);
            }
            // Kill Step
            foreach (Cell cell in cellsToKill)
            {
                Console.WriteLine("Killed " + cell.id);
                KillCell(cell);
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

        public void StartCellDivision(Cell cell)
        {
            cell.sprite.animations.CurrentAnimationName = "divide";
        }

        public void DivideCell(Cell cell)
        {
            cell.a /= 2;
            cell.c /= 2;
            cell.g /= 2;
            cell.t /= 2;
            Cell newCell = CreateCell(cell.sprite.position);
            newCell.dna = cell.dna;
            newCell.a = cell.a;
            newCell.c = cell.c;
            newCell.g = cell.g;
            newCell.t = cell.t;
            cells.Add(newCell);
        }

        public void KillCell(Cell cell)
        {
            cells.Remove(cell);
        }
    }
}
