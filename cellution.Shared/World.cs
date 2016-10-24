using System;
using System.Collections;
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
        SpriteBatch spriteBatch;
        public TextureManager textureManager;

        public PermanantStates<Room> rooms;
        public ArrayList cells = new ArrayList();
        public int selectedId;

        public World(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            rooms = new PermanantStates<Room>();
            rooms.AddState("game", new Room(graphics));
            selectedId = -1;
        }

        public void Update()
        {
            rooms.CurrentState.Update();
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
