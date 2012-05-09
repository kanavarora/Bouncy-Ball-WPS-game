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
    public enum Pattern
    {
        random,
        randomrow,
        FourSquare,
        NineSquare,
        doubleDiagnol,
        lastPattern,
    }


    public class PatternStars
    {
        List<Star> stars;
        //Texture2D starTexture;
        Pattern pattern;
        GameplayScreen game;
        protected double timeToAppear;
        protected double timeAppeared;

        // for shading out an object
        protected int mAlphaValue;
        protected int alphaValueIncrementPerSecond;

        public static Texture2D starTexture;
        public static void LoadContent(ContentManager Content)
        {
            starTexture = Content.Load<Texture2D>("star");
            Star.LoadContent(Content);
        }

        public PatternStars(Pattern pattern, GameplayScreen game, double timeToAppear)
        {
            stars = new List<Star>();
            this.game = game;
            this.pattern = pattern;
            this.timeAppeared = 0.0;
            this.timeToAppear = timeToAppear;
            this.mAlphaValue = 255;
            this.alphaValueIncrementPerSecond = (int)((2 * 255) / this.timeToAppear);
            InitiatePattern();
        }

        protected void InitiatePattern()
        {
            switch (this.pattern)
            {
                case Pattern.random:
                    {
                        Vector2 randVector = new Vector2();
                        Random rand = new System.Random();
                        randVector.X = rand.Next(10, this.game.GameWidth() - 10);
                        randVector.Y = rand.Next(20, this.game.GameHeight() - 20);
                        Star randStar = new Star(starTexture, randVector, this.game);
                        stars.Add(randStar);
                        break;
                    }
                case Pattern.randomrow:
                    {
                        Vector2 randVector = new Vector2();
                        Random rand = new System.Random();
                        randVector.Y = rand.Next(30, this.game.GameHeight() - 30);
                        int i = 0;
                        for (int x = this.game.GameWidth() / 4; i < 3; i++, x = x + (this.game.GameWidth() / 4))
                        {
                            randVector.X = x;
                            stars.Add(new Star(starTexture, randVector, this.game));
                        }
                        break;
                    }
                case Pattern.FourSquare:
                    {
                        stars.Add(new Star(starTexture, new Vector2(100, 125), this.game));
                        stars.Add(new Star(starTexture, new Vector2(100, this.game.GameHeight() - 375), this.game));
                        stars.Add(new Star(starTexture, new Vector2(this.game.GameWidth() - 150, this.game.GameHeight() - 375), this.game));
                        stars.Add(new Star(starTexture, new Vector2(this.game.GameWidth() - 150, 125), this.game));
                        break;
                    }
                case Pattern.NineSquare:
                    {
                        Vector2 vecPos = new Vector2();
                        int x = 100;
                        int y = 250;
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                vecPos.X = x + (100 * i);
                                vecPos.Y = y + (100 * j);
                                stars.Add(new Star(starTexture, vecPos, this.game));
                            }
                        }
                        break;
                    }
                case Pattern.doubleDiagnol:
                    {
                        Vector2 vecPos = new Vector2();
                        int x = (int)(this.game.GameWidth() / 2) - 25;
                        int y = (int)(this.game.GameHeight() / 2) - 25 ;
                        vecPos.X = x; vecPos.Y = y; stars.Add(new Star(starTexture, vecPos, this.game));
                        int xinc = 75;
                        int yinc = 125;
                        for (int i = 1; i < 3; i++)
                        {
                            stars.Add(new Star(starTexture, new Vector2(x + i*xinc, y + i*yinc), this.game));
                            stars.Add(new Star(starTexture, new Vector2(x + i * xinc, y - i * yinc), this.game));
                            stars.Add(new Star(starTexture, new Vector2(x - i * xinc, y - i * yinc), this.game));
                            stars.Add(new Star(starTexture, new Vector2(x - i * xinc, y + i * yinc), this.game));
                        }
                        break;
                    }

            }

        }

        public void draw(SpriteBatch spriteBatch)
        {
            foreach (Star star in stars)
            {
                star.draw(spriteBatch, mAlphaValue);
            }
        }

        protected void UpdateStatsOnAchievingAllStars()
        {
            switch (this.pattern)
            {
                case (Pattern.random):
                    {
                        //this.game.stats.DrawAllStarsAchievementString("+20 All stars combo");
                        //this.game.stats.currentScore += 20;
                        break;
                    }

                case (Pattern.randomrow):
                    {
                        this.game.stats.DrawAllStarsAchievementString("+20 All stars combo");
                        this.game.stats.addPoints(20);
                        break;
                    }
                case (Pattern.FourSquare):
                    {
                        this.game.stats.DrawAllStarsAchievementString("+40 All stars combo");
                        this.game.stats.addPoints(40);
                        break;
                    }
                case (Pattern.NineSquare):
                    {
                        this.game.stats.DrawAllStarsAchievementString("+90 All stars combo");
                        this.game.stats.addPoints(90);
                        break;
                    }
                case (Pattern.doubleDiagnol):
                    {
                        this.game.stats.DrawAllStarsAchievementString("+90 All stars combo");
                        this.game.stats.addPoints(90);
                        break;
                    }
            }
        }

        public void Update(GameTime gameTime)
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
            Boolean hasAchievedAll = true;
            foreach (Star star in stars)
            {
                star.Update(gameTime);
                if (!star.FAchieved())
                {
                    hasAchievedAll = false;
                }
            }
            if (hasAchievedAll)
            {
                UpdateStatsOnAchievingAllStars();
                // TODO: fix this later
                this.timeAppeared = this.timeToAppear + 1;
            }
        }

        public void ResetAllStars()
        {
            foreach (Star star in stars)
            {
                star.CancelDrawing();
            }
        }

        public Boolean FExpiredTime()
        {
            return this.timeAppeared > this.timeToAppear;
        }



        protected static Pattern IntToPattern(int i)
        {
           /* if (i == 0)
            {
                return Pattern.random;
            }
            if (i == 1)
            {
                return Pattern.randomrow;
            }
            if (i == 2)
            {
                return Pattern.FourSquare;
            }
            if (i == 3)
            {
                return Pattern.NineSquare;
            }*/
            return (Pattern)i;
            //return Pattern.random;
        }

        public static PatternStars currentPatternStars;
        //public static double lastShownGameTime;
        public static int currentPatternInteger = 0;
        public static void RenderPatternStars(SpriteBatch spriteBatch, GameTime gameTime,
            GameplayScreen game)
        {

            if (currentPatternStars == null && gameTime.TotalGameTime.TotalSeconds > game.stats.gameStartTime + 1.0)
            {
                //lastShownGameTime = gameTime.TotalGameTime.TotalSeconds;
                currentPatternStars = new PatternStars(IntToPattern(currentPatternInteger),
                    game, 5.0);
                currentPatternInteger = (currentPatternInteger + 1) % (int)(Pattern.lastPattern);
            }
            if (currentPatternStars != null)
            {
                currentPatternStars.draw(spriteBatch);
            }
        }

        public static void UpdatePatternStars(GameTime gameTime)
        {
            if (currentPatternStars != null)
            {
                currentPatternStars.Update(gameTime);
                if (currentPatternStars.FExpiredTime())
                {
                    currentPatternStars = null;
                }
            }
        }

        public static void ResetAllPatternStars()
        {
            if (currentPatternStars != null)
            {
                currentPatternStars.ResetAllStars();
                currentPatternStars = null;
            }
        }
    }
}
