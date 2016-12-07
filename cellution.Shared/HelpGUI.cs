using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace cellution
{
    class HelpGUI
    {
        public string room;
        private GraphicsDeviceManager graphics;
        private Sprite Cell;
        private Texture2D rect;
        private TextItem aHelp;
        private TextItem cHelp;
        private TextItem gHelp;
        private TextItem tHelp;
        private TextItem divHelp;
        private TextItem divHelp2;
        private TextItem gainRes;
        private TextItem gainRes2;
        private TextItem changePage;
        private TextItem conMove;
        private TextItem conUpgrade;
        private TextItem conHelp;
        private TextItem conDivide;
        private TextItem conWander;
        private TextItem conNext;
        private TextItem conPrev;
        private TextItem conAuto;
        private TextItem conArrows;
        private Vector2 position = new Vector2(0,0);
        private List<TextItem> pageBasics = new List<TextItem>();
        private List<TextItem> pageControls = new List<TextItem>();
        private List<TextItem> active;
        private KeyboardState keys;
        private KeyboardState prevKeys;


        public HelpGUI(string room, Sprite Cell, GraphicsDeviceManager g) {
            this.graphics = g;
            this.room = room;
            if (Cell != null)
            {
                this.Cell = Cell;
                aHelp = new TextItem(World.fontManager["ScoreFont"], "This shows how many red resources have been eaten");
                cHelp = new TextItem(World.fontManager["ScoreFont"], "This shows how many yellow resources have been eaten");
                gHelp = new TextItem(World.fontManager["ScoreFont"], "This shows how many green resources have been eaten");
                tHelp = new TextItem(World.fontManager["ScoreFont"], "This shows how many blue resources have been eaten");
                divHelp = new TextItem(World.fontManager["ScoreFont"], "When these are all 10 or above you can divide your cell into 2 cells");
                divHelp2 = new TextItem(World.fontManager["ScoreFont"], "They each split the resources of the original cell");
                gainRes = new TextItem(World.fontManager["ScoreFont"], "You can gain resources by either eating them directly,");
                gainRes2 = new TextItem(World.fontManager["ScoreFont"], "or attacking other teams' cells that have them and are smaller than you");
                changePage = new TextItem(World.fontManager["ScoreFont"], "To Change Instruction Pages Press 'F' ");
                



                aHelp.origin = Vector2.Zero;
                cHelp.origin = Vector2.Zero;
                gHelp.origin = Vector2.Zero;
                tHelp.origin = Vector2.Zero;
                divHelp.origin = Vector2.Zero;
                divHelp2.origin = Vector2.Zero;
                gainRes.origin = Vector2.Zero;
                gainRes2.origin = Vector2.Zero;
                changePage.origin = Vector2.Zero;
                

                aHelp.position = position + new Vector2(200, 30);
                cHelp.position = position + new Vector2(200, aHelp.position.Y + aHelp.TextSize.Y + 10);
                gHelp.position = position + new Vector2(200, cHelp.position.Y + cHelp.TextSize.Y + 10);
                tHelp.position = position + new Vector2(200, gHelp.position.Y + gHelp.TextSize.Y + 10);
                divHelp.position = position + new Vector2(50, tHelp.position.Y + tHelp.TextSize.Y + 70);
                divHelp.color = Color.Maroon;
                divHelp2.position = position + new Vector2(50, divHelp.position.Y + divHelp.TextSize.Y + 10);
                divHelp2.color = Color.Maroon;
                gainRes.position = position + new Vector2(0, divHelp2.position.Y + divHelp2.TextSize.Y + 10);
                gainRes.color = Color.Black;
                gainRes2.position = position + new Vector2(0, gainRes.position.Y + gainRes.TextSize.Y + 10);
                gainRes2.color = Color.Black;
                changePage.position = position + new Vector2(800, gainRes2.position.Y + gainRes2.TextSize.Y + 40);
                changePage.color = Color.Black;
                addTo(pageBasics, aHelp, cHelp, gHelp, tHelp, divHelp, divHelp2, gainRes, gainRes2, changePage);
                active = pageBasics;


                /*------------------------------------------------------------------------------------------PageChanges---------*/





                conMove = new TextItem(World.fontManager["ScoreFont"], "When a Cell is selected, you can right click a location to have your cell move there");
                conUpgrade = new TextItem(World.fontManager["ScoreFont"], "Press Space to access the upgrade Room");
                conHelp = new TextItem(World.fontManager["ScoreFont"], "Press tab to go to/exit the help screen(This Screen)");
                conDivide = new TextItem(World.fontManager["ScoreFont"], "Press D to divide if you have 10 or more of each resource");
                conWander = new TextItem(World.fontManager["ScoreFont"], "Press W to wander to a new location on the map");
                conNext = new TextItem(World.fontManager["ScoreFont"], "Press E to switch to the next player cell, if you have multiple");
                conPrev = new TextItem(World.fontManager["ScoreFont"], "Press Q to switch to the previous player cell, if you have multiple");
                conAuto = new TextItem(World.fontManager["ScoreFont"], "Press S to enable/disable auto behavior on the selected cell while it is selected");
                conArrows = new TextItem(World.fontManager["ScoreFont"], "Use The Arrow Keys to Move the selected Cell");
                initializer(conMove, conUpgrade, conHelp, conDivide, conWander, conNext, conPrev, conAuto, conArrows);
                addTo(pageControls, conMove, conUpgrade, conHelp, conDivide, conWander, conNext, conPrev, conAuto, conArrows, changePage);




                if (room == "game")
                {


                }
            }


        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (Cell != null)
            {
                TextItem T = new TextItem(World.fontManager["Impact-36"], "This is your Cell");
                Vector2 newPos = new Vector2(Cell.position.X - Cell.rectangle.Width , Cell.position.Y - Cell.rectangle.Height );
                T.position = newPos;
                T.Draw(spriteBatch);
                System.Console.WriteLine(Cell.rectangle);
            }

            foreach (TextItem x in active) {
                x.Draw(spriteBatch);
            }


        }

        public void updateStatsGUIPosition(Vector2 v) {
            position = new Vector2(v.X, v.Y);
        }

        public void Update(GameTime gametime) {
            keys = Keyboard.GetState();
            if (keys.IsKeyDown(Keys.F) && prevKeys.IsKeyUp(Keys.F))
            {
                if (active == pageBasics)
                {
                    active = pageControls;
                }
                else {
                    active = pageBasics;
                }
            }
            prevKeys = keys;

        }


        private void addTo(List<TextItem> l, params TextItem[] list) {
            foreach (TextItem x in list) {
                l.Add(x);
            }


        }

        private void initializer(params TextItem[] list) {
            Vector2 prevPos = new Vector2(400, 30);
            float prevTSize = 0;
           
            foreach (TextItem x in list) {
                x.origin = Vector2.Zero;
                x.color = Color.DarkGreen;
                x.position = position + new Vector2(200, prevPos.Y + prevTSize + 10);
                prevPos = x.position;
                prevTSize = x.TextSize.Y;
            }
        }
    }
}
