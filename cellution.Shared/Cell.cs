﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;

namespace cellution
{
    public class Cell
    {
        public Sprite sprite; // the cell graphic
        private Sprite sizeShadingSprite; // sprite that indicates to the player the relative size of this cell
        public Vector2 targetPosition; // the position the cell moves towards
        public int a; // amount of 'a' resource contained in the cell
        public int c; // amount of 'c' resource contained in the cell
        public int g; // amount of 'g' resource contained in the cell
        public int t; // amount of 't' resource contained in the cell
        public string name; // debug name for the cell
        public int id; // the unique id of the cell
        public DNA dna; // the dna of the cell (contains behaviors and probabilities (epigenes))
        //private double rand; // random number for generating random behaviors is stored in this
        public int behavior; // -1 is ready, -2 is player command, -3 is mid cell action
        public bool divide; // triggers cell to be added to divide list in CellManager
        public int lastBehavior; // the last behavior of the cell
        public TimeSpan waitUntil; // the timespan the cell must wait for gameTime to be before it stops waiting 
        public DateTime deathDay; // the time that the cell dies of old age
        public bool kill; // triggers cell to be added to kill list in CellManager
        public Cell targetCell; // the targetCell that the cell is attacking, equal to 'this' if it is not attacking
        public TimeSpan chaseUntil; // the timespan the cell must wait for gameTime to be before it stops attacking a cell
        public float speed; // the speed the cell moves at. On each update: speed = startingSpeed * cell.sprite.scale
        public float startingSpeed; // the starting speed of the cell
        public double AttackRange; // the range that the cell can see enemies it might attack
        public int minToDivA; // the minimum amount of 'a' required for the cell to divide
        public int minToDivC; // the minimum amount of 'c' required for the cell to divide
        public int minToDivG; // the minimum amount of 'g' required for the cell to divide
        public int minToDivT; // the minimum amount of 't' required for the cell to divide
        public double absorbEfficiency; // the efficiency of the cell in absorbing the resources of cells it eats
        public bool DoneDividing { get; set; }
        public bool selected; // if the cell is the selectedCell in CellManager
        public bool autoSelected; // if the cell is the selectedCell and it is set to auto behavior mode
        public int chaseSecs; // the amount of seconds the cell will chase a cell it is attacking
        public int waitSecs; // the amount of seconds the cell will wait during the wait behavior
        public int influencePercent; // the percent chance that the chosen behavior's probability is increased with each use
        public float sizeResourceDenominator; // the number that the sum of resources is divided by when calculating size
        public int killingSpreeTotal; // How many cells will it have to kill before stopping its spree
        public int killingSpreeCounter; // how many cells it has killed in the spree so far
        public List<double> eatingPreferences; // probabilities of eating different foods (A, C, G, T)

        public Cell(Vector2 position, Texture2D texture, GraphicsDeviceManager graphics, SpriteSheetInfo spriteSheetInfo)
        {
            dna = new DNA();
            sprite = new Sprite(texture, graphics, spriteSheetInfo);
            sprite.position = position;
            sprite.origin = new Vector2(texture.Width / 2, texture.Height / 2);
            sizeShadingSprite = new Sprite(World.textureManager["shade_circle"]);
            sizeShadingSprite.visible = false;
            id = World.Random.Next(0, int.MaxValue);
            behavior = -1;
            lastBehavior = -4;
            divide = false;
            waitUntil = new TimeSpan(0);
            deathDay = DateTime.Now.AddMinutes(10);
            kill = false;
            targetCell = this;
            chaseUntil = new TimeSpan(0);
            startingSpeed = speed = 3.0f;
            AttackRange = 300;
            selected = false;
            autoSelected = false;
            minToDivA = minToDivC = minToDivG = minToDivT = 10;
            absorbEfficiency = 3 / 4;
            chaseSecs = 5;
            waitSecs = 1;
            influencePercent = 1;
            sizeResourceDenominator = 100f;
            killingSpreeCounter = 0;
            killingSpreeTotal = 5;
            eatingPreferences = new List<double>() {.4, .3, .2, .1};
            eatingPreferences = Shuffle(eatingPreferences);
        }

        public static List<T> Shuffle<T>(List<T> list)
        {
            List<T> tempList = list; 
            int n = tempList.Count;
            while (n > 1)
            {
                n--;
                int k = World.Random.Next(n + 1);
                T value = tempList[k];
                tempList[k] = tempList[n];
                tempList[n] = value;
            }
            return tempList;
        }

        public void Update(GameTime gameTime)
        {
            UpdateSize();
            KillCellIfTooOld();
            StopCellIfReady();
            KillInteriorCells();

            /*if (lastBehavior == -4)
            {
                behavior = 7;
            }*/

            TryRegenerateBehavior();

            /*if (lastBehavior != behavior)
            {
                Console.WriteLine(id + " " + behavior);
            }*/

            lastBehavior = behavior;
            switch (behavior)
            {
                // Eat A
                case 0:
                    Eat();
                    break;
                // Divide
                case 1:
                    if (a >= minToDivA && c >= minToDivC && g >= minToDivG && t >= minToDivT)
                    {
                        divide = true;
                    }
                    behavior = -1;
                    break;
                // Wander
                case 2:
                    Wander();
                    behavior = -3;
                    break;
                // Attack
                case 3:
                    Attack(gameTime);
                    break;
                // Wait
                case 4:
                    Wait(gameTime);
                    break;
                // behaviors past this point can only be mutated/cells dont start with these\
                //
                // congregate (move to nearest friendly cell)
                case 8:
                    Congregate(gameTime);
                    break;
                // killing spree (go on a killing spree of enemies)
                case 9:
                    Attack(gameTime, true);
                    break;
                // cannabalize (eat the closest cell of your own team)
                case 10:
                    Attack(gameTime, false, true);
                    break;
                // glutton (go on an eating spree)
                // breed like rabbits (divide until you cannot anymore)
                // home base (move to a specific corner based on team)
                // flee (run away from the closest cell)

                // Mid Action, Do Nothing
                default:
                    break;
            }
            sprite.Update(gameTime);
            sizeShadingSprite.scale = sprite.scale;
            sizeShadingSprite.position = sprite.position;
        }

        public void CellSizeShadeVisibility(bool visible)
        {
            sizeShadingSprite.visible = visible;
        }

        public void SetCellSizeShadeColor(float scale)
        {
            if (scale <= sprite.scale)
            {
                sizeShadingSprite.color = Color.Red;
            }
            else
            {
                sizeShadingSprite.color = Color.Blue;
            }
        }

        // Eat the nearest resource of type: 0 = a, 1 = c,  2 = g, 3 = t
        private void Eat()
        {
            double random = World.Random.NextDouble();
            Resource.ResourceTypes rType;
            if (random - eatingPreferences[0] <= 0)
            {
                rType = Resource.ResourceTypes.A;
            }
            else if (random - eatingPreferences[0] - eatingPreferences[1] <= 0)
            {
                rType = Resource.ResourceTypes.C;
            }
            else if (random - eatingPreferences[0] - eatingPreferences[1] - eatingPreferences[2] <= 0)
            {
                rType = Resource.ResourceTypes.G;
            }
            else
            {
                rType = Resource.ResourceTypes.T;
            }
            
            Vector2 newTarget = sprite.position;
            double nTDist = Double.MaxValue;
            foreach (Resource r in Game1.world.resourceManager.resources)
            {
                if (r.resourceType == rType)
                {
                    double temp = Vector2.Distance(r.sprite.position, sprite.position);
                    if (temp < nTDist)
                    {
                        nTDist = temp;
                        newTarget = r.sprite.position;
                    }
                }
            }
            if (newTarget != sprite.position)
            {
                GoTo(newTarget);
                behavior = -3;
            }
            else // If there are none of these resources
            {
                behavior = -1;
            }
        }
        
        // cell waits for waitSecs seconds
        public void Wait(GameTime gameTime)
        {
            if (waitUntil.Ticks == 0)
            {
                waitUntil = gameTime.TotalGameTime.Add(new TimeSpan(0, 0, waitSecs));
                //Console.WriteLine("gameTime: " + gameTime.TotalGameTime + " waitUntil: " + waitUntil);
                sprite.velocity = Vector2.Zero;
            }
            else if (gameTime.TotalGameTime.CompareTo(waitUntil) == 1)
            {
                //Console.Write("Stop Waiting: " + gameTime.TotalGameTime);
                waitUntil = new TimeSpan(0);
                behavior = -1;
            }
        }

        // go to a random coordinate on the map
        public void Wander()
        {
            GoTo(new Vector2(World.Random.Next(Game1.world.resourceManager.viewport.Width), World.Random.Next(Game1.world.resourceManager.viewport.Height)));
        }

        // follow the nearest friendly cell
        public void Congregate(GameTime gameTime)
        {
            if (targetCell.id == id)
            {
                double nTDist = Double.MaxValue;
                foreach (Cell cell in Game1.world.cellManager.cells)
                {
                    // Only friendly cells
                    if (cell.sprite.color == sprite.color)//cell.id != id)
                    {
                        double temp = Vector2.Distance(cell.sprite.position, sprite.position);
                        if (temp < nTDist)
                        {
                            nTDist = temp;
                            targetCell = cell;
                        }
                    }
                }
                //Console.WriteLine("Dist " + nTDist);
                // If it couldn't find any friendly cells in range, reset behavior
                if (nTDist > AttackRange || targetCell.id == id)
                {
                    targetCell = this;
                    behavior = -1;
                }
            }
            
            else if (targetCell.id != id && Game1.world.cellManager.cells.Contains(targetCell))
            {
                // If at target cell, reset behavior
                if (sprite.rectangle.Contains(targetCell.sprite.position))
                {
                    targetCell = this;
                    behavior = -1;
                }
                else
                {
                    // Cells only give chase for 5 seconds before giving up
                    if (chaseUntil.Ticks == 0)
                    {
                        chaseUntil = gameTime.TotalGameTime.Add(new TimeSpan(0, 0, 0, chaseSecs));
                        // Give chase
                        GoTo(targetCell.sprite.position);
                    }
                    else if (gameTime.TotalGameTime.CompareTo(chaseUntil) == 1)
                    {
                        chaseUntil = new TimeSpan(0);
                        targetCell = this;
                        behavior = -1;
                    }
                    else
                    {
                        // Give chase
                        GoTo(targetCell.sprite.position);
                    }
                }
            }
            else // If target died before it could be reached
            {
                behavior = -1;
            }
        }

        // attack the neaest enemy cell with optional killing spree or cannibal
        public void Attack(GameTime gameTime, bool spree=false, bool cannibal=false, Cell customTarget = null)
        {
            if (targetCell.id == id)
            {
                double nTDist = Double.MaxValue;
                if (customTarget == null)
                {
                    foreach (Cell cell in Game1.world.cellManager.cells)
                    {
                        // Only attack those who are different colored and smaller
                        if (((!cannibal && cell.sprite.color != sprite.color) || (cannibal && cell.sprite.color == sprite.color)) &&
                            cell.sprite.scale < sprite.scale)//cell.id != id)
                        {
                            double temp = Vector2.Distance(cell.sprite.position, sprite.position);
                            if (temp < nTDist)
                            {
                                nTDist = temp;
                                targetCell = cell;
                            }
                        }
                    }
                }
                else
                {
                    targetCell = customTarget;
                    nTDist = AttackRange;
                    behavior = 3;
                }
                //Console.WriteLine("Dist " + nTDist);
                // If it couldn't find any cells in range, reset behavior
                if (nTDist > AttackRange || targetCell.id == id)
                {
                    targetCell = this;
                    if (spree)
                    {
                        killingSpreeCounter++;
                        if (killingSpreeCounter >= killingSpreeTotal)
                        {
                            killingSpreeCounter = 0;
                            behavior = -1;
                        }
                    }
                    else
                    {
                        behavior = -1;
                    }
                }
            }
            // If target lives
            else if (!kill && targetCell.id != id && Game1.world.cellManager.cells.Contains(targetCell))
            {
                // If at target and target is smaller, kill them and reset
                if (sprite.rectangle.Contains(targetCell.sprite.position) && targetCell.sprite.scale < sprite.scale)
                {
                    targetCell.kill = true;
                    a += (int)(targetCell.a * absorbEfficiency) + 1;
                    c += (int)(targetCell.c * absorbEfficiency) + 1;
                    g += (int)(targetCell.g * absorbEfficiency) + 1;
                    t += (int)(targetCell.t * absorbEfficiency) + 1;
                    targetCell = this;
                    if (spree)
                    {
                        killingSpreeCounter++;
                        if (killingSpreeCounter >= killingSpreeTotal)
                        {
                            killingSpreeCounter = 0;
                            behavior = -1;
                        }
                    }
                    else
                    {
                        behavior = -1;
                    }
                }
                else
                {
                    // Cells only give chase for 5 seconds before giving up
                    if (chaseUntil.Ticks == 0)
                    {
                        chaseUntil = gameTime.TotalGameTime.Add(new TimeSpan(0, 0, 0, chaseSecs));
                        // Give chase
                        GoTo(targetCell.sprite.position);
                    }
                    else if (gameTime.TotalGameTime.CompareTo(chaseUntil) == 1)
                    {
                        chaseUntil = new TimeSpan(0);
                        targetCell = this;
                        if (spree)
                        {
                            killingSpreeCounter++;
                            if (killingSpreeCounter >= killingSpreeTotal)
                            {
                                killingSpreeCounter = 0;
                                behavior = -1;
                            }
                        }
                        else
                        {
                            behavior = -1;
                        }
                    }
                    else if (targetCell.sprite.scale > sprite.scale)
                    {
                        behavior = -1;
                    }
                    else
                    {
                        // Give chase
                        GoTo(targetCell.sprite.position);
                    }
                }
            }
            else // If target died before it could be killed
            {
                if (spree)
                {
                    killingSpreeCounter++;
                    if (killingSpreeCounter >= killingSpreeTotal)
                    {
                        killingSpreeCounter = 0;
                        behavior = -1;
                    }
                }
                else
                {
                    behavior = -1;
                }
            }
        }

        // Kill the cell if its past their deathDay
        public void KillCellIfTooOld()
        {
            // Cell dies of old age
            if (DateTime.Now.CompareTo(deathDay) == 1)
            {
                Console.WriteLine("Marked for death " + id);
                kill = true;
            }
        }

        // crucial behavior which stops the cell and resets behavior if it encompasses the targetPosition and isn't waiting.
        // If it is attacking it does not reset behavior.  
        public void StopCellIfReady()
        {
            // If at target and not waiting
            if (behavior != 4 && Vector2.Distance(sprite.position, targetPosition) < sprite.tex.Width * sprite.scale / 2 - 3)
            {
                // Stop
                sprite.velocity = Vector2.Zero;
                // If not Attacking or congregating, reset behavior
                if (behavior != 3 && behavior != 5)
                {
                    behavior = -1;
                }
            }
        }

        // kill any smaller cells within the interior of the cell
        private void KillInteriorCells()
        {
            foreach (Cell cell in Game1.world.cellManager.cells)
            {
                // If at target and target is smaller, kill them and reset
                if (cell.sprite.color != sprite.color && sprite.rectangle.Contains(cell.sprite.position) && cell.sprite.scale < sprite.scale)
                {
                    cell.kill = true;
                    a += (int)(cell.a * absorbEfficiency);
                    c += (int)(cell.c * absorbEfficiency);
                    g += (int)(cell.g * absorbEfficiency);
                    t += (int)(cell.t * absorbEfficiency);
                }
            }
        }

        // if the behavior is reset (-1) and the cell is not selected or on auto, then the behavior is randomly generated
        // using the probabilities (epigenes) of each gene in the cell's dna
        public void TryRegenerateBehavior()
        {
            // Generate a random behavior choice if behavior is reset and the cell is not selected
            if (behavior == -1 && (this != Game1.world.cellManager.selectedCell || autoSelected))
            {
                double rand = World.Random.NextDouble();
                for (int i = 0; i < dna.genes.Count; i++)
                {
                    DNA.Genes geneKey = (DNA.Genes)i;
                    double geneValue = dna.genes[geneKey];
                    rand -= geneValue;
                    if (rand <= 0)
                    {
                        behavior = i;
                        dna.InfluenceGene(geneKey, influencePercent);
                        if (sprite.color == Game1.world.cellManager.playerColor)
                        {
                            dna.Print();
                        }
                        break;
                    }
                }
            }
        }

        // Moves to the target vector
        public void GoTo(Vector2 target)
        {
            targetPosition = target;
            sprite.velocity = new Vector2(targetPosition.X - sprite.position.X, targetPosition.Y - sprite.position.Y);
            sprite.velocity.Normalize();
            sprite.velocity *= speed;
        }

        // updates the size and speed based on sum of resources
        public void UpdateSize()
        {
            sprite.scale = (a + c + g + t) / sizeResourceDenominator + 1f;
            speed = startingSpeed / sprite.scale;
        }

        public void SetDoneDividing()
        {
            DoneDividing = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
            if (sizeShadingSprite.visible)
            {
                sizeShadingSprite.Draw(spriteBatch);
            }
            //DrawLine(spriteBatch, sprite.position, targetPosition);
        }

        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle = (float) Math.Atan2(edge.Y, edge.X);

            sb.Draw(Game1.world.oneByOne, new Rectangle(// rectangle defines shape of line and position of start of line
                    (int) start.X,
                    (int) start.Y,
                    (int) edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null, Color.Black, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None, 0);

        }

        // Set the cell color to one of the indices
        public void SetColor(int index)
        {
            if (index == 0)
            {
                sprite.color = World.Red;
            }
            else if (index == 1)
            {
                sprite.color = World.Yellow;
            }
            else if (index == 2)
            {
                sprite.color = World.Green;
            }
            else if (index == 3)
            {
                sprite.color = World.Purple;
            }
            else if (index == 4)
            {
                sprite.color = World.Blue;
            }
            sprite.alpha = 0.8f;
        }
    }
}
