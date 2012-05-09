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
    public class SoundStuff
    {
        SoundEffect soundEffect;

        public SoundStuff(SoundEffect soundEffect)
        {
            this.soundEffect = soundEffect;
        }

        public void Play()
        {
            if (OptionsMenuScreen.soundEffects)
                this.soundEffect.Play();
        }
    }
}
