using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

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

        internal Dictionary<string, Camera> cameras;
        private string currentCamera;
        public string CurrentCamera
        {
            get
            {
                return currentCamera;
            }
            set
            {
                if (cameras.ContainsKey(value))
                {
                    currentCamera = value;
                }
                else
                {
                    throw new Exception("That camera doesn't exist");
                }
            }
        }

        public Camera Camera1 { get; private set; }

        public World(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            cameras = new Dictionary<string, Camera>();
            Camera1 = new Camera(graphics.GraphicsDevice.Viewport, Camera.CameraFocus.TopLeft);
            cameras.Add("camera1", Camera1);
            CurrentCamera = "camera1";
        }

        public void BeginDraw()
        {
            BeginDraw(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                null, null, null, null,
                cameras[currentCamera].Transform);
        }

        public void BeginDraw(SpriteSortMode spriteSortMode, BlendState blendState)
        {
            BeginDraw(spriteSortMode, blendState,
                null, null, null, null,
                cameras[currentCamera].Transform);
        }

        public void BeginDraw(SpriteSortMode sortMode, BlendState blendState,
            SamplerState samplerState, DepthStencilState depthStencilState,
            RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
        {
            spriteBatch.Begin(sortMode, blendState, samplerState,
                depthStencilState, rasterizerState, effect, transformMatrix);
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
