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

    public class Stats
    {
        public static int highScore = 0;
        protected int currentScore;

        public static SpriteFont spriteFontCurrent;
        public static SpriteFont spriteFontHigh;
        public GameplayScreen game;

        public double gameStartTime;

        StaticString bonusStringDraw; // for the bonus line

        public static void LoadContent(ContentManager Content)
        {
            spriteFontCurrent = Content.Load<SpriteFont>("Current");
            spriteFontHigh = Content.Load<SpriteFont>("HighScore");
        }
        public AnywhereStaticString randomPositionStrings;
        public Stats(/*SpriteFont spriteFontCurrent, SpriteFont spriteFontHigh, */GameplayScreen game)
        {
            currentScore = 0;
            //this.spriteFontCurrent = spriteFontCurrent;
            //this.spriteFontHigh = spriteFontHigh;
            this.game = game;
            bonusStringDraw = new StaticString(this.game);
            randomPositionStrings = new AnywhereStaticString();
            currentBonusObjects = new Dictionary<string, Texture2D>();
            isUnderTimeBonus = false;
            Stats.highScore = HighScoreScreen.HighestScore(OptionsMenuScreen.currentGameMode, 
                OptionsMenuScreen.currentGameDifficulty);
             
        }

        public void resetScore(Boolean fUpdate, GameTime gameTime)
        {
            if (fUpdate)
            {
                if (currentScore > highScore)
                {
                    Stats.highScore = this.currentScore;
                }

                HighScoreScreen.PutHighScore("Kanav", this.currentScore, 
                        OptionsMenuScreen.currentGameMode, OptionsMenuScreen.currentGameDifficulty);


            }
            currentScore = 0;
            this.gameStartTime = gameTime.TotalGameTime.TotalSeconds;
        }

        public void DrawBallCrashingString(Vector2 vecPosition)
        {
            StaticString ss = new StaticString(this.game, "-50",
                1.0, Color.Red, spriteFontCurrent, vecPosition);
            randomPositionStrings.AddStaticString(ss);
        }

        public void DrawThroughTheLoopString(string achievment)
        {
            bonusStringDraw.drawNewString(achievment, 1.0,
               PositionOfDisplay.center, Color.White, spriteFontCurrent);
        }

        public void DrawAllStarsAchievementString(string achievment)
        {
            bonusStringDraw.drawNewString(achievment, 1.0,
                PositionOfDisplay.center, Color.White, spriteFontCurrent);
        }

        public void DrawStringBallSmaller()
        {
            bonusStringDraw.drawNewString("Ball shrunk!!!", 1.0,
                    PositionOfDisplay.center, Color.White, spriteFontCurrent);
        }

        public void DrawWindRight()
        {
            bonusStringDraw.drawNewString("Watch the wind!!!", 1.0,
                PositionOfDisplay.center, Color.White, spriteFontCurrent);
        }

        public void DrawDoublePoints()
        {
            bonusStringDraw.drawNewString("Double Points", 1.0,
                PositionOfDisplay.center, Color.White, spriteFontCurrent);
        }

        public void DrawSkillShot()
        {
            bonusStringDraw.drawNewString("+50  Skill Shot!!!!", 1.0,
                PositionOfDisplay.center, Color.White, spriteFontCurrent);
        }

        public void DrawTimeFreeze()
        {
            bonusStringDraw.drawNewString("Time Freeze", 1.0,
               PositionOfDisplay.center, Color.White, spriteFontCurrent);
        }

        public void DrawBonusStringWithoutAnyWallsTouched(int touchesWithoutWall)
        {
            int addScore = 5 * touchesWithoutWall;

            if (touchesWithoutWall > 3)
            {
                bonusStringDraw.drawNewString("No wall bonus: + " + addScore, 1.0,
                    PositionOfDisplay.center, Color.White, spriteFontCurrent);
                this.currentScore += addScore;
            }
        }



        // touch on the ball, change the current score and update touches without wall.
        public int addKick()
        {
            currentScore++;
            return currentScore;
        }

        public int addPoints(int pointToBeAdded)
        {
            currentScore += pointToBeAdded;
            return currentScore;
        }

        public int subtractPoints(int pointsToBeSubtracted)
        {
            currentScore -= pointsToBeSubtracted;
            if (currentScore < 0)
            {
                currentScore = 0;
            }
            return currentScore;
        }

        public int current()
        {
            return currentScore;
        }

        public void draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            drawCurrentScore(spriteBatch);
            if (OptionsMenuScreen.currentGameMode == gameMode.Classic)
            {
                drawHighScore(spriteBatch);
            }
            else
            {
                drawTimeLeft(spriteBatch, gameTime);
            }
            bonusStringDraw.Draw(spriteBatch);
            randomPositionStrings.Draw(spriteBatch);
            drawCurrentBonusObjects(spriteBatch);

        }


        public void Update(GameTime gameTime)
        {
            bonusStringDraw.Update(gameTime);
            randomPositionStrings.Update(gameTime);
            UpdateDoublePointsChange(gameTime);
            // check to see if arcade game finished
            if (OptionsMenuScreen.currentGameMode == gameMode.Arcade)
            {
                int timeLeft = 60;
                if (this.game.gameStarted)
                {
                    timeLeft = (int)(60 - (int)(gameTime.TotalGameTime.TotalSeconds - this.gameStartTime));
                }
                if (timeLeft < 0)
                {
                    this.game.resetGameArcade(gameTime);
                }
            }
            updateTimeBonusChange(gameTime);
        }

        protected void drawCurrentScore(SpriteBatch spriteBatch)
        {
            string currentScoreString = "Bounces: " + this.currentScore;
            spriteBatch.DrawString(spriteFontHigh, currentScoreString, new Vector2(2, 0), Color.White);

        }

        protected void drawHighScore(SpriteBatch spriteBatch)
        {
            string highScoreString = "HighScore: " + Stats.highScore;
            Vector2 positionOfScore = new Vector2(this.game.GameWidth() - spriteFontHigh.MeasureString(highScoreString).X, 0);
            spriteBatch.DrawString(spriteFontHigh, highScoreString, positionOfScore, Color.White);
            
        }
        protected int timeLeftBeforeTimeBonus;
        protected Boolean isUnderTimeBonus;
        protected double timeBonusAchieved;
        public void makeTimeBonusChange(GameTime gameTime)
        {
            isUnderTimeBonus = true;
            timeBonusAchieved = gameTime.TotalGameTime.TotalSeconds;
            int timeLeft = 60;
            if (this.game.gameStarted)
            {
                timeLeft = (int)(60 - (int)(gameTime.TotalGameTime.TotalSeconds - this.gameStartTime));
            }
            if (timeLeft < 0)
            {
                timeLeft = 0;
            }
            timeLeftBeforeTimeBonus = timeLeft;
        }
        public void updateTimeBonusChange(GameTime gameTime)
        {
            if (isUnderTimeBonus)
            {
                if (timeBonusAchieved + 5.0 < gameTime.TotalGameTime.TotalSeconds)
                {
                    ResetTimeBonusChange(gameTime);
                }
            }
        }
        public void ResetTimeBonusChange(GameTime gameTime)
        {
            if (this.isUnderTimeBonus)
            {
                this.isUnderTimeBonus = false;
                this.gameStartTime += (gameTime.TotalGameTime.TotalSeconds - this.timeBonusAchieved);
            }
        }

        protected void drawTimeLeft(SpriteBatch spriteBatch, GameTime gameTime)
        {
            int timeLeft = 60;

            if (this.game.gameStarted)
            {
                if (!this.isUnderTimeBonus)
                {
                    timeLeft = (int)(60 - (int)(gameTime.TotalGameTime.TotalSeconds - this.gameStartTime));
                }
                else
                {
                    timeLeft = timeLeftBeforeTimeBonus;
                }
            }
            if (timeLeft < 0)
            {
                timeLeft = 0;
            }
            string timeLeftString = "Time: " + timeLeft;
            Vector2 positionOfTime = new Vector2(this.game.GameWidth() - spriteFontHigh.MeasureString(timeLeftString).X, 0);
            spriteBatch.DrawString(spriteFontHigh, timeLeftString, positionOfTime, Color.White);
        }

        public Dictionary<string, Texture2D> currentBonusObjects;
        protected static int bonusobjheight=25;
        protected static int bonusobjwidth=25;
        protected void drawCurrentBonusObjects(SpriteBatch spriteBatch)
        {
            int startX = this.game.GameWidth() - 10- bonusobjwidth;
            int y = 30;
            foreach (KeyValuePair<string, Texture2D> kv in currentBonusObjects)
            {
                float width = (float)kv.Value.Bounds.Width;
                float height = (float)kv.Value.Bounds.Height;
                Vector2 scale = new Vector2(bonusobjwidth/width, bonusobjheight/height);
                spriteBatch.Draw(kv.Value, new Vector2(startX, y), null,
                    Color.White,
                    0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                startX -= 10+bonusobjheight;
            }
        }

        public void AddCurrentBonusObject(string key, Texture2D texture)
        {
            if (!currentBonusObjects.ContainsKey(key))
            {
                currentBonusObjects.Add(key, texture);
            }
        }

        public void RemoveCurrentBonusObject(string key)
        {
            if (currentBonusObjects.ContainsKey(key))
            {
                currentBonusObjects.Remove(key);
            }
        }
        public void RemoveAllCurrentBonusObjects()
        {
            currentBonusObjects = new Dictionary<string, Texture2D>();
        }

        protected double timeDoubleInEffect;
        protected double timeTotalDoubleInEffect;
        protected Boolean DoublePointsinEffect;
        protected int initialScore;
        public void HandleDoublePointsChange(GameTime gameTime)
        {
            this.timeTotalDoubleInEffect = 5.0;
            this.timeDoubleInEffect = 0.0;
            this.DoublePointsinEffect = true;
            this.initialScore = this.currentScore;
        }

        public void UpdateDoublePointsChange(GameTime gameTime)
        {
            if (this.DoublePointsinEffect)
            {
                this.timeDoubleInEffect += gameTime.ElapsedGameTime.TotalSeconds;

                if (this.timeDoubleInEffect > this.timeTotalDoubleInEffect)
                {
                    int ScoreInBetween = this.currentScore - this.initialScore;
                    this.currentScore += 2 * ScoreInBetween;
                    // TODO: Add some animation here to show how much score.
                    ResetDoublePointsChange();
                }
            }
        }

        public void ResetDoublePointsChange()
        {
            this.DoublePointsinEffect = false;
            this.timeDoubleInEffect = this.timeTotalDoubleInEffect + 1.0;
            RemoveCurrentBonusObject("double");
        }
    }
}
