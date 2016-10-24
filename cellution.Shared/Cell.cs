using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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

        public Cell(Texture2D loadedTex, int x, int y) : base(loadedTex)
        {
            position = new Vector2(x, y);
            id = World.Random.Next(0, int.MaxValue);
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
