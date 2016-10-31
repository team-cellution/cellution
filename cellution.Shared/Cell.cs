using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;

namespace cellution
{
    public class Cell
    {
        public Sprite sprite;
        public Vector2 targetPosition;
        public int a;
        public int c;
        public int g;
        public int t;
        public string name;
        public int id;
        public DNA dna;
        private double rand;
        public int behavior; // -1 is ready, -2 is player command, -3 is mid cell action
        public bool divide;
        public int lastBehavior;
        public TimeSpan waitUntil;
        public DateTime deathDay;
        public bool kill;
        public Cell targetCell;
        public double speed;
        public double attackRange;
        public bool DoneDividing { get; set; }

        public Cell(Vector2 position, Texture2D texture, GraphicsDeviceManager graphics, SpriteSheetInfo spriteSheetInfo)
        {
            sprite = new Sprite(texture, graphics, spriteSheetInfo);
            sprite.position = position;
            id = World.Random.Next(0, int.MaxValue);
            behavior = -1;
            lastBehavior = -4;
            divide = false;
            waitUntil = new TimeSpan(0);
            deathDay = DateTime.Now.AddHours(1);
            kill = false;
            targetCell = this;
            dna = new DNA();
            speed = 10.0;
            attackRange = 200;
        }

        public void Update(GameTime gameTime)
        {
            if (DateTime.Now.CompareTo(deathDay) == 1)
            {
                Console.WriteLine("Marked for death " + id);
                kill = true;
            }

            if (behavior != 7 && Vector2.Distance(sprite.position, targetPosition) < speed)
            {
                sprite.velocity = Vector2.Zero;
                if (behavior != 6)
                {
                    behavior = -1;
                }
            }

            /*if (lastBehavior == -4)
            {
                behavior = 7;
            }*/

            if (behavior == -1) // If the Cell is doing nothing.
            {
                rand = World.Random.NextDouble();
                int tempIndex = 0;
                foreach (Tuple<int, double> gene in dna.genes)
                {
                    rand -= gene.Item2;
                    if (rand <= 0)
                    {
                        behavior = gene.Item1;
                        dna.influenceGene(tempIndex, 1);
                        dna.print();
                        break;
                    }
                    tempIndex++;
                }
            }

            if (lastBehavior != behavior)
            {
                Console.WriteLine(id + " " + behavior);
            }

            lastBehavior = behavior;
            switch (behavior)
            {
                // Eat A
                case 0:
                    eat(0);
                    break;
                // Eat C
                case 1:
                    eat(1);
                    break;
                // Eat G
                case 2:
                    eat(2);
                    break;
                // Eat T
                case 3:
                    eat(3);
                    break;
                // Divide
                case 4:
                    if (a >= 10 && c >= 10 && g >= 10 && t >= 10)
                    {
                        divide = true;
                    }
                    behavior = -1;
                    break;
                // Wander
                case 5:
                    goTo(new Vector2(World.Random.Next(Game1.world.resourceManager.viewport.Width), World.Random.Next(Game1.world.resourceManager.viewport.Height)));//randomVector);
                    behavior = -3;
                    break;
                // Attack
                // ADD Chase length
                case 6:
                    if (targetCell.id == id)
                    {
                        double nTDist = Double.MaxValue;
                        foreach (Cell cell in Game1.world.cellManager.cells)
                        {
                            if (cell.id != id)
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
                        // If it couldn't find any cells in range, reset behavior
                        if (nTDist > attackRange || targetCell.id == id)
                        {
                            targetCell = this;
                            behavior = -1;
                        }
                    }
                    // If target lives
                    else if (!kill && targetCell.id != id && Game1.world.cellManager.cells.Contains(targetCell))
                    {
                        // If at target, kill them and reset
                        if (Vector2.Distance(sprite.position, targetCell.sprite.position) < speed)
                        {
                            targetCell.kill = true;
                            a += targetCell.a * 3 / 4;
                            c += targetCell.c * 3 / 4;
                            g += targetCell.g * 3 / 4;
                            t += targetCell.t * 3 / 4;
                            targetCell = this;
                            behavior = -1;
                        }
                        else
                        {
                            // Give chase
                            goTo(targetCell.sprite.position);
                        }
                    }
                    else // If target died before it could be killed
                    {
                        behavior = -1;
                    }
                    break;
                // Wait
                case 7:
                    if(waitUntil.Ticks == 0)
                    {
                        waitUntil = gameTime.TotalGameTime.Add(new TimeSpan(0, 0, 1));
                        //Console.WriteLine("gameTime: " + gameTime.TotalGameTime + " waitUntil: " + waitUntil);
                        sprite.velocity = Vector2.Zero;
                    }
                    else if (gameTime.TotalGameTime.CompareTo(waitUntil) == 1)
                    {
                        //Console.Write("Stop Waiting: " + gameTime.TotalGameTime);
                        waitUntil = new TimeSpan(0);
                        behavior = -1;
                    }
                    break;
                // Mid Action, Do Nothing
                default:
                    break;
            }
            sprite.Update(gameTime);
        }
        private void eat(int resourceType)
        {
            Resource.ResourceTypes rType;
            if (resourceType == 0)
            {
                /*if (a >= 15)
                {
                    return;
                }*/
                rType = Resource.ResourceTypes.A;
            }
            else if (resourceType == 1)
            {
                /*if (c >= 15)
                {
                    return;
                }*/
                rType = Resource.ResourceTypes.C;
            }
            else if (resourceType == 2)
            {
                /*if (g >= 15)
                {
                    return;
                }*/
                rType = Resource.ResourceTypes.G;
            }
            else if (resourceType == 3)
            {
                /*if (t >= 15)
                {
                    return;
                }*/
                rType = Resource.ResourceTypes.T;
            }
            else
            {
                throw new System.ArgumentException("Parameter must be between 0 (A) and 3 (T)", "resourceType");
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
                goTo(newTarget);
                behavior = -3;
            }
            else // If there are none of these resources
            {
                behavior = -1;
            }
        }

        public void goTo(Vector2 target)
        {
            targetPosition = target;
            sprite.velocity = new Vector2(targetPosition.X - sprite.position.X, targetPosition.Y - sprite.position.Y);
            sprite.velocity.Normalize();
            sprite.velocity *= (float) speed;//2.0f;
        }

        public void SetDoneDividing()
        {
            DoneDividing = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}
