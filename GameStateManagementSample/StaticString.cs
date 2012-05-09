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
    public enum PositionOfDisplay
    {
        center,
        quarterdown,
    }

    public class StaticString
    {
        public String displayString;
        public double timeToAppear;
        public Vector2 vecPositionOfDisplay;
        public Color color;
        public SpriteFont spriteFont;
        public double timeAppearedTillNow;
        public GameplayScreen game;
        public Boolean toDraw;

        public StaticString(GameplayScreen game)
        {
            this.game = game;
        }
        public StaticString(GameplayScreen game, String displayString,
            double timeToAppear, Color color, SpriteFont spriteFont, Vector2 vecPosition)
        {
            this.displayString = displayString;
            this.game = game;
            this.timeToAppear = timeToAppear;
            this.timeAppearedTillNow = 0;
            this.color = color;
            this.spriteFont = spriteFont;
            toDraw = true;
            this.vecPositionOfDisplay = vecPosition;
        }

        public void drawNewString(String displayString, double timeToAppear, PositionOfDisplay positionOfDisplay, Color color, SpriteFont spriteFont)
        {
            this.displayString = displayString;
            this.timeToAppear = timeToAppear;
            this.timeAppearedTillNow = 0;
            this.color = color;
            this.spriteFont = spriteFont;
            toDraw = true;
            ConvertPositionToVector(positionOfDisplay);
        }


        public void ConvertPositionToVector(PositionOfDisplay positionOfDisplay)
        {
           // this.vecPositionOfDisplay = (new Vector2(this.game.GameWidth(), this.game.GameHeight()) -
             //   spriteFont.MeasureString(displayString)) / 2;
            Vector2 vecString = spriteFont.MeasureString(displayString);
            this.vecPositionOfDisplay = new Vector2((this.game.GameWidth() - vecString.X)/2, 
                this.game.GameHeight()*0.20f);
           
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (toDraw && timeAppearedTillNow < this.timeToAppear)
                spriteBatch.DrawString(spriteFont, displayString, vecPositionOfDisplay, color);
        }

        public void Update(GameTime gameTime)
        {
            if (toDraw && timeAppearedTillNow < this.timeToAppear)
            {
                timeAppearedTillNow += gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (timeAppearedTillNow > this.timeToAppear)
            {
                toDraw = false;
            }
        }

        public void CancelDrawing()
        {
            toDraw = false;
        }
    }
}
