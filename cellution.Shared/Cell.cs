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
        public int behavior; // -1 is nothing, -2 is player command

        public Cell(Texture2D loadedTex, int x, int y) : base(loadedTex)
        {
            position = new Vector2(x, y);
            id = World.Random.Next(0, int.MaxValue);
            behavior = -1;
            foreach (int i in Enumerable.Range(0, 6))
            {
                dna.Add(new Tuple<int, double>(i, 1.0/7));
            }
            /*foreach(Tuple<int, double> p in dna)
            {
                Console.Write(p+", ");
            }*/
        }

        public override void Update()
        {
            if (Vector2.Distance(position, targetPosition) < 5)
            {
                velocity = Vector2.Zero;
                behavior = -1;
            }
            Console.WriteLine(name + " " + behavior);
            rand = World.Random.NextDouble();
            if (behavior == -1) // If the Cell is doing nothing.
            {
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
            switch (behavior)
            {
                // Eat A
                case 0:
                    //Console.WriteLine("Case 0");
                    Vector2 newTarget = position;
                    double nTDist = Double.MaxValue;
                    foreach (Resource r in Game1.world.resourceManager.resources)
                    {
                        if (r.resourceType == Resource.ResourceTypes.A)
                        {
                            //Console.WriteLine(r.sprite.position);
                            double temp = Vector2.Distance(r.sprite.position, position);
                            //Console.WriteLine(temp + " vs " + nTDist);
                            if (temp < nTDist)
                            {
                                nTDist = temp;
                                newTarget = r.sprite.position;
                            }
                        }
                    }
                    //Console.WriteLine("newTarget" + newTarget);
                    if (newTarget != position)
                    {
                        targetPosition = newTarget;
                        velocity = new Vector2(targetPosition.X - position.X, targetPosition.Y - position.Y);
                        velocity.Normalize();
                        velocity *= 5.0f;
                    }
                    //Console.WriteLine("targetPosition" + targetPosition);
                    break;
                // Eat C
                case 1:
                    behavior = -1;
                    break;
                // Eat G
                case 2:
                    behavior = -1;
                    break;
                // Eat T
                case 3:
                    behavior = -1;
                    break;
                // Divide
                case 4:
                    behavior = -1;
                    break;
                // Wander
                case 5:
                    behavior = -1;
                    break;
                // Procreate
                case 6:
                    behavior = -1;
                    break;
                // Attack
                case 7:
                    behavior = -1;
                    break;
                // Mid Action, Do Nothing
                default:
                    break;
            }
            base.Update();
        }
    }
}
