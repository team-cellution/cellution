using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace cellution
{
    public class TextureManager
    {
        private ContentManager Content;
        private Dictionary<string, Texture2D> textures;

        public Texture2D this[string key]
        {
            get
            {
                if (textures.ContainsKey(key))
                {
                    return textures[key];
                }
                else
                {
                    throw new Exception("That texture hasn't been loaded");
                }
            }
        }

        public TextureManager(ContentManager Content)
        {
            this.Content = Content;
            textures = new Dictionary<string, Texture2D>();
        }

        public void Load(string textureName)
        {
            textures.Add(textureName, Content.Load<Texture2D>(textureName));
        }
    }
}
