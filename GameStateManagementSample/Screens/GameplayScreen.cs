#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
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

#endregion

namespace BouncyBall
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
   public class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager Content;
        SpriteFont gameFont;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D background;
        public Football football;

        public Stats stats;
        int height;
        int width;
        public Texture2D ringTexture;

        public List<Wall> walls;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            this.height = 800;//this.GraphicsDevice.Viewport.Height;
            this.width = 480;//this.GraphicsDevice.Viewport.Width;
            this.gameStarted = false;
            stats = new Stats(this);
            football = new Football(this,
                new Vector2(240- Football.InitialFootballRadius, this.GameHeight() - (2 * Football.InitialFootballRadius)),
                new Vector2(0.0f, 0.0f), Football.InitialFootballRadius, stats);

        }


        public int GameHeight()
        {
            return this.height;
        }

        public int GameWidth()
        {
            return this.width;
        }
        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");

            //gameFont = Content.Load<SpriteFont>("gamefont");
            Football.LoadContent(Content);
            football.CalculateScale(); // need to call this after loading content.
            Stats.LoadContent(Content);
            background = Content.Load<Texture2D>("soccer field");

            // load high scores
            HighScoreScreen.LoadHighscore();

            // Bonus Object Load Content
            WindBonusObject.LoadContent(Content);
            RingBonusObject.LoadContent(Content);
            DoubleBonusObject.LoadContent(Content);
            TimeBonusObject.LoadContent(Content);
            //ringTexture = Content.Load<Texture2D>("ring");
            PatternStars.LoadContent(Content);
            //loading walls
            Wall leftWall = new LeftWall(null, this);
            Wall rightWall = new RightWall(null, this);
            Wall topWall = new TopWall(null, this);
            Wall bottomWall = new BottomWall(null, this);
            walls = new List<Wall>();
            walls.Add(leftWall); walls.Add(rightWall); walls.Add(topWall); walls.Add(bottomWall);

            InitializeBonusObjects();
            foreach (BonusObject bo in bonusObjects)
            {
                bo.CalculateScale();
            }

           
            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            //Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

        public SmallBallBonusObject sbbo;
        public WindBonusObject wibo;
        public RingBonusObject ribo;
        public DoubleBonusObject dobo;
        public TimeBonusObject tibo;
        public List<BonusObject> bonusObjects;
        public void InitializeBonusObjects()
        {
            bonusObjects = new List<BonusObject>();
            sbbo = new SmallBallBonusObject( this, new Vector2(40, 40), 5.0);
            wibo = new WindBonusObject( this, new Vector2(50, 50), 5.0);
            ribo = new RingBonusObject(this, new Vector2(230, 75), 10.0);
            dobo = new DoubleBonusObject(this, new Vector2(60, 50), 5.0);
            tibo = new TimeBonusObject(this, new Vector2(40, 40), 5.0);
            bonusObjects.Add(sbbo);
            bonusObjects.Add(wibo);
            bonusObjects.Add(ribo);
            bonusObjects.Add(dobo);
            bonusObjects.Add(tibo);
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            Content.Unload();
        }


        #endregion

        #region Update and Draw

        public Boolean touching = false;
        private Boolean GameStarted;
        public Boolean gameStarted
        {
            get{
                return GameStarted;
            }
            set {
                GameStarted = value;
            }
        }

        private void HandleTouches2(GameTime gameTime)
        {
            TouchCollection touches = TouchPanel.GetState();

            if (!touching && touches.Count > 0)
            {
                touching = true;
                TouchLocation touch = touches.First();
                football.HandleTouch(touch.Position, gameTime);
            }
            else if (touches.Count == 0)
            {
                touching = false;
            }
        }

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {

                // TODO: this game isn't very fun! You could probably improve
                // it by inserting something more interesting in this space :-)*/


                HandleTouches2(gameTime);
                football.Update(gameTime);
                // TODO: Add your update logic here
                stats.Update(gameTime);

                // update bonus objects
                if (gameStarted && !this.football.inTouchWithWall)
                {
                    BonusObject.UpdateBonusObjects(this, gameTime);

                    PatternStars.UpdatePatternStars(gameTime);
                }

            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // if the user pressed the back button, we return to the main menu
            PlayerIndex player;
            if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
            {
                HighScoreScreen.SaveHighscore();
                LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen());
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 2;
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // draw the background
            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);

            //draw football
            football.Draw(spriteBatch, gameTime);

            // draw stats
            stats.draw(spriteBatch, gameTime);

            if (this.gameStarted && !this.football.inTouchWithWall)
            {
                // draw bonus objects
                BonusObject.RenderBonusObjects(this, spriteBatch, gameTime);

                PatternStars.RenderPatternStars(spriteBatch, gameTime, this);
            }

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);
        }

        public void resetGameClassic(GameTime gameTime)
        {
            stats.resetScore(true, gameTime);
            this.gameStarted = false;
            resetCommonBonusObjects(gameTime);
        }
        public void resetGameArcade(GameTime gameTime)
        {
            ScreenManager.AddScreen(new FinalScorePopup("Score: " + this.stats.current(), false), null);
            this.gameStarted = false;
            this.stats.resetScore(true, gameTime);
            resetCommonBonusObjects(gameTime);
        }

        public void resetCommonBonusObjects(GameTime gameTime)
        {
            BonusObject.InvalidateAllDrawingObjects();
            BonusObject.ResetAllBonusObjects(this);
            PatternStars.ResetAllPatternStars();

            football.inTouchWithWall = true;
            // reset bonus effects
            football.ResetFootball();
            football.ResetWindEffectChange();
            stats.ResetDoublePointsChange();
            football.ResetTimeBonusChange(gameTime);
        }
        #endregion
    }
}
