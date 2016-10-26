using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

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
                sprite = new Sprite(World.textureManager["a"]);
            }
            else if (resourceType == ResourceTypes.C)
            {
                sprite = new Sprite(World.textureManager["c"]);
            }
            else if (resourceType == ResourceTypes.G)
            {
                sprite = new Sprite(World.textureManager["g"]);
            }
            else if (resourceType == ResourceTypes.T)
            {
                sprite = new Sprite(World.textureManager["t"]);
            }
        }

        public void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}
