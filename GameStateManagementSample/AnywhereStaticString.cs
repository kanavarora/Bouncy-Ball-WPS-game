using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace BouncyBall
{
    public class AnywhereStaticString
    {
        List<StaticString> stringsToDraw;

        public AnywhereStaticString()
        {
            stringsToDraw = new List<StaticString>();
        }

        public void AddStaticString(StaticString ss)
        {
            stringsToDraw.Add(ss);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (StaticString ss in stringsToDraw)
            {
                ss.Draw(spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            List<StaticString> toRemove = new List<StaticString>();
            foreach (StaticString ss in stringsToDraw)
            {
                ss.Update(gameTime);
                if (!ss.toDraw)
                {
                    toRemove.Add(ss);
                }
            }

            // check to see if any have been done and remove
            foreach (StaticString ss in toRemove)
            {
                if (stringsToDraw.Contains(ss))
                {
                    stringsToDraw.Remove(ss);
                }
            }
        }

        public void ResetAllStrings()
        {
            foreach (StaticString ss in stringsToDraw)
            {
                ss.CancelDrawing();
            }
            stringsToDraw = new List<StaticString>();
        }
    }
}
