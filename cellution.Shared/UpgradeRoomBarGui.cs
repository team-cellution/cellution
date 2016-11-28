using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace cellution
{
    public class UpgradeRoomBarGui
    {
        private GraphicsDeviceManager graphics;

        private Sprite barsOverlay;
        private Sprite aBar;
        private Sprite aRect;
        private Sprite cBar;
        private Sprite cRect;
        private Sprite gBar;
        private Sprite gRect;
        private Sprite tBar;
        private Sprite tRect;

        public UpgradeRoomBarGui(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            barsOverlay = new Sprite(World.textureManager["UI-bars"]);
            barsOverlay.origin = Vector2.Zero;
            barsOverlay.position = new Vector2(760, 330);

            aBar = LoadBar(new Vector2(768, 335), World.Red);
            cBar = LoadBar(new Vector2(991, 335), World.Yellow);
            gBar = LoadBar(new Vector2(1215, 335), World.Green);
            tBar = LoadBar(new Vector2(1437, 335), World.Blue);

            aRect = LoadRectangle(new Vector2(768, 335));
            cRect = LoadRectangle(new Vector2(991, 335));
            gRect = LoadRectangle(new Vector2(1215, 335));
            tRect = LoadRectangle(new Vector2(1437, 335));
        }

        private Sprite LoadBar(Vector2 position, Color color)
        {
            Sprite bar = new Sprite(World.textureManager["filled_bar"]);
            bar.origin = Vector2.Zero;
            bar.position = position;
            bar.color = color;
            return bar;
        }

        private Sprite LoadRectangle(Vector2 position)
        {
            Sprite rectangle = new Sprite(graphics);
            rectangle.origin = Vector2.Zero;
            rectangle.position = position;
            rectangle.drawRect = new Rectangle((int)position.X, (int)position.Y, 162, 221);
            return rectangle;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            aBar.Draw(spriteBatch);
            aRect.DrawRect(spriteBatch);
            cBar.Draw(spriteBatch);
            cRect.DrawRect(spriteBatch);
            gBar.Draw(spriteBatch);
            gRect.DrawRect(spriteBatch);
            tBar.Draw(spriteBatch);
            tRect.DrawRect(spriteBatch);
            barsOverlay.Draw(spriteBatch);
        }
    }
}
