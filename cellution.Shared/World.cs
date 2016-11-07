using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace cellution
{
    public class World
    {
        public static Random Random { get; private set; }

        static World()
        {
            Random = new Random();
        }

        GraphicsDeviceManager graphics;
        public int width;
        public int height;
        SpriteBatch spriteBatch;
        public static ContentManager<Texture2D> textureManager;
        public static ContentManager<SpriteFont> fontManager;

        public PermanantStates<Room> rooms;
        public CellManager cellManager;
        public ResourceManager resourceManager;

        public Texture2D oneByOne;

        public World(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            rooms = new PermanantStates<Room>();
            rooms.AddState("game", new Room(graphics));
        }

        public void Update(GameTime gameTime)
        {
            rooms.CurrentState.Update(gameTime);
        }

        public void BeginDraw()
        {
            BeginDraw(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                null, null, null, null,
                rooms.CurrentState.cameras.CurrentState.Transform);
        }

        public void BeginDraw(SpriteSortMode spriteSortMode, BlendState blendState)
        {
            BeginDraw(spriteSortMode, blendState,
                null, null, null, null,
                rooms.CurrentState.cameras.CurrentState.Transform);
        }

        public void BeginDraw(SpriteSortMode sortMode, BlendState blendState,
            SamplerState samplerState, DepthStencilState depthStencilState,
            RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
        {
            spriteBatch.Begin(sortMode, blendState, samplerState,
                depthStencilState, rasterizerState, effect, transformMatrix);
        }

        public void Draw()
        {
            rooms.CurrentState.Draw(spriteBatch);
        }

        public void Draw(Action<SpriteBatch> drawMethod)
        {
            drawMethod.Invoke(spriteBatch);
        }

        public void EndDraw()
        {
            spriteBatch.End();
        }
    }
}
