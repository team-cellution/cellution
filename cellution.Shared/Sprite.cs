using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace cellution
{
    public class Sprite : SpriteBase
    {
        public Texture2D tex;
        public Rectangle drawRect;
        public Matrix spriteTransform;


        public Sprite(Texture2D loadedTex)
        {
            tex = loadedTex;
            drawRect = new Rectangle((int)Math.Round(position.X), (int)Math.Round(position.Y), 0, 0);
            rectange = new Rectangle((int)Math.Round(position.X), (int)Math.Round(position.Y), tex.Width, tex.Height);
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
        }

        public override void Update()
        {
            position += velocity;
            drawRect.X = (int)Math.Round(position.X);
            drawRect.Y = (int)Math.Round(position.Y);
            rectange = new Rectangle((int)Math.Round(position.X), (int)Math.Round(position.Y), tex.Width, tex.Height);
            spriteTransform = Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                Matrix.CreateScale(scale) * Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(new Vector3(position, 0.0f));
            rectange = CalculateBoundingRectangle(new Rectangle(0, 0, tex.Width, tex.Height), spriteTransform);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, position, null, color * alpha, MathHelper.ToRadians(rotation), origin, scale, SpriteEffects.None, 0);
        }

        public void DrawRect(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, drawRect, null, color * alpha, MathHelper.ToRadians(rotation), origin, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Used for pixel perfect collision with scaled/rotated sprites.
        /// </summary>
        /// <param name="rectangle">The current bounding rectangle</param>
        /// <param name="transform">The current sprite transform matrix</param>
        /// <returns>Returns a new bounding rectangle</returns>
        private Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)Math.Round(min.X), (int)Math.Round(min.Y),
                                 (int)Math.Round(max.X - min.X), (int)Math.Round(max.Y - min.Y));
        }
    }
}
