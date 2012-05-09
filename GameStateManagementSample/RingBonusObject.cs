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
    public class RingBonusObject : BonusObject
    {
        public static Texture2D ringTexture;
        public static void LoadContent(ContentManager Content)
        {
            ringTexture = Content.Load<Texture2D>("bballhoop");
        }

        public RingBonusObject(GameplayScreen game, Vector2 scale, double timeToAppear) :
            base(ringTexture, game, scale, timeToAppear)
        {
        }

        public override void makeChange(Football football, GameTime gameTime)
        {
            this.game.stats.addPoints(100);
            this.game.stats.DrawThroughTheLoopString("+100 Through the basket!!!");
        }

        public override Vector2 GetPositionToDraw()
        {
            int x = (int)((this.game.GameWidth() / 2) - (this.dimensions.X / 2));
            Random rand = new System.Random();
            int y = rand.Next(250, this.game.GameHeight() - 250);
            return new Vector2(x, y);
        }

        public override Boolean CanDraw()
        {
            if (this.isDrawing())
            {
                return false;
            }
            /*if (this.lastDrawnScore == -1)
            {
                return (this.game.stats.currentScore > 2);
            }
            else
            {
                return (this.game.stats.currentScore > this.lastDrawnScore + 10);
            }*/
            if (this.game.stats.current() > 2)
            {
                return countOfBonusObject % TOTALBONUSOBJS == 2;
            }
            else return false;
            //return (this.game.stats.currentScore > 10 && !this.isDrawing());
        }


        public override Boolean isAchieving(Football football)
        {
            if (football.speed.Y < 0)
            {
                return false;
            }
            //int yOfLoop = (int)(this.vecPosition.Y + (this.dimensions.Y / 2));
            int yOfLoop = (int)this.vecPosition.Y;
            int midYFootball = (int)(football.position.Y + football.radius);
            int lowYFootballBound = (int)(midYFootball - (0.3 * football.radius));
            int highYFootballBound = (int)(midYFootball + (0.3 * football.radius));

            if (yOfLoop > lowYFootballBound && yOfLoop < midYFootball)
            {
                int lowXBoundLoop = (int)this.vecPosition.X;
                int highXBoundLoop = (int)this.vecPosition.X + (int)this.dimensions.X;

                int lowXBoundFootball = (int)football.position.X;
                int highXBoundFootball = (int)(football.position.X + (football.radius * 2));
                if (lowXBoundFootball > lowXBoundLoop && highXBoundFootball < highXBoundLoop)
                {
                    UpdateStatsOnAchieving();
                    return true;
                }
            }
            return false;
        }
    }
}
