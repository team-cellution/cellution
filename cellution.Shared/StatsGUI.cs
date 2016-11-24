using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace cellution
{
    public class StatsGUI
    {
        private const float xOffset = 150;

        private GraphicsDeviceManager graphics;
        private Vector2 position;
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                background.position = position;
                aScore.position = position + new Vector2(xOffset, 30);
                cScore.position = position + new Vector2(xOffset, aScore.TextSize.Y + 10);
                gScore.position = position + new Vector2(xOffset, cScore.TextSize.Y + 10);
                tScore.position = position + new Vector2(xOffset, gScore.TextSize.Y + 10);
                upgradeHud.position = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height - upgradeHud.tex.Height / 2);
                upgradeInfoText.position = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, upgradeHud.position.Y - upgradeHud.tex.Height / 2 - upgradeInfoText.TextSize.Y - 10);
            }
        }
        private Sprite background;
        private TextItem aScore;
        private TextItem cScore;
        private TextItem gScore;
        private TextItem tScore;
        private CellManager cellManager;
        private Sprite upgradeHud;
        private TextItem upgradeInfoText;
        private TimeSpan upgradeHudTimer = TimeSpan.FromSeconds(30);

        public StatsGUI(Texture2D background, CellManager cellManager, GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            this.background = new Sprite(background);
            this.background.origin = Vector2.Zero;
            aScore = new TextItem(World.fontManager["ScoreFont"], "0");
            aScore.origin = Vector2.Zero;
            aScore.color = new Color(248, 215, 241);
            cScore = new TextItem(World.fontManager["ScoreFont"], "0");
            cScore.origin = Vector2.Zero;
            cScore.color = new Color(245, 248, 195);
            gScore = new TextItem(World.fontManager["ScoreFont"], "0");
            gScore.origin = Vector2.Zero;
            gScore.color = new Color(193, 250, 196);
            tScore = new TextItem(World.fontManager["ScoreFont"], "0");
            tScore.origin = Vector2.Zero;
            tScore.color = new Color(190, 245, 255);
            this.cellManager = cellManager;
            upgradeHud = new Sprite(World.textureManager["upgrade_now"]);
            upgradeHud.visible = false;
            upgradeInfoText = new TextItem(World.fontManager["ScoreFont"], $"Press {World.upgradeRoomKey.ToString()} to");
            upgradeInfoText.color = Color.Black;
            Position = this.background.position;
        }

        public void Update(GameTime gameTime)
        {
            if (cellManager.selectedCell == null)
            {
                aScore.Text = cellManager.a.ToString();
                cScore.Text = cellManager.c.ToString();
                gScore.Text = cellManager.g.ToString();
                tScore.Text = cellManager.t.ToString();
            }
            else
            {
                aScore.Text = cellManager.selectedCell.a.ToString();
                cScore.Text = cellManager.selectedCell.c.ToString();
                gScore.Text = cellManager.selectedCell.g.ToString();
                tScore.Text = cellManager.selectedCell.t.ToString();
            }
            aScore.position = position + new Vector2(xOffset, 30);
            cScore.position = position + new Vector2(xOffset, aScore.position.Y + aScore.TextSize.Y + 10);
            gScore.position = position + new Vector2(xOffset, cScore.position.Y + cScore.TextSize.Y + 10);
            tScore.position = position + new Vector2(xOffset, gScore.position.Y + gScore.TextSize.Y + 10);
            if (!upgradeHud.visible)
            {
                upgradeHudTimer -= gameTime.ElapsedGameTime;
            }
            if (upgradeHudTimer <= TimeSpan.Zero)
            {
                upgradeHud.visible = true;
                upgradeHudTimer = TimeSpan.FromMinutes(2.5);
            }
            if (upgradeHud.visible)
            {
                upgradeHud.Update(gameTime);
                upgradeInfoText.Update();
            }
        }

        public void HideUpgradeHud()
        {
            upgradeHud.visible = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(spriteBatch);
            aScore.Draw(spriteBatch);
            cScore.Draw(spriteBatch);
            gScore.Draw(spriteBatch);
            tScore.Draw(spriteBatch);
            if (upgradeHud.visible)
            {
                upgradeHud.Draw(spriteBatch);
                upgradeInfoText.Draw(spriteBatch);
            }
        }
    }
}
