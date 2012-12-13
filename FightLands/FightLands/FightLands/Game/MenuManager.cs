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
                {
                    mainMenu = new MainMenu(gameManager);
                    gameManager.player1.addControlable(mainMenu);
                }
            }
            else
                if (mainMenu != null)
                {
                    gameManager.player1.removeControlable(mainMenu);
                    mainMenu = null;
                }
        }

        public class MainMenu : Menu<MainMenu.DefaultEntry>, Controlable
        {
            GameManager manager;

            public MainMenu(GameManager manager)
                :base(manager.world)
            {
                this.manager = manager;

                entries.Add(new QuitGame(manager));
                entries.Add(new StartGame(manager));

                entries[0].position = new Vector2(0, 100);
                entries[1].position = new Vector2(0, -100);
            }

            public class DefaultEntry : GameObject, MenuEntry
            {
                protected GameManager manager;
                protected DrawableText texture;

                public DefaultEntry(GameManager manager, String text)
                    :base(manager.world)
                {
                    this.manager = manager;

                    AssetSpriteFont font = AssetManager.getAssetSpriteFont("menuHeaderFont");
                    texture = new DrawableText(font, this, text, Color.Gray);
                }
                public virtual void Select()
                {}
                public void Focus()
                {
                    texture.filter = Color.White;
                }
                public void Unfocus()
                {
                    texture.filter = Color.Gray;
                }

                public override void Draw(DrawState state)
                {
                    texture.Draw(state);
                }
            }
            public class StartGame : DefaultEntry
            {
                public StartGame(GameManager manager)
                    :base(manager, "Start Game")
                {}
                public override void Select()
                {
                    manager.StartGame();
                }
            }
            public class QuitGame : DefaultEntry
            {
                public QuitGame(GameManager manager)
                    : base(manager, "Quit game")
                {}
                public override void Select()
                {
                    manager.QuitGame();
                }
            }
            public void Interact(PlayerKeyboard actionKeyboard)
            {
                if (actionKeyboard.keyboard[ActionKeyType.Up].JustPressed)
                    selectedEntry++;

                if (actionKeyboard.keyboard[ActionKeyType.Down].JustPressed)
                    selectedEntry--;

                if (actionKeyboard.keyboard[ActionKeyType.Space].JustPressed)
                    SelectCurrentEntry();
            }
        }
    }
}
