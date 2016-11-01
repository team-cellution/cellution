﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

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
        public int numCellsAtStart;
        public bool startWithRandomEpigenes;

        public CellManager(Texture2D cellTexture, GraphicsDeviceManager graphics)
        {
            this.cellTexture = cellTexture;
            this.graphics = graphics;
            cells = new List<Cell>();
            selectedCell = null;
            cellCap = 15;
            numCellsAtStart = 12;
            startWithRandomEpigenes = true;
    }

        public void SpawnCell()
        {
            foreach (int i in Enumerable.Range(0, numCellsAtStart))
            {
                cells.Add(CreateCell(new Vector2(World.Random.Next(Game1.world.resourceManager.viewport.Width), World.Random.Next(Game1.world.resourceManager.viewport.Height))));
                // Random Epigenetics
                if (startWithRandomEpigenes)
                {
                    int index = 0;
                    foreach (Tuple<int, double> gene in cells.Last().dna.genes)
                    {
                        cells.Last().dna.replaceGene(index, gene.Item1, World.Random.Next(100));
                        index++;
                    }
                    cells.Last().dna.recalcEpigenes();
                }
            }

        }

        private Cell CreateCell(Vector2 position)
        {
            Cell cell = new Cell(position, cellTexture, graphics, new SpriteSheetInfo(120, 120));
            cell.sprite.animations["divide"] = cell.sprite.animations.AddSpriteSheet(World.textureManager["Cell-Division"], 9, 3, 3, SpriteSheet.Directions.LeftToRight, 250, false);
            cell.sprite.animations.CurrentAnimationName = null;
            cell.sprite.animations.SetFrameAction("divide", 8, cell.SetDoneDividing);
            int cellColor = World.Random.Next(5);
            cell.setColor(cellColor);
            return cell;
        }

        public void Update(GameTime gameTime)
        {
            // Keeps the number of alive cells constant
            /*if (cells.Count < cellCap)
            {
                cells.Add(CreateCell(new Vector2(World.Random.Next(0, 1920), World.Random.Next(0, 1080))));
            }*/

            // 1 in 500 updates a cell will change color
            if (World.Random.Next(500) == 0)
            {
                cells[World.Random.Next(cells.Count)].setColor(World.Random.Next(5));
            }

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
            // To be implemented in another fashion, for now, 1 in 5 divides changes color
            if (World.Random.Next(5) == 0)
            {
                newCell.setColor(World.Random.Next(5));
            }
            else
            {
                newCell.sprite.color = cell.sprite.color;
            }

            cells.Add(newCell);
        }

        public void KillCell(Cell cell)
        {
            cells.Remove(cell);
        }
    }
}
