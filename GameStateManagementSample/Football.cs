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
    public class Football
    {
        public static Texture2D ballTexture;
        public Vector2 position;
        public Vector2 speed;
        Vector2 acceleration;
        public float radius;
        public float scale;
        GameplayScreen game;
        Stats stats;
        public static int InitialFootballRadius = 65; 

        public static Vector2 gravityAcceleration = new Vector2(0.0f, 2.0f);
        

        //bonus object variables
        public List<BonusObject> bonusObjectsInEffect;
        protected Boolean isBallSmallerChange;
        protected double timeBallSmallerChange;

        protected Boolean isWindInEffect;
        protected double timeWindInEffect;


        //public static Texture2D ballTexture;
        public static SoundEffect footballKickSound;
        public static SoundEffect footballShrunkSound;
        public static SoundEffect windSound;
        public static SoundEffect footballCrashingSound;
        public static Texture2D puddleTexture;
        public static void LoadContent(ContentManager Content){
            ballTexture = Content.Load<Texture2D>("soccer ball");
            footballKickSound = Content.Load<SoundEffect>("ballkick");
            footballShrunkSound = Content.Load<SoundEffect>("shrunkball");
            windSound = Content.Load<SoundEffect>("wind");
            footballCrashingSound = Content.Load<SoundEffect>("footballground");
            puddleTexture = Content.Load<Texture2D>("water puddle");
        }

        public Football(GameplayScreen game, Vector2 startPosition, Vector2 startSpeed, float radius, Stats stats)
        {
            this.game = game;
            position = startPosition;
            speed = startSpeed;
            acceleration = gravityAcceleration;
            this.radius = radius;
            this.stats = stats;
            maxDistance = 2.4f * radius;
            this.bonusObjectsInEffect = new List<BonusObject>();
            this.isBallSmallerChange = false;
            this.timeBallSmallerChange = 0.0;
            this.isWindInEffect = false;
            this.timeWindInEffect = 0.0;
            //CalculateScale();
            this.inTouchWithWall = true;
        }

        
         public void CalculateScale()  
        {  
            float width = (float)ballTexture.Bounds.Width;  
            this.scale = (this.radius * 2) / width;  
        }  

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!this.isCrashing)
            {
                spriteBatch.Draw(ballTexture, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            //animate crashing football
            drawAnimateCrashingFootball(spriteBatch, gameTime);
        }

        public float maxChangeInVelocity = 36.0f;
        public float minChangeInVelocity = 20.0f;
        public float maxDistance;
        public float hardleeway = 0.10f;
        public float easyleeway = 0.15f;
        private Boolean InTouchWithWall;

        public Boolean inTouchWithWall
        {
            get
            {
                return InTouchWithWall;
            }
            set
            {
                InTouchWithWall = value;
            }
        }

        public void HandleTouch(Vector2 touchPosition, GameTime gameTime)
        {
            if (this.isCrashing)
            {
                return;
            }

            float leeway = easyleeway;
            if (OptionsMenuScreen.currentGameDifficulty == gameDifficulty.Hard)
                leeway = hardleeway;

            Vector2 center = this.position + new Vector2(radius, radius);
            Vector2 displacement = center - touchPosition;
            if (displacement.Length() < (radius*(1+(leeway*0.95))))
            {
                Vector2 top = this.position + new Vector2(radius, -(leeway*radius));
                Vector2 bottom = this.position + new Vector2(radius, (2+leeway)* radius);
                Vector2 disp = top - touchPosition;
                float displength =  disp.Length();
                float x = (2+(leeway*2)) * radius;
                float tmp = (minChangeInVelocity + ((maxChangeInVelocity-minChangeInVelocity)*(displength/x)));
                this.speed = (disp/(displength) )* (tmp);
                float len = speed.Length();
                playBallKickSound();
                UpdateScoreOnTouch();
                // if the ball is in touch with the wall,
                // start the game and make the ball not touch with wall.
                // reset the score only if the game is classic mode
                if (this.inTouchWithWall)
                {
                    
                    if (OptionsMenuScreen.currentGameMode == gameMode.Classic 
                        || !this.game.gameStarted)
                    {
                        this.game.stats.resetScore(false, gameTime);
                    }
                    this.game.gameStarted = true;
                    this.inTouchWithWall = false;
                }
            }

            if (displacement.Length() < (radius * (0.1)))
            {
                UpdateScoreOnSkillShot();
            }
        }


        protected Vector2 speedCrashing;
        protected Vector2 positionCrashing;
        protected float scaleCrashing;
        protected Boolean isCrashing;
        protected float radiusCrashing;
        static float timeOfCrash = 0.75f;
        protected int crashAlphaValue;
        protected float crashAlphaDecrementPerSecond;
        public void AnimateCrashingFootball(Vector2 speedBeforeCrashing, Vector2 positionBeforeCrashing,
            float scaleBeforeCrashing, float radiusBeforeCrashing)
        {
            speedCrashing = speedBeforeCrashing*0.5f;
            positionCrashing = positionBeforeCrashing;
            scaleCrashing = scaleBeforeCrashing;
            radiusCrashing = radiusBeforeCrashing;
            isCrashing = true;

            positionCrashing.Y = this.game.GameHeight() - 2 * (radiusCrashing);
            crashAlphaValue = 200;
            crashAlphaDecrementPerSecond = 200f / timeOfCrash;
            SoundStuff footballCrashingSoundStuff = new SoundStuff(footballCrashingSound);
            footballCrashingSoundStuff.Play();
        }
        protected void drawAnimateCrashingFootball(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //float maxDiffY = (float)2 * Football.InitialFootballRadius;
            //float scaleOfPuddleY = (this.positionCrashing.Y - this.game.GameHeight() + maxDiffY) / maxDiffY;
            //float scaleOfPuddleX = (float)((2.0f * (float)Football.InitialFootballRadius) /(float) puddleTexture.Width);
            if (this.isCrashing)
            {
                
                spriteBatch.Draw(ballTexture, positionCrashing, null,
                    new Color(255,255,255, crashAlphaValue), 0f, Vector2.Zero, scaleCrashing, SpriteEffects.None, 0f);
                
            }
            //spriteBatch.Draw(puddleTexture, new Vector2(positionCrashing.X, this.game.GameHeight() - puddleTexture.Height),
                  //  null, Color.White, 0f, Vector2.Zero, new Vector2(scaleOfPuddleX, scaleOfPuddleY), SpriteEffects.None, 0f);
        }
        protected void updateAnimateCrashingFootball(GameTime gameTime)
        {
            if (this.isCrashing)
            {
                //this.positionCrashing += this.speedCrashing;
                this.crashAlphaValue -= (int)(gameTime.ElapsedGameTime.TotalSeconds * crashAlphaDecrementPerSecond);
                //if (this.positionCrashing.Y >= this.game.GameHeight())
                if (this.crashAlphaValue < 0)
                {
                    this.isCrashing = false;
                }
            }
        }

        protected void UpdateScoreOnSkillShot()
        {
            this.stats.addPoints(50);
            this.stats.DrawSkillShot();
        }

        private void playBallKickSound()
        {
            SoundStuff footballKickSoundStuff = new SoundStuff(footballKickSound);
            footballKickSoundStuff.Play();
            //footballKickSound.Play();
        }

        private void playBallShrunkSound()
        {
            SoundStuff footballShrunkSoundStuff = new SoundStuff(footballShrunkSound);
            footballShrunkSoundStuff.Play();
            //footballShrunkSound.Play();
        }

        // updates currentscore on touch
        protected void UpdateScoreOnTouch()
        {
            List<Wall> walls = this.game.walls;
            for (int i = 0; i < walls.Count; i++)
            {
                Wall currentWall = walls.ElementAt(i);
                currentWall.UpdateStatsOnNoCollisionWithThisWall();
            }
            stats.addKick();
            Wall.touchesWithoutAnyWall++;
        }



        public void Update(GameTime gameTime)
        {
            Vector2 tempFinalPosition = calculateDistance(speed, acceleration);
            List<Wall> walls = game.walls;
            Wall collidedWith = null;
            for (int i = 0; i < walls.Count; i++)
            {
                Wall currentWall = walls.ElementAt(i);
                if (currentWall.DidCollide(this, tempFinalPosition))
                {
                    // fix this
                    collidedWith = currentWall;
                    break;
                }
            }
            if (collidedWith == null)
            {
                UpdateNoCollision(tempFinalPosition);
            }
            else
            {
                // if collided with some wall
                stats.DrawBonusStringWithoutAnyWallsTouched(Wall.touchesWithoutAnyWall);
                collidedWith.UpdateStatsOnCollisionWithThisWall(gameTime);
                collidedWith.UpdateSpeed(ref this.speed);
            }

            //animate crashing football
            updateAnimateCrashingFootball(gameTime);

            //updates on bonus objects
            UpdateSmallBallChange(gameTime);
            UpdateWindEffectChange(gameTime);
        }


        private void UpdateNoCollision(Vector2 tempFinalPostion)
        {
            position = tempFinalPostion;
            speed = calculateSpeed(speed, acceleration);
            
        }

        private Vector2 calculateDistance(Vector2 u, Vector2 a)
        {
            Vector2 s;
            s = this.position + u + new Vector2(0.5f, 0.5f)*(a);
            return s;
        }

        private Vector2 calculateSpeed(Vector2 u, Vector2 a)
        {
            Vector2 v;
            v = u + a;
            return v;
        }

        public void ResetFootball()
        {
            this.isBallSmallerChange = false;
            this.timeBallSmallerChange = 0.0;
            bonusObjectsInEffect = new List<BonusObject>();
            this.position = new Vector2(240 - Football.InitialFootballRadius, this.game.GameHeight() - (2 * Football.InitialFootballRadius));
            this.speed = new Vector2(0.0f, 0.0f);
            this.inTouchWithWall = true;
            ResetRadius();
        }

        public void ResetRadius()
        {
            this.radius = Football.InitialFootballRadius;
            CalculateScale();
            Vector2 bottomright = new Vector2(this.position.X + (2 * this.radius), 
                this.position.Y + (2 * this.radius));

            if (bottomright.X >= this.game.GameWidth())
            {
                this.position.X -= bottomright.X - this.game.GameWidth() - 2;
            }
            if (bottomright.Y >= this.game.GameHeight())
            {
                this.position.Y -= bottomright.Y - this.game.GameHeight() - 2;
            }
            this.game.stats.RemoveCurrentBonusObject("small ball");
        }

        
        public void MakeBallSmaller()
        {
            this.isBallSmallerChange = true;
            this.timeBallSmallerChange = 0.0;
            this.radius = this.radius * (0.80f);
            playBallShrunkSound();
            CalculateScale();
        }

        protected void UpdateSmallBallChange(GameTime gameTime)
        {
            if (this.isBallSmallerChange)
            {
                this.timeBallSmallerChange += gameTime.ElapsedGameTime.TotalSeconds;

                if (this.timeBallSmallerChange > 4.0)
                {
                    this.isBallSmallerChange = false;
                    ResetRadius();
                }
            }
        }

        
        public void WindEffect()
        {
            this.isWindInEffect = true;
            this.timeWindInEffect = 0.0;
            if (this.windSoundEffectInstance == null)
            {
                windSoundEffectInstance = windSound.CreateInstance();
                windSoundEffectInstance.IsLooped = true;
            }
            windSoundEffectInstance.Play();
        }
        protected SoundEffectInstance windSoundEffectInstance;
        protected void UpdateWindEffectChange(GameTime gameTime)
        {
            if (this.isWindInEffect)
            {
                this.timeWindInEffect += gameTime.ElapsedGameTime.TotalSeconds;
                this.speed.X = this.speed.X + 0.3f;
                if (this.timeWindInEffect > 5.0)
                {
                    //windSoundEffectInstance.Stop();
                   // this.isWindInEffect = false;
                    ResetWindEffectChange();
                }
            }
        }

        public void ResetWindEffectChange()
        {
            this.isWindInEffect = false;
            if (this.windSoundEffectInstance != null)
            {
                windSoundEffectInstance.Stop();
                
            }
            this.game.stats.RemoveCurrentBonusObject("wind");
        }

        public void DoublePointsChange(GameTime gameTime)
        {
            this.game.stats.HandleDoublePointsChange(gameTime);
        }

        public void TimeBonusChange(GameTime gameTime)
        {
            this.game.stats.makeTimeBonusChange(gameTime);
        }
        public void ResetTimeBonusChange(GameTime gameTime)
        {
            this.game.stats.ResetTimeBonusChange(gameTime);
            this.game.stats.RemoveCurrentBonusObject("time bonus");
        }
    }
}
