using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;

namespace cellution
{
    public class Cell : Sprite
    {
        public Vector2 targetPosition;
        public int a;
        public int c;
        public int g;
        public int t;
        public string name;
        public int id;
        public List<Tuple<int, double>> dna = new List<Tuple<int, double>>();
        private double rand;
        public int behavior; // -1 is ready, -2 is player command, -3 is mid cell action
        public bool divide;
        public int lastBehavior;
        public TimeSpan waitUntil;
        public DateTime deathDay;
        public bool kill;

        public Cell(Texture2D loadedTex, int x, int y) : base(loadedTex)
        {
            position = new Vector2(x, y);
            id = World.Random.Next(0, int.MaxValue);
            behavior = -1;
            lastBehavior = -4;
            divide = false;
            waitUntil = new TimeSpan(0);
            deathDay = DateTime.Now.AddMinutes(4);
            kill = false;

            foreach (int i in Enumerable.Range(0, 8))
            {
                dna.Add(new Tuple<int, double>(i, .125));
            }
            /*foreach(Tuple<int, double> p in dna)
            {
                Console.Write(p+", ");
            }*/
        }

        public Cell(GraphicsDeviceManager graphics, SpriteSheetInfo spriteSheetInfo) : base (graphics, spriteSheetInfo)
        {
            position = Vector2.Zero;
            id = World.Random.Next(0, int.MaxValue);
            behavior = -1;
            lastBehavior = -4;
            divide = false;
            waitUntil = new TimeSpan(0);
            deathDay = DateTime.Now.AddMinutes(4);
            kill = false;

            foreach (int i in Enumerable.Range(0, 8))
            {
                dna.Add(new Tuple<int, double>(i, .125));
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (DateTime.Now.CompareTo(deathDay) == 1)
            {
                Console.WriteLine("Marked for death " + id);
                kill = true;
            }

            if (behavior != 7 && Vector2.Distance(position, targetPosition) < 5)
            {
                velocity = Vector2.Zero;
                behavior = -1;
            }

            /*/if (lastBehavior == -4)
            {
                behavior = 7;
            }*/

            if (behavior == -1) // If the Cell is doing nothing.
            {
                rand = World.Random.NextDouble();
                foreach (Tuple<int, double> gene in dna)
                {
                    rand -= gene.Item2;
                    if (rand <= 0)
                    {
                        behavior = gene.Item1;
                        break;
                    }
                }
            }

            /*if (lastBehavior != behavior)
            {
                Console.WriteLine(id + " " + behavior);
            }*/

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
                // Procreate
                case 6:
                    behavior = -1;
                    break;
                // Wait
                case 7:
                    if(waitUntil.Ticks == 0)
                    {
                        waitUntil = gameTime.TotalGameTime.Add(new TimeSpan(0, 0, 1));
                        //Console.WriteLine("gameTime: " + gameTime.TotalGameTime + " waitUntil: " + waitUntil);
                        velocity = Vector2.Zero;
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
            base.Update(gameTime);
        }
        private void eat(int resourceType)
        {
            Resource.ResourceTypes rType;
            if (resourceType == 0)
            {
                if (a >= 15)
                {
                    return;
                }
                rType = Resource.ResourceTypes.A;
            }
            else if (resourceType == 1)
            {
                if (c >= 15)
                {
                    return;
                }
                rType = Resource.ResourceTypes.C;
            }
            else if (resourceType == 2)
            {
                if (g >= 15)
                {
                    return;
                }
                rType = Resource.ResourceTypes.G;
            }
            else if (resourceType == 3)
            {
                if (t >= 15)
                {
                    return;
                }
                rType = Resource.ResourceTypes.T;
            }
            else
            {
                throw new System.ArgumentException("Parameter must be between 0 (A) and 3 (T)", "resourceType");
            }
            
            Vector2 newTarget = position;
            double nTDist = Double.MaxValue;
            foreach (Resource r in Game1.world.resourceManager.resources)
            {
                if (r.resourceType == rType)
                {
                    double temp = Vector2.Distance(r.sprite.position, position);
                    if (temp < nTDist)
                    {
                        nTDist = temp;
                        newTarget = r.sprite.position;
                    }
                }
            }
            if (newTarget != position)
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
            velocity = new Vector2(targetPosition.X - position.X, targetPosition.Y - position.Y);
            velocity.Normalize();
            velocity *= 5.0f;
        }
    }
}
