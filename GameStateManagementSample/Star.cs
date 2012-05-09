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
    public class Star
    {
        Texture2D starTexture;
        Vector2 vecPosition;
        Vector2 vecDimensions;
        GameplayScreen game;
        bool fIsDrawing;
        double timeToAppear;
        double timeAppeared;
        Vector2 scale;
        Boolean hasAchieved;

        public static SoundEffect StarAchievedSound;
        public static void LoadContent(ContentManager Content)
        {
            StarAchievedSound = Content.Load<SoundEffect>("Ding");
        }
        public Star(Texture2D starTexture, Vector2 vecPosition, GameplayScreen game)
        {
            this.starTexture = starTexture;
            this.vecPosition = vecPosition;
            this.vecDimensions = new Vector2(50, 50);
            this.game = game;
            this.fIsDrawing = true;
            this.timeToAppear = 5.0;
            this.timeAppeared = 0.0;
            this.hasAchieved = false;
            CalculateScale();
        }

        private void CalculateScale()
        {
            float width = (float)starTexture.Bounds.Width;
            float height = (float)starTexture.Bounds.Height;

            this.scale = new Vector2((this.vecDimensions.X) / width, (this.vecDimensions.Y) / height);
        }

        public void draw(SpriteBatch spriteBatch, int mAlphaValue)
        {
            if (this.timeAppeared < this.timeToAppear && this.fIsDrawing)
            {
                spriteBatch.Draw(this.starTexture, this.vecPosition, null,
                    new Color(255, 255, 255, mAlphaValue),
                    0f, Vector2.Zero, this.scale, SpriteEffects.None, 0f);
            }
        }

        public Boolean isAchieving(Football football)
        {
            float radius = this.vecDimensions.X / 2;
            BoundingSphere starSphere = new BoundingSphere(new Vector3(vecPosition.X + radius, vecPosition.Y + radius, 0), radius);
            BoundingSphere footballsphere = new BoundingSphere(new Vector3(football.position.X + football.radius, football.position.Y + football.radius, 0), football.radius);
            if (starSphere.Intersects(footballsphere))
            {

                return true;
            }
            return false;
        }

        protected void UpdateStatsOnAchieving()
        {
            this.game.stats.addPoints(10);
        }

        public void CancelDrawing()
        {
            this.fIsDrawing = false;
        }

        public Boolean FAchieved()
        {
            return this.hasAchieved;
        }

        public Boolean Update(GameTime gameTime)
        {
            Football football = this.game.football;
            if (this.fIsDrawing && this.isAchieving(football))
            {
                this.fIsDrawing = false;
                this.hasAchieved = true;
                this.UpdateStatsOnAchieving();
                SoundStuff starAchievedSoundStuff = new SoundStuff(StarAchievedSound);
                starAchievedSoundStuff.Play();
            }

            // only update if you are actually drawing
            if (this.fIsDrawing)
            {
                if (this.timeAppeared > this.timeToAppear)
                {
                    //CancelDrawing();
                    this.fIsDrawing = false;
                    return false;
                }
                return true;
            }
            return false;
        }

    }
}
