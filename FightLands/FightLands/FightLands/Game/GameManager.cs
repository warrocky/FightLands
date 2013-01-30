using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

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
        LandActiveBox landActiveBox;

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
            keyMapping.Add(ActionKeyType.Space, Keys.Space);
            player1 = new LocalPlayer(keyMapping);
            PlayerManager.addPlayer(player1, "player1");
        }

        public void StartGame()
        {
            gameState = gameManagerState.inLands;
            land = new Land(0);
            landActiveBox = new LandActiveBox(land, world, new Point(600,600));
            landCamera = landActiveBox.camera;
            LandCameraControl control = new LandCameraControl(land, landCamera);
            HumanPlayer human = new HumanPlayer(land);
            PlayerManager.getPlayer("player1").addControlable(human);
            control.setAnchor(human);
            land.addContentRequirer(human);
            land.addUpdateNode(human);

            MapPreviewer minimap = new MapPreviewer(this.world, land, new Vector2(200f,200f));
            minimap.position = UserInterfaceManager.getCurrentUpperLeftCorner() + new Vector2(100f,100f);

            landActiveBox.position.X = UserInterfaceManager.getCurrentUpperLeftCorner().X + 200f + 300f;
        }
        public void QuitGame()
        {
            game.Exit();
        }

        public override void Update(UpdateState state)
        {
            if (gameState == gameManagerState.inLands)
                land.Update(state);
        }
    }
}
