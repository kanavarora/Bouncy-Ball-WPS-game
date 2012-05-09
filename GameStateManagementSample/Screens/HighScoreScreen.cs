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
using System.IO;
using System.IO.IsolatedStorage;


namespace BouncyBall
{
    public class HighScoreScreen: GameScreen
    {
        const int highscorePlaces = 10;
        // the number of pixels to pad above and below menu entries for touch input
        const int menuEntryPadding = 10;

        public static KeyValuePair<string, Int32>[][] highScore = new KeyValuePair<string, Int32>[4][];
        public static KeyValuePair<string, Int32>[] defaultHighScore = new KeyValuePair<string, Int32>[highscorePlaces]
{
    
    new KeyValuePair<string,Int32>
        ("Jasper",0),
    new KeyValuePair<string,Int32>
        ("Jasper",0),
        new KeyValuePair<string,Int32>
        ("Jasper",0),
        new KeyValuePair<string,Int32>
        ("Jasper",0),
        new KeyValuePair<string,Int32>
        ("Jasper",0),
        new KeyValuePair<string,Int32>
        ("Jasper",0),
    new KeyValuePair<string,Int32>
        ("Jasper",0),
        new KeyValuePair<string,Int32>
        ("Jasper",0),
        new KeyValuePair<string,Int32>
        ("Jasper",0),
        new KeyValuePair<string,Int32>
        ("Jasper",0)
};
        SpriteFont highScoreFont;
        
       MenuEntry gameModeMenuEntry;
       MenuEntry gameDifficultyMenuEntry;
       static gameMode currentGameMode = gameMode.Arcade;
       static gameDifficulty currentGameDifficulty = gameDifficulty.Easy;

        public HighScoreScreen()
        {
            EnabledGestures = GestureType.Tap;
           gameModeMenuEntry = new MenuEntry(string.Empty);
            gameDifficultyMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            // event handlers
            // Hook up menu event handlers.
            gameModeMenuEntry.Selected += gameModeMenuEntrySelected;
            gameDifficultyMenuEntry.Selected += gameDifficultyMenuEntrySelected;
        }

        
        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            gameModeMenuEntry.Text = "Mode: " + currentGameMode;
            gameDifficultyMenuEntry.Text = "Difficulty: " + currentGameDifficulty;
        }

        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void gameModeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentGameMode++;

            if (currentGameMode > gameMode.Arcade)
                currentGameMode = 0;

            SetMenuEntryText();
        }


        void gameDifficultyMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentGameDifficulty++;
            if (currentGameDifficulty > gameDifficulty.Hard)
                currentGameDifficulty = 0;

            SetMenuEntryText();
        }
        

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            highScoreFont = content.Load<SpriteFont>("menufont");

            // load high scores
            HighScoreScreen.LoadHighscore();

            base.LoadContent();
            
        }

        /// <summary>
        /// Allows the screen to create the hit bounds for a particular menu entry.
        /// </summary>
        Rectangle GetMenuEntryHitBounds(MenuEntry entry)
        {
            // the hit bounds are the entire width of the screen, and the height of the entry
            // with some additional padding above and below.
            return new Rectangle(
                (int)entry.Position.X,
                (int)entry.Position.Y - menuEntryPadding,
                entry.GetWidth(this) + (menuEntryPadding *2),
                entry.GetHeight(this) + (menuEntryPadding * 2));
        }


        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.IsPauseGame(null))
            {
                Exit();
            }

             // look for any taps that occurred and select any entries that were tapped
            foreach (GestureSample gesture in input.Gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    // convert the position to a Point that we can test against a Rectangle
                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);

                    // select the entry. since gestures are only available on Windows Phone,
                    // we can safely pass PlayerIndex.One to all entries since there is only
                    // one player on Windows Phone.
                        if (GetMenuEntryHitBounds(gameModeMenuEntry).Contains(tapLocation))
                        {
                            gameModeMenuEntry.OnSelectEntry(PlayerIndex.One);
                        }
                        if (GetMenuEntryHitBounds(gameDifficultyMenuEntry).Contains(tapLocation))
                        {
                            gameDifficultyMenuEntry.OnSelectEntry(PlayerIndex.One);
                        }
                    }
                }
            /*
            // Return to main menu when tap on the phone
            if (input.Gestures.Count > 0)
            {
                GestureSample sample = input.Gestures[0];
                if (sample.GestureType == GestureType.Tap)
                {
                    Exit();

                    input.Gestures.Clear();
                }
            }*/
        }

        private void Exit()
        {
            this.ExitScreen();
            ScreenManager.AddScreen(new BackgroundScreen(), null);
            ScreenManager.AddScreen(new MainMenuScreen(), null);
        }


        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();

            // Draw the title
            ScreenManager.SpriteBatch.DrawString(highScoreFont,
                "High Scores", new Vector2(30, 30), Color.White);

            // Draw the Menu Entries
            this.gameModeMenuEntry.Position = new Vector2(20, 90);
            this.gameModeMenuEntry.DrawAsAButton(this, gameTime);

            this.gameDifficultyMenuEntry.Position = new Vector2(250, 90);
            this.gameDifficultyMenuEntry.DrawAsAButton(this, gameTime);

            int type = ModeDifficultyToInt(currentGameMode, currentGameDifficulty);
            // Draw the highscores table
            for (int i = 0; i < highScore[type].Length; i++)
            {
                if (highScore[type][i].Value != 0)
                {

                    ScreenManager.SpriteBatch.DrawString(highScoreFont,
                        String.Format("#{0}. ", i + 1/*, highScore[i].Key*/),
                        new Vector2(100, i * 40 + 150), Color.YellowGreen);
                    ScreenManager.SpriteBatch.DrawString(highScoreFont,
                        highScore[type][i].Value + "",
                        new Vector2(200, i * 40 + 150), Color.YellowGreen);
                }
            }

            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }

        public static int HighestScore(gameMode gameMode, 
            gameDifficulty gameDifficulty)
        {
            OrderGameScore();
            int type = ModeDifficultyToInt(gameMode, gameDifficulty);
            if (type > 0)
            {
                return highScore[type][0].Value;
            }
            else
            {
                return 0;
            }
        }

        public static bool IsInHighscores(int gameScore, int type)
        {
            // If the score is less from the last place score
            return gameScore > highScore[type][highscorePlaces - 1].Value;
        }

        private static void OrderGameScore()
        {
            if (highScore[0] == null)
            {
                FillUpDefaultHighScores();
            }

            for (int type = 0; type < 4; type++)
            {
                highScore[type] = (highScore[type].OrderByDescending(e => e.Value)).ToArray();
            }
        }

        private static int ModeDifficultyToInt(gameMode gameMode,
            gameDifficulty gameDifficulty)
        {
            if (gameMode == gameMode.Classic &&
                gameDifficulty == gameDifficulty.Easy)
            {
                return 0;
            }
            if (gameMode == gameMode.Classic &&
                gameDifficulty == gameDifficulty.Hard)
            {
                return 1;
            }
            if (gameMode == gameMode.Arcade &&
                gameDifficulty == gameDifficulty.Easy)
            {
                return 2;
            }
            if (gameMode == gameMode.Arcade &&
                gameDifficulty == gameDifficulty.Hard)
            {
                return 3;
            }
            return -1;
        }

        public static void PutHighScore(string playerName, int gameScore, 
            gameMode gameMode, gameDifficulty gameDifficulty)
        {
            int type = ModeDifficultyToInt(gameMode, gameDifficulty);
            if (type == -1)
                return;

            if (IsInHighscores(gameScore, type))
            {
                highScore[type][highscorePlaces - 1] =
                    new KeyValuePair<string, Int32>(playerName,
                        gameScore);
                OrderGameScore();
            }
        }

        public static void SaveHighscore()
        {
            // Get the place to store the data
            using (IsolatedStorageFile isf =
                IsolatedStorageFile.GetUserStoreForApplication())
            {
                for (int type = 0; type < 4; type++)
                {
                    // Create the file to save the data
                    using (IsolatedStorageFileStream isfs = new
                        IsolatedStorageFileStream("highscores" + type + ".txt",
                        FileMode.Create, isf))
                    {
                        // Get the stream to write the file
                        using (StreamWriter writer = new StreamWriter(isfs))
                        {
                            for (int i = 0; i < highScore[type].Length; i++)
                            {
                                // Write the scores
                                writer.WriteLine(highScore[type][i].Key);
                                writer.WriteLine(highScore[type][i].Value.ToString());
                            }
                            // Save and close the file
                            writer.Flush();
                            writer.Close();
                        }
                    }
                }
            }
        }

        private static void FillUpDefaultHighScores()
        {
            for (int type = 0; type < 4; type++)
            {
                highScore[type] = new KeyValuePair<string, Int32>[highscorePlaces];
                for (int i = 0; i < highscorePlaces; i++)
                {
                    highScore[type][i] = new KeyValuePair<string, Int32>("Kanav", 0);
                }
            }
        }

        public static void LoadHighscore()
        {
            FillUpDefaultHighScores();

            // Get the place the data stored
            using (IsolatedStorageFile isf =
                IsolatedStorageFile.GetUserStoreForApplication())
            {
                for (int type=0; type <4; type++){

                // Try to open the file
                    if (isf.FileExists("highscores" + type+ ".txt"))
                    {
                        using (IsolatedStorageFileStream isfs = new
                            IsolatedStorageFileStream("highscores" + type +".txt",
                            FileMode.Open, isf))
                        {
                            // Get the stream to read the data
                            using (StreamReader reader = new StreamReader(isfs))
                            {
                                // Read the highscores
                                int i = 0;
                                while (!reader.EndOfStream)
                                {
                                    string[] line = new[] { reader.ReadLine(),
                                reader.ReadLine() };
                                    highScore[type][i++] = new KeyValuePair<string,
                                        Int32>(line[0],
                                    Int32.Parse(line[1]));
                                }
                            }
                        }
                    }
                }
            }


            OrderGameScore();
        }
    }

    
}
