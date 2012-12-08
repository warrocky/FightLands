using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class MenuManager : GameObject
    {
        GameManager gameManager;

        MainMenu mainMenu;

        public MenuManager(GameManager gameManager)
            : base(gameManager.world)
        {
            this.gameManager = gameManager;

            AssetManager.CreateAssetSpriteFont("menuHeaderFont", "defaultFont");
        }
        public override void Update(UpdateState state)
        {
            if (gameManager.gameState == GameManager.gameManagerState.inMenu)
            {
                if (mainMenu == null)
                    mainMenu = new MainMenu(this);
            }
            else
                if (mainMenu != null)
                    mainMenu.destroy();
        }

        public class MainMenu : GameObject
        {
            MenuManager manager;

            DrawableText startGame;
            DrawableText exitGame;

            int selectedOption;

            public MainMenu(MenuManager manager)
                :base(manager.world)
            {
                this.manager = manager;

                AssetSpriteFont font = AssetManager.getAssetSpriteFont("menuHeaderFont");
                startGame = new DrawableText(font, this, "Start Game", Color.Gray);
                startGame.position.Y = -100;
                exitGame = new DrawableText(font, this, "Quit Game", Color.Gray);
                exitGame.position.Y = 100;
            }
            public override void Draw(DrawState state)
            {
                startGame.Draw(state);
                exitGame.Draw(state);
            }
        }
    }
}
