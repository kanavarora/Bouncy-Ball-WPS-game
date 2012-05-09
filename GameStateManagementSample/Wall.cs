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
    public abstract class Wall
    {
        protected Texture2D texture;
        protected GameplayScreen game;
        public static int touchesWithoutAnyWall;
        public int touchesWithoutThisWall;
        public static float elasticity = 0.6f;



        public abstract Boolean DidCollide(Football football, Vector2 tempPosition);


        public void Draw(SpriteBatch spriteBatch)
        {
        }

        public void Update(GameTime gameTime)
        {
        }

        public abstract void UpdateSpeed(ref Vector2 speed);

        public virtual void UpdateStatsOnCollisionWithThisWall(GameTime gameTime)
        {
            this.touchesWithoutThisWall = 0;
            touchesWithoutAnyWall = 0;
        }

        public void UpdateStatsOnNoCollisionWithThisWall()
        {
            this.touchesWithoutThisWall++;
        }
    }

    public class LeftWall : Wall
    {
        public LeftWall(Texture2D texture, GameplayScreen game)
        {
            this.texture = texture;
            this.game = game;
        }

        override public Boolean DidCollide(Football football, Vector2 tempPosition)
        {
            if (tempPosition.X < 0)
            {
                return true;
            }
            return false;
        }

        override public void UpdateSpeed(ref Vector2 speed)
        {
            speed.X = -(elasticity * speed.X);
        }
    }

    public class RightWall : Wall
    {
        public RightWall(Texture2D texture, GameplayScreen game)
        {
            this.texture = texture;
            this.game = game;
        }

        override public Boolean DidCollide(Football football, Vector2 tempPosition)
        {
            if (tempPosition.X + (2 * football.radius) > game.GameWidth())
            {
                return true;
            }
            return false;
        }

        override public void UpdateSpeed(ref Vector2 speed)
        {
            speed.X = -(elasticity * speed.X);
        }
    }

    public class TopWall : Wall
    {
        public TopWall(Texture2D texture, GameplayScreen game)
        {
            this.texture = texture;
            this.game = game;
        }
        override public Boolean DidCollide(Football football, Vector2 tempPosition)
        {
            if (tempPosition.Y < 0)
            {
                return true;
            }
            return false;
        }

        override public void UpdateSpeed(ref Vector2 speed)
        {
            speed.Y = -(elasticity * speed.Y);
        }
    }


    public class BottomWall : Wall
    {
        public BottomWall(Texture2D texture, GameplayScreen game)
        {
            this.texture = texture;
            this.game = game;
        }

        override public Boolean DidCollide(Football football, Vector2 tempPosition)
        {
            if (tempPosition.Y + (2 * football.radius) > game.GameHeight())
            {
                return true;
            }
            return false;
        }

        override public void UpdateSpeed(ref Vector2 speed)
        {
            speed.Y = -(elasticity * speed.Y);
        }

        public override void UpdateStatsOnCollisionWithThisWall(GameTime gameTime)
        {
            this.touchesWithoutThisWall = 0;
            touchesWithoutAnyWall = 0;

            if (OptionsMenuScreen.currentGameMode == gameMode.Classic)
            {
                if (this.game.gameStarted)
                    this.game.football.AnimateCrashingFootball(this.game.football.speed,
                        this.game.football.position, this.game.football.scale, this.game.football.radius);
                this.game.resetGameClassic(gameTime);
            }
            else
            {
                //only reset in arcade mode, if we are not touching the wall
                //ie. we just touched the ground for the first time
                if (!this.game.football.inTouchWithWall)
                {
                    this.game.football.AnimateCrashingFootball(this.game.football.speed,
                        this.game.football.position, this.game.football.scale, this.game.football.radius);
                    this.game.stats.subtractPoints(50);
                    this.game.stats.DrawBallCrashingString(this.game.football.position +
                        new Vector2(this.game.football.radius, this.game.football.radius));
                    this.game.resetCommonBonusObjects(gameTime);
                    
                }
            }
        }
    }
}
