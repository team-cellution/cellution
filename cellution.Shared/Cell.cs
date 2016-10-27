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
        public int behavior; // -1 is nothing, -2 is player command, -3 is mid cell action
        public bool divide;
        public int lastBehavior = -4;
        public int waitTimer;

        public Cell(Texture2D loadedTex, int x, int y) : base(loadedTex)
        {
            position = new Vector2(x, y);
            id = World.Random.Next(0, int.MaxValue);
            behavior = -1;
            divide = false;
            waitTimer = 10000;
            foreach (int i in Enumerable.Range(0, 6))
            {
                dna.Add(new Tuple<int, double>(i, 1.0/8));
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
            divide = false;
            waitTimer = 10000;
            foreach (int i in Enumerable.Range(0, 6))
            {
                dna.Add(new Tuple<int, double>(i, 1.0 / 7));
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Vector2.Distance(position, targetPosition) < 5)
            {
                velocity = Vector2.Zero;
                behavior = -1;
            }

            if (behavior == -1) // If the Cell is doing nothing.
            {
                rand = World.Random.NextDouble();
                // Currently Broken
                foreach (Tuple<int, double> gene in dna)
                {
                    rand -= gene.Item2;
                    if (rand <= 0)
                    {
                        behavior = gene.Item1;
                        break;
                    }
                }
                //behavior = World.Random.Next(0, 8);
            }
            if (lastBehavior != behavior)
            {
                Console.WriteLine(name + " " + behavior);
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
                // Procreate
                case 6:
                    behavior = -1;
                    break;
                // Wait
                case 7:
                    // Broken
                    /*
                    if (waitTimer > 0)
                    {
                        waitTimer -= 1;
                    }
                    else
                    {
                        waitTimer = 10000;
                        behavior = -1;
                    }*/
                    behavior = -1;
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
