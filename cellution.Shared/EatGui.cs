using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace cellution
{
    public class EatGui
    {
        private List<KeyValuePair<TextItem, TextItem>> slots;
        private TextItem a;
        private TextItem c;
        private TextItem g;
        private TextItem t;
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
                slots[0].Key.position = position;
                slots[0].Value.PositionRight(slots[0].Key);
                for (int i = 1; i < slots.Count; i++)
                {
                    slots[i].Key.PositionBelow(slots[i - 1].Key);
                    slots[i].Value.PositionRight(slots[i].Key);
                }
            }
        }

        private MouseState previousMouseState;

        public EatGui()
        {
            slots = new List<KeyValuePair<TextItem, TextItem>>();
            slots.Add(new KeyValuePair<TextItem, TextItem>(new TextItem(World.fontManager["InfoFont"], "80%"), null));
            slots.Add(new KeyValuePair<TextItem, TextItem>(new TextItem(World.fontManager["InfoFont"], "60%"), null));
            slots.Add(new KeyValuePair<TextItem, TextItem>(new TextItem(World.fontManager["InfoFont"], "40%"), null));
            slots.Add(new KeyValuePair<TextItem, TextItem>(new TextItem(World.fontManager["InfoFont"], "20%"), null));

            a = new TextItem(World.fontManager["InfoFont"], "Eat A");
            c = new TextItem(World.fontManager["InfoFont"], "Eat C");
            g = new TextItem(World.fontManager["InfoFont"], "Eat G");
            t = new TextItem(World.fontManager["InfoFont"], "Eat T");
            SetColor(Color.Black);
            slots[0] = new KeyValuePair<TextItem, TextItem>(slots[0].Key, a);
            slots[1] = new KeyValuePair<TextItem, TextItem>(slots[1].Key, c);
            slots[2] = new KeyValuePair<TextItem, TextItem>(slots[2].Key, g);
            slots[3] = new KeyValuePair<TextItem, TextItem>(slots[3].Key, t);
        }

        public void SetColor(Color color)
        {
            foreach (var i in slots)
            {
                i.Key.color = color;
            }
            a.color = color;
            c.color = color;
            g.color = color;
            t.color = color;
        }

        private void Swap(TextItem first, TextItem second)
        {
            int position1 = 0;
            int position2 = 0;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].Value == first)
                {
                    position1 = i;
                }
                else if (slots[i].Value == second)
                {
                    position2 = i;
                }
            }

            slots[position1] = new KeyValuePair<TextItem, TextItem>(slots[position1].Key, second);
            slots[position2] = new KeyValuePair<TextItem, TextItem>(slots[position2].Key, first);
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            Position = position;
            foreach (var i in slots)
            {
                i.Key.Update();
            }
            a.Update();
            c.Update();
            g.Update();
            t.Update();

            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].Value.rectangle.Contains(mouseState.Position) &&
                    mouseState.LeftButton == ButtonState.Pressed &&
                    previousMouseState.LeftButton == ButtonState.Released)
                {
                    int previous = i - 1;
                    if (previous < 0)
                    {
                        previous = slots.Count - 1;
                    }
                    Swap(slots[i].Value, slots[previous].Value);
                    break;
                }
            }
            previousMouseState = mouseState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var i in slots)
            {
                i.Key.Draw(spriteBatch);
            }
            a.Draw(spriteBatch);
            c.Draw(spriteBatch);
            g.Draw(spriteBatch);
            t.Draw(spriteBatch);
        }
    }
}
