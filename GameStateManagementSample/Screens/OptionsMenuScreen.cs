#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace BouncyBall
{
    public enum gameMode
    {
        Classic,
        Arcade,
    }

    public enum gameDifficulty
    {
        Easy,
        Hard
    }

    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry gameModeMenuEntry;
        MenuEntry gameDifficultyMenuEntry;
        MenuEntry languageMenuEntry;
        MenuEntry SoundEffectsMenuEntry;
        MenuEntry elfMenuEntry;

        

        public static gameMode currentGameMode = gameMode.Arcade;
        public static gameDifficulty currentGameDifficulty = gameDifficulty.Easy;

        static string[] languages = { "C#", "French", "Deoxyribonucleic acid" };
        static int currentLanguage = 0;

        public static bool soundEffects = true;

        static int elf = 23;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            gameModeMenuEntry = new MenuEntry(string.Empty);
            gameDifficultyMenuEntry = new MenuEntry(string.Empty);
            languageMenuEntry = new MenuEntry(string.Empty);
            SoundEffectsMenuEntry = new MenuEntry(string.Empty);
            elfMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            // Hook up menu event handlers.
            gameModeMenuEntry.Selected += UngulateMenuEntrySelected;
            gameDifficultyMenuEntry.Selected += gameDifficultyMenuEntrySelected;
            languageMenuEntry.Selected += LanguageMenuEntrySelected;
            SoundEffectsMenuEntry.Selected += SoundEffectsMenuEntrySelected;
            elfMenuEntry.Selected += ElfMenuEntrySelected;
            
            // Add entries to the menu.
            MenuEntries.Add(gameModeMenuEntry);
            MenuEntries.Add(gameDifficultyMenuEntry);
            MenuEntries.Add(languageMenuEntry);
            MenuEntries.Add(SoundEffectsMenuEntry);
            MenuEntries.Add(elfMenuEntry);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            gameModeMenuEntry.Text = "Game Mode: " + currentGameMode;
            gameDifficultyMenuEntry.Text = "Game Difficulty: " + currentGameDifficulty;
            languageMenuEntry.Text = "Language: " + languages[currentLanguage];
            SoundEffectsMenuEntry.Text = "Sound Effects: " + (soundEffects ? "on" : "off");
            elfMenuEntry.Text = "elf: " + elf;
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void UngulateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
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

        /// <summary>
        /// Event handler for when the Language menu entry is selected.
        /// </summary>
        void LanguageMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentLanguage = (currentLanguage + 1) % languages.Length;

            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Frobnicate menu entry is selected.
        /// </summary>
        void SoundEffectsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            soundEffects = !soundEffects;

            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Elf menu entry is selected.
        /// </summary>
        void ElfMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            elf++;

            SetMenuEntryText();
        }


        #endregion
    }
}
