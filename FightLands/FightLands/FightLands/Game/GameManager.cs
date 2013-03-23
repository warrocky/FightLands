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
        LandManager landManager;
        FightManager fightManager;

        public gameManagerState gameState;
        public enum gameManagerState { inMenu, inLands, inFight }

        public readonly LocalPlayer player1;

        public GameManager(World UIWorld, Game game)
            :base(UIWorld)
        {
            this.game = game;
            
            // Create managers
            menuManager = new MenuManager(this);
            landManager = new LandManager(this);
            fightManager = new FightManager(this);
            gameState = gameManagerState.inMenu;


            //Create player
            Dictionary<ActionKeyType, Keys> keyMapping = new Dictionary<ActionKeyType,Keys>();
            keyMapping.Add(ActionKeyType.Up, Keys.W);
            keyMapping.Add(ActionKeyType.Down, Keys.S);
            keyMapping.Add(ActionKeyType.Right, Keys.D);
            keyMapping.Add(ActionKeyType.Left, Keys.A);
            keyMapping.Add(ActionKeyType.Space, Keys.Space);
            player1 = new LocalPlayer(keyMapping);
            PlayerManager.addPlayer(player1, "player1");


            //UserInterfaceManager.createNewUserInterfaceArea(new Vector2(Graphics.resolution.X - 200, Graphics.resolution.Y), new Vector2((Graphics.resolution.X/2f - 200f)/2f, 0), "activebox");
            UserInterfaceManager.createNewUserInterfaceArea(new Rectangle(200, 0, Graphics.resolution.X - 200, Graphics.resolution.Y), "activebox");
        }

        public void StartGame()
        {
            gameState = gameManagerState.inLands;

            landManager.StartGame();

            //land = new Land(1);
            //landActiveBox = new LandActiveBox(land, world, new Point(600,600));
            //landCamera = landActiveBox.camera;
            //LandCameraControl control = new LandCameraControl(land, landCamera);
            //HumanPlayer human = new HumanPlayer(land);
            //PlayerManager.getPlayer("player1").addControlable(human);
            //control.setAnchor(human);
            //land.addContentRequirer(human);
            //land.addUpdateNode(human);

            //MapPreviewer minimap = new MapPreviewer(this.world, land, new Vector2(200f,200f));
            //minimap.position = UserInterfaceManager.getCurrentUpperLeftCorner() + new Vector2(100f,100f);

            //landActiveBox.position.X = UserInterfaceManager.getCurrentUpperLeftCorner().X + 200f + 300f;

            //land.loadMap();

        }
        public void StartFight(Creature creature1, Creature creature2, Vector2 worldPosition)
        {
            gameState = gameManagerState.inFight;
            landManager.StartFight();
            fightManager.StartFight(creature1, creature2, landManager.land, worldPosition);
        }
        public void FightEnded()
        {
            gameState = gameManagerState.inLands;
            landManager.ReturnFromFight();
        }
        public void QuitGame()
        {
            game.Exit();
        }

        public Rectangle getUiLeftColumn()
        {
            return new Rectangle(0, 0, 200, Graphics.resolution.Y);
        }
        public Rectangle getActiveBoxArea()
        {
            return UserInterfaceManager.getUserInterfaceArea("activebox").getRectangle();
        }

        public override void Update(UpdateState state)
        {

        }
    }
}
