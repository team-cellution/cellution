﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace cellution
{
    public class Room
    {
        private List<Tuple<Action, int>> updateMethods;
        private List<Tuple<Action<SpriteBatch>, int>> drawMethods;

        public PermanantStates<Camera> cameras;

        public Room(GraphicsDeviceManager graphics)
        {
            updateMethods = new List<Tuple<Action, int>>();
            drawMethods = new List<Tuple<Action<SpriteBatch>, int>>();
            cameras = new PermanantStates<Camera>();
            cameras.AddState("camera1", new Camera(graphics.GraphicsDevice.Viewport, Camera.CameraFocus.TopLeft));
        }

        public void AddUpdate(Action updateMethod)
        {
            AddUpdate(updateMethod, 0);
        }

        public void AddUpdate(Action updateMethod, int order)
        {
            updateMethods.Add(new Tuple<Action, int>(updateMethod, order));
            updateMethods.Sort((a, b) =>
            {
                return a.Item2.CompareTo(b.Item2);
            });
        }

        public void RemoveUpdate(Action updateMethod)
        {
            for (int i = 0; i < updateMethods.Count; i++)
            {
                if (updateMethods[i].Item1 == updateMethod)
                {
                    updateMethods.RemoveAt(i);
                    return;
                }
            }
        }

        public void Update()
        {
            cameras.CurrentState.Update();
            foreach (var method in updateMethods)
            {
                method.Item1.Invoke();
            }
        }

        public void AddDraw(Action<SpriteBatch> drawMethod)
        {
            AddDraw(drawMethod, 0);
        }

        public void AddDraw(Action<SpriteBatch> drawMethod, int order)
        {
            drawMethods.Add(Tuple.Create(drawMethod, order));
            drawMethods.Sort((a, b) =>
            {
                return a.Item2.CompareTo(b.Item2);
            });
        }

        public void RemoveDraw(Action<SpriteBatch> drawMethod)
        {
            for (int i = 0; i < drawMethods.Count; i++)
            {
                if (drawMethods[i].Item1 == drawMethod)
                {
                    drawMethods.RemoveAt(i);
                    return;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var method in drawMethods)
            {
                method.Item1.Invoke(spriteBatch);
            }
        }
    }
}
