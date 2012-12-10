using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FightLands
{
    class GameManager : GameObject
    {
        private Game game;

        MenuManager menuManager;

        public gameManagerState gameState;
        public enum gameManagerState { inMenu, inLands, inFight }

        Land land;
        Camera landCamera;
        ActiveBox landActiveBox;

        public readonly LocalPlayer player1;

        public GameManager(World UIWorld, Game game)
            :base(UIWorld)
        {
            this.game = game;
            menuManager = new MenuManager(this);
            gameState = gameManagerState.inMenu;

            Dictionary<ActionKeyType, Keys> keyMapping = new Dictionary<ActionKeyType,Keys>();
            keyMapping.Add(ActionKeyType.Up, Keys.W);
            keyMapping.Add(ActionKeyType.Down, Keys.S);
            keyMapping.Add(ActionKeyType.Right, Keys.D);
            keyMapping.Add(ActionKeyType.Left, Keys.A);
            player1 = new LocalPlayer(keyMapping);
            PlayerManager.addPlayer(player1, "player1");
        }
        public void StartGame()
        {
            gameState = gameManagerState.inLands;
            land = new Land();
            landCamera = new Camera(400,400, land);
            landActiveBox = new ActiveBox(world, landCamera);
        }
        public void QuitGame()
        {
            game.Exit();
        }
        public override void Update(UpdateState state)
        {

        }
    }
}
