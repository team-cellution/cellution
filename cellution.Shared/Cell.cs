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
        public TimeSpan chaseUntil;
        public float speed;
        public double AttackRange;
        public bool DoneDividing { get; set; }
        public bool selected;

        public Cell(Vector2 position, Texture2D texture, GraphicsDeviceManager graphics, SpriteSheetInfo spriteSheetInfo)
        {
            sprite = new Sprite(texture, graphics, spriteSheetInfo);
            sprite.position = position;
            sprite.origin = new Vector2(texture.Width/2, texture.Height/2);
            sprite.scale = 1f;
            id = World.Random.Next(0, int.MaxValue);
            behavior = -1;
            lastBehavior = -4;
            divide = false;
            waitUntil = new TimeSpan(0);
            deathDay = DateTime.Now.AddMinutes(5);
            kill = false;
            targetCell = this;
            chaseUntil = new TimeSpan(0);
            dna = new DNA();
            speed = 2.0f;
            AttackRange = 300;
            selected = false;
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
                    Eat(0);
                    break;
                // Eat C
                case 1:
                    Eat(1);
                    break;
                // Eat G
                case 2:
                    Eat(2);
                    break;
                // Eat T
                case 3:
                    Eat(3);
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
                    Wander();
                    behavior = -3;
                    break;
                // Attack
                case 6:
                    Attack(gameTime);
                    break;
                // Wait
                case 7:
                    wait(gameTime);
                    break;
                // Mid Action, Do Nothing
                default:
                    break;
            }
            if (selected)
            {
                dna.SetUpDNAValues(sprite.position);
            }
            sprite.Update(gameTime);
        }
        private void Eat(int resourceType)
        {
            Resource.ResourceTypes rType;
            if (resourceType == 0)
            {
                rType = Resource.ResourceTypes.A;
            }
            else if (resourceType == 1)
            {
                rType = Resource.ResourceTypes.C;
            }
            else if (resourceType == 2)
            {
                rType = Resource.ResourceTypes.G;
            }
            else if (resourceType == 3)
            {
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
                GoTo(newTarget);
                behavior = -3;
            }
            else // If there are none of these resources
            {
                behavior = -1;
            }
        }

        public void Attack(GameTime gameTime)
        {
            if (targetCell.id == id)
            {
                double nTDist = Double.MaxValue;
                foreach (Cell cell in Game1.world.cellManager.cells)
                {
                    // Only attack those who are different colored and smaller
                    if (cell.sprite.color != sprite.color && cell.sprite.scale < sprite.scale)//cell.id != id)
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
                if (nTDist > AttackRange || targetCell.id == id)
                {
                    targetCell = this;
                    behavior = -1;
                }
            }
            // If target lives
            else if (!kill && targetCell.id != id && Game1.world.cellManager.cells.Contains(targetCell))
            {
                // If at target and target is smaller, kill them and reset
                if (sprite.rectangle.Contains(targetCell.sprite.position) && targetCell.sprite.scale < sprite.scale)
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
                    // Cells only give chase for 5 seconds before giving up
                    if (chaseUntil.Ticks == 0)
                    {
                        chaseUntil = gameTime.TotalGameTime.Add(new TimeSpan(0, 0, 0, 5));
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
            else // If target died before it could be killed
            {
                behavior = -1;
            }
        }

        public void wait(GameTime gameTime)
        {
            if (waitUntil.Ticks == 0)
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
        }

        public void Wander()
        {
            GoTo(new Vector2(World.Random.Next(Game1.world.resourceManager.viewport.Width), World.Random.Next(Game1.world.resourceManager.viewport.Height)));
        }

        public void KillCellIfTooOld()
        {
            // Cell dies of old age
            if (DateTime.Now.CompareTo(deathDay) == 1)
            {
                Console.WriteLine("Marked for dEath " + id);
                kill = true;
            }
        }

        public void StopCellIfReady()
        {
            // If at target and not waiting
            if (behavior != 7 && Vector2.Distance(sprite.position, targetPosition) < sprite.tex.Width * sprite.scale / 2 - 3)
            {
                // Stop
                sprite.velocity = Vector2.Zero;
                // If not Attacking, reset behavior
                if (behavior != 6)
                {
                    behavior = -1;
                }
            }
        }

        private void KillInteriorCells()
        {
            foreach (Cell cell in Game1.world.cellManager.cells)
            {
                // If at target and target is smaller, kill them and reset
                if (cell.sprite.color != sprite.color && sprite.rectangle.Contains(cell.sprite.position) && cell.sprite.scale < sprite.scale)
                {
                    cell.kill = true;
                    a += cell.a * 3 / 4;
                    c += cell.c * 3 / 4;
                    g += cell.g * 3 / 4;
                    t += cell.t * 3 / 4;
                }
            }
        }

        public void TryRegenerateBehavior()
        {
            // Generate a random behavior choice if behavior is reset and the cell is not selected
            if (behavior == -1 && this != Game1.world.cellManager.selectedCell)
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
                        if (sprite.color == Game1.world.cellManager.playerColor)
                        {
                            dna.print();
                        }
                        break;
                    }
                    tempIndex++;
                }
            }
        }

        public void GoTo(Vector2 target)
        {
            targetPosition = target;
            sprite.velocity = new Vector2(targetPosition.X - sprite.position.X, targetPosition.Y - sprite.position.Y);
            sprite.velocity.Normalize();
            sprite.velocity *= speed;
        }

        public void UpdateSize()
        {
            sprite.scale = (a + c + g + t) / 400f + 1f;
        }

        public void SetDoneDividing()
        {
            DoneDividing = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
            if (selected)
            {
                dna.DrawDNAValues(spriteBatch);
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

        public void SetColor(int index)
        {
            if (index == 0)
            {// red
                sprite.color = new Color(219, 107, 94);
            }
            else if (index == 1)
            {// yellow
                sprite.color = new Color(224, 227, 87);
            }
            else if (index == 2)
            {// green
                sprite.color = new Color(109, 221, 101);
            }
            else if (index == 3)
            {// purple
                sprite.color = new Color(176, 93, 232);
            }
            else if (index == 4)
            {// blue
                sprite.color = new Color(75, 209, 239);
            }
            sprite.alpha = 0.8f;
        }
    }
}
