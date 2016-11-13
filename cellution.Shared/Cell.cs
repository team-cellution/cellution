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
        public Sprite sprite; // the cell graphic
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

        public Cell(Vector2 position, Texture2D texture, GraphicsDeviceManager graphics, SpriteSheetInfo spriteSheetInfo)
        {
            dna = new DNA();
            sprite = new Sprite(texture, graphics, spriteSheetInfo);
            sprite.position = position;
            sprite.origin = new Vector2(texture.Width/2, texture.Height/2);
            sprite.scale = 1f;
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
                    if (a >= minToDivA && c >= minToDivC && g >= minToDivG && t >= minToDivT)
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
        }
        // Eat the nearest resource of type: 0 = a, 1 = c,  2 = g, 3 = t
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
        public void Attack(GameTime gameTime, bool spree=false, bool cannibal=false)
        {
            if (targetCell.id == id)
            {
                double nTDist = Double.MaxValue;
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
                    a += (int)(targetCell.a * absorbEfficiency);
                    c += (int)(targetCell.c * absorbEfficiency);
                    g += (int)(targetCell.g * absorbEfficiency);
                    t += (int)(targetCell.t * absorbEfficiency);
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
            if (behavior != 7 && Vector2.Distance(sprite.position, targetPosition) < sprite.tex.Width * sprite.scale / 2 - 3)
            {
                // Stop
                sprite.velocity = Vector2.Zero;
                // If not Attacking or congregating, reset behavior
                if (behavior != 6 && behavior != 8)
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
                int tempIndex = 0;
                foreach (Tuple<int, double> gene in dna.genes)
                {
                    rand -= gene.Item2;
                    if (rand <= 0)
                    {
                        behavior = gene.Item1;
                        dna.InfluenceGene(tempIndex, influencePercent);
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
