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
        MenuManager menuManager;

        public gameManagerState gameState;
        public enum gameManagerState { inMenu, inLands, inFight }

        Land land;
        Camera landCamera;
        ActiveBox landActiveBox;

        LocalPlayer player1;


        public GameManager(World UIWorld)
            :base(UIWorld)
        {
            menuManager = new MenuManager(this);
            gameState = gameManagerState.inMenu;

            Dictionary<Player.ActionKeyType, Keys> keyMapping = new Dictionary<Player.ActionKeyType,Keys>();
            keyMapping.Add(Player.ActionKeyType.Up, Keys.W);
            keyMapping.Add(Player.ActionKeyType.Down, Keys.S);
            keyMapping.Add(Player.ActionKeyType.Right, Keys.D);
            keyMapping.Add(Player.ActionKeyType.Left, Keys.A);
            player1 = new LocalPlayer(keyMapping);
            PlayerManager.addPlayer(player1);
        }
        public void StartGame()
        {
            gameState = gameManagerState.inLands;
            land = new Land();
            landCamera = new Camera(400,400, land);
            landActiveBox = new ActiveBox(world, landCamera);
        }
        public override void Update(UpdateState state)
        {

        }
    }
}
