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
        public bool selected;
        public string name;
        public int id;
        public List<Tuple<int, double>> dna = new List<Tuple<int, double>>();

        public Cell(Texture2D loadedTex, int x, int y) : base(loadedTex)
        {
            position = new Vector2(x, y);
            id = World.Random.Next(0, int.MaxValue);
            foreach (int i in Enumerable.Range(0, 6))
            {
                dna.Add(new Tuple<int, double>(i, 1.0/7));
            }
            foreach(Tuple<int, double> p in dna)
            {
                Console.Write(p+", ");
            }
        }

        public override void Update()
        {
            if (Vector2.Distance(position, targetPosition) < 5)
            {
                velocity = Vector2.Zero;
            }
            base.Update();
        }
    }
}
