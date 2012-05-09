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
    public class SmallBallBonusObject : BonusObject
    {

        public SmallBallBonusObject(GameplayScreen game, Vector2 scale, double timeToAppear) :
            base(Football.ballTexture, game, scale, timeToAppear)
        {
        }

        public override void makeChange(Football football, GameTime gameTime)
        {
            //football.bonusObjectsInEffect.Add(this);
            football.MakeBallSmaller();
            game.stats.DrawStringBallSmaller();
            game.stats.AddCurrentBonusObject("small ball", this.texture2d);
        }

        public override Boolean CanDraw()
        {
            if (this.isDrawing())
            {
                return false;
            }
            /*if (this.lastDrawnScore == -1)
            {
                return (this.game.stats.currentScore > 10);
            }
            else
            {
                return (this.game.stats.currentScore > this.lastDrawnScore + 30);
            }*/
            if (this.game.stats.current() > 2)
            {
                return countOfBonusObject % TOTALBONUSOBJS == 1;
            }
            else return false;
            //return (this.game.stats.currentScore > 10 && !this.isDrawing());
        }

        public override Boolean isAchieving(Football football)
        {
            float radius = this.dimensions.X / 2;
            BoundingSphere bonusball = new BoundingSphere(new Vector3(vecPosition.X + radius, vecPosition.Y + radius, 0), radius);
            BoundingSphere footballsphere = new BoundingSphere(new Vector3(football.position.X + football.radius, football.position.Y + football.radius, 0), football.radius);
            if (bonusball.Intersects(footballsphere))
            {
                UpdateStatsOnAchieving();
                return true;
            }
            return false;
        }
    }
}
