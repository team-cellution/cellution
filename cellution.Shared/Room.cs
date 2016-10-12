using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace cellution
{
    public class Room
    {
        private List<Action> updateMethods;
        private List<Action<SpriteBatch>> drawMethods;

        public PermanantStates<Camera> cameras;

        public Room(GraphicsDeviceManager graphics)
        {
            updateMethods = new List<Action>();
            drawMethods = new List<Action<SpriteBatch>>();
            cameras = new PermanantStates<Camera>();
            cameras.AddState("camera1", new Camera(graphics.GraphicsDevice.Viewport, Camera.CameraFocus.TopLeft));
        }

        public void AddUpdate(Action updateMethod)
        {
            updateMethods.Add(updateMethod);
        }

        public void RemoveUpdate(Action updateMethod)
        {
            updateMethods.Remove(updateMethod);
        }

        public void Update()
        {
            cameras.CurrentState.Update();
            foreach (var method in updateMethods)
            {
                method.Invoke();
            }
        }

        public void AddDraw(Action<SpriteBatch> drawMethod)
        {
            drawMethods.Add(drawMethod);
        }

        public void RemoveUpdate(Action<SpriteBatch> drawMethod)
        {
            drawMethods.Remove(drawMethod);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var method in drawMethods)
            {
                method.Invoke(spriteBatch);
            }
        }
    }
}
