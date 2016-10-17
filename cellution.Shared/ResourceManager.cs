﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace cellution
{
    public class ResourceManager
    {
        public List<Resource> resources;
        private const int totalResources = 10;
        public int currentResources = 0;
        private Viewport viewport;

        public ResourceManager(Viewport viewport)
        {
            this.viewport = viewport;
            resources = new List<Resource>();
        }

        public void Update()
        {
            while (currentResources < totalResources)
            {
                Resource resource = new Resource();
                resource.sprite.position = new Vector2(World.Random.Next(viewport.Width), World.Random.Next(viewport.Height));
                resources.Add(resource);
                currentResources++;
            }

            foreach (Resource resource in resources)
            {
                resource.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Resource resource in resources)
            {
                resource.Draw(spriteBatch);
            }
        }
    }
}
