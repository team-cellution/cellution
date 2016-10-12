using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace cellution
{
    public class Resource
    {
        public enum ResourceTypes
        {
            A,
            C,
            G,
            T
        }

        public ResourceTypes resourceType;
        public Sprite sprite;

        public Resource()
        {
            int type = World.Random.Next(4);
            resourceType = (ResourceTypes)type;
            if (resourceType == ResourceTypes.A)
            {
                sprite = new Sprite(Game1.world.textureManager["a"]);
            }
            else if (resourceType == ResourceTypes.C)
            {
                sprite = new Sprite(Game1.world.textureManager["c"]);
            }
            else if (resourceType == ResourceTypes.G)
            {
                sprite = new Sprite(Game1.world.textureManager["g"]);
            }
            else if (resourceType == ResourceTypes.T)
            {
                sprite = new Sprite(Game1.world.textureManager["t"]);
            }
        }

        public void Update()
        {
            sprite.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}
