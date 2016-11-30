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
        private UpgradeRoomBar a;
        private UpgradeRoomBar c;
        private UpgradeRoomBar g;
        private UpgradeRoomBar t;

        public UpgradeRoomBarGui(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            barsOverlay = new Sprite(World.textureManager["UI-bars"]);
            barsOverlay.origin = Vector2.Zero;
            barsOverlay.position = new Vector2(760, 330);

            a = new UpgradeRoomBar(graphics, new Vector2(768, 335), World.Red, DNA.Genes.Attack);
            a.SetTitle("Attack");
            a.SetValue(25);
            c = new UpgradeRoomBar(graphics, new Vector2(991, 335), World.Yellow, DNA.Genes.Wait);
            c.SetTitle("Wait");
            c.SetValue(100);
            g = new UpgradeRoomBar(graphics, new Vector2(1215, 335), World.Green, DNA.Genes.Wander);
            g.SetTitle("Wander");
            g.SetValue(50);
            t = new UpgradeRoomBar(graphics, new Vector2(1437, 335), World.Blue, DNA.Genes.Divide);
            t.SetTitle("Speed");
            t.SetValue(5);
        }

        public void UpdateDnaValues(DNA dna)
        {
            a.SetCurrentDna(dna);
            c.SetCurrentDna(dna);
            g.SetCurrentDna(dna);
            t.SetCurrentDna(dna);
        }

        public void Update(GameTime gameTime)
        {
            a.Update(gameTime);
            c.Update(gameTime);
            g.Update(gameTime);
            t.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            a.Draw(spriteBatch);
            c.Draw(spriteBatch);
            g.Draw(spriteBatch);
            t.Draw(spriteBatch);
            barsOverlay.Draw(spriteBatch);
        }
    }
}
