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
    public class BonusObject
    {
        public Texture2D texture2d;
        public double timeToAppear;
        public double timeAppeared;
        public Vector2 vecPosition;
        public Vector2 scale;
        public Vector2 dimensions;
        public GameplayScreen game;

        //stats
        protected int noOfTimesDrawn;
        protected int noOfTimesAchieved;
        protected int lastDrawnScore;
        protected int lastAchievedScore;
        protected double lastDrawnTime;

        protected Boolean fisDrawing;

        // for shading out an object
        protected int mAlphaValue;
        protected int alphaValueIncrementPerSecond;


        public static int TOTALBONUSOBJS = 5;
        public BonusObject(Texture2D texture2D, GameplayScreen game, Vector2 dimensions, double timeToAppear)
        {
            this.texture2d = texture2D;
            this.game = game;
            this.dimensions = dimensions;
            //CalculateScale();
            this.noOfTimesDrawn = 0;
            this.noOfTimesAchieved = 0;
            this.lastAchievedScore = -1;
            this.lastDrawnScore = -1;
            this.lastDrawnTime = -1;
            this.timeToAppear = timeToAppear;
        }

        public void CalculateScale()
        {
            float width = (float)texture2d.Bounds.Width;
            float height = (float)texture2d.Bounds.Height;

            this.scale = new Vector2((this.dimensions.X) / width, (this.dimensions.Y) / height);
        }

        public static List<BonusObject> currentDrawnBonusObjects = new List<BonusObject>();
        public static void InvalidateAllDrawingObjects()
        {
            foreach (BonusObject bo in currentDrawnBonusObjects)
            {
                bo.CancelDrawing();
            }
            currentDrawnBonusObjects.RemoveRange(0, currentDrawnBonusObjects.Count());
        }

        public static void ResetAllBonusObjects(GameplayScreen game)
        {
            foreach (BonusObject bo in game.bonusObjects)
            {
                bo.ResetStats();
            }
        }

        public static int countOfBonusObject = 0;
        public static void RenderBonusObjects(GameplayScreen game, SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (currentDrawnBonusObjects.Count() == 0)
            {
                List<BonusObject> totalBonusObjects = game.bonusObjects;

                // go through the list of bonus objects and form a list
                // of objects you can actually render
                List<BonusObject> canRenderBonusObjects = new List<BonusObject>();
                foreach (BonusObject bo in totalBonusObjects)
                {
                    if (bo.CanDraw())
                    {
                        canRenderBonusObjects.Add(bo);
                    }
                }

                // go through the list of canRender objects and choose something 
                // to draw
                if (canRenderBonusObjects.Count > 0)
                {
                    // start drawing the bonus object
                    BonusObject toDrawBonusObject = canRenderBonusObjects.ElementAt(0);
                    Vector2 positionToDrawbo = toDrawBonusObject.GetPositionToDraw();
                    toDrawBonusObject.StartDrawing(positionToDrawbo, gameTime);
                    if (!currentDrawnBonusObjects.Contains(toDrawBonusObject))
                    {
                        currentDrawnBonusObjects.Add(toDrawBonusObject);
                        countOfBonusObject++;
                    }
                }
            }

            foreach (BonusObject bo in currentDrawnBonusObjects)
            {
                bo.Draw(spriteBatch);
            }
        }

        public static void UpdateBonusObjects(GameplayScreen game, GameTime gameTime)
        {
            // go throught the list of currrent drawn objects,
            // and check to see if they are achieved.
            // if they are, then cancel drawing on them, and apply the change
            // if they are not, then just update them
            List<BonusObject> bonusObjectsToCancelDrawing = new List<BonusObject>();
            foreach (BonusObject bo in currentDrawnBonusObjects)
            {
                if (bo.isAchieving(game.football))
                {
                    bo.makeChange(game.football, gameTime);
                    bonusObjectsToCancelDrawing.Add(bo);
                    bo.CancelDrawing();
                }
                else if (!bo.Update(gameTime))
                {
                    // if it returns false, we need to cancel drawing too
                    bonusObjectsToCancelDrawing.Add(bo);
                    bo.CancelDrawing();
                }
            }

            foreach (BonusObject bo in bonusObjectsToCancelDrawing)
            {
                if (currentDrawnBonusObjects.Contains(bo))
                {
                    currentDrawnBonusObjects.Remove(bo);
                }
            }
        }

        public void StartDrawing(Vector2 vecPosition, GameTime gameTime)
        {
            this.vecPosition = vecPosition;
            this.timeAppeared = 0;
            this.mAlphaValue = 255;
            this.alphaValueIncrementPerSecond = (int)((2 * 255) / this.timeToAppear);
            this.fisDrawing = true;
            this.noOfTimesDrawn++;
            this.lastDrawnScore = this.game.stats.current();
            this.lastDrawnTime = gameTime.TotalGameTime.Seconds;
        }


        public virtual void makeChange(Football football, GameTime gameTime)
        {
        }

        public Boolean isDrawing()
        {
            return this.fisDrawing;
        }

        public virtual Boolean CanDraw()
        {
            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.timeAppeared < this.timeToAppear && this.fisDrawing)
            {
                spriteBatch.Draw(this.texture2d, this.vecPosition, null,
                    new Color(255, 255, 255, mAlphaValue),
                    0f, Vector2.Zero, this.scale, SpriteEffects.None, 0f);
            }
        }

        // returns false if now we have to stop drawing because time expired
        public Boolean Update(GameTime gameTime)
        {
            // only update if you are actually drawing
            if (this.fisDrawing)
            {
                if (this.timeAppeared < this.timeToAppear / 2)
                {
                    mAlphaValue = 255;
                }
                else
                {
                    mAlphaValue -= (int)(gameTime.ElapsedGameTime.TotalSeconds *
                        this.alphaValueIncrementPerSecond);
                    mAlphaValue = (int)MathHelper.Clamp(mAlphaValue, 0, 255);
                }
                this.timeAppeared += gameTime.ElapsedGameTime.TotalSeconds;

                if (this.timeAppeared > this.timeToAppear)
                {
                    //CancelDrawing();
                    this.fisDrawing = false;
                    return false;
                }
                return true;
            }
            return false;
        }

        public virtual Boolean isAchieving(Football football)
        {
            return false;
        }

        // Also removes it from the current drawn object list
        public void CancelDrawing()
        {
            timeAppeared = timeToAppear + 1;
            this.fisDrawing = false;
            //   if (BonusObject.currentDrawnBonusObjects.Contains(this))
            //  {
            //    currentDrawnBonusObjects.Remove(this);
            // }
        }

        protected void UpdateStatsOnAchieving()
        {
            this.noOfTimesAchieved++;
            this.lastAchievedScore = this.game.stats.current();
        }

        protected void ResetStats()
        {
            this.noOfTimesDrawn = 0;
            this.noOfTimesAchieved = 0;
            this.lastAchievedScore = -1;
            this.lastDrawnScore = -1;
        }

        public virtual Vector2 GetPositionToDraw()
        {
            Vector2 tortn = new Vector2();
            Random rand = new System.Random();
            tortn.X = rand.Next(50, this.game.GameWidth() - 50);
            tortn.Y = rand.Next(100, this.game.GameHeight() - 100);
            return tortn;
        }
    }
}
