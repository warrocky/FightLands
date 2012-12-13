using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FightLands
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        Point resolution;
        KeyboardState previousKeyboardState;
        KeyboardState currentKeyboardState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            resolution = new Point(800, 600);
            graphics.PreferredBackBufferWidth = resolution.X;
            graphics.PreferredBackBufferHeight = resolution.Y;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            UpdateManager.Initialize();
            Statistics.Initialize();
            PlayerManager.Initialize();
            AssetManager.Initialize(Content);
            Graphics.Initialize(graphics.GraphicsDevice, resolution);
            UserInterfaceManager.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Graphics.LoadContent();
            AssetManager.LoadContent();


            new GameManager(UserInterfaceManager.baseWorld, this);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            UpdateManager.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Graphics.Draw();
            Graphics.Render();

            Statistics.drawCount++;
            base.Draw(gameTime);
        }
    }

    static class Statistics
    {
        public static int updateLogLength = 60;
        public static List<UpdateStatistics> lastUpdateStatistics;
        public static float updatesPerSec;

        public static int drawLogLength = 4;
        public static int drawCount;
        static int second;
        public static List<DrawStatistics> LastDrawStatistics;
        public static float fps;

        public static int keyPresses;

        public static void Initialize()
        {
            lastUpdateStatistics = new List<UpdateStatistics>();
            LastDrawStatistics = new List<DrawStatistics>();
        }
        public static void Update(GameTime time)
        {
            float elapsedTime = ((float)time.ElapsedGameTime.Milliseconds) / 1000f;
            UpdateStatistics cycleStatistics = new UpdateStatistics();
            cycleStatistics.timeElapsed = elapsedTime;
            lastUpdateStatistics.Add(cycleStatistics);

            if (lastUpdateStatistics.Count >= updateLogLength)
                lastUpdateStatistics.RemoveAt(0);

            float timesum = 0;
            for (int i = 0; i < lastUpdateStatistics.Count; i++)
                timesum += lastUpdateStatistics[i].timeElapsed;

            updatesPerSec = 1f/(timesum / lastUpdateStatistics.Count);

            //DrawStatistics
            if (second != time.TotalGameTime.Seconds)
            {
                second = time.TotalGameTime.Seconds;

                DrawStatistics drawCycleStatistics = new DrawStatistics();
                drawCycleStatistics.drawsThisSec = drawCount;
                drawCount = 0;

                LastDrawStatistics.Add(drawCycleStatistics);

                if (LastDrawStatistics.Count >= drawLogLength)
                    LastDrawStatistics.RemoveAt(0);

                int drawSum = 0;
                for (int i = 0; i < LastDrawStatistics.Count; i++)
                    drawSum += LastDrawStatistics[i].drawsThisSec;

                if(LastDrawStatistics.Count != 0)
                    fps = (float)drawSum / (float)LastDrawStatistics.Count;
            }
        }

        public struct UpdateStatistics
        {
            public float timeElapsed;
        }
        public struct DrawStatistics
        {
            public int drawsThisSec;
        }

        public static int repID = -1;
        public static void checkIfRepeats(UpdateState state)
        {
            bool a;

            if (state.GlobalUpdateID != repID)
                repID = state.GlobalUpdateID;
            else
                a = true;
        }
    }
}
