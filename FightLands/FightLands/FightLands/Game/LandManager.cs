using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class LandManager : GameObject
    {
        GameManager gameManager;

        Land land;
        LandActiveBox activeBox;
        Camera landCamera;

        MapPreviewer minimap;
        LoopingAnimation minimapLoopAnimation;
        Animation minimapFinishLoadAnimation;

        LandLoadPreviewer loadPreviewer;

        DrawableTexture background;

        public enum LandManagerState { idle, loading, inland, infight }
        public LandManagerState state;

        public LandManager(GameManager gameManager)
            :base(gameManager.world)
        {
            state = LandManagerState.idle;
            this.gameManager = gameManager;

            background = new DrawableTexture("whiteSquare", this);
            background.size = UserInterfaceManager.getDiagonalOnWorld();
            background.filter = Color.Black;
            background.layer = 1f;
        }

        public void StartGame()
        {
            if (state != LandManagerState.idle)
                throw new Exception("Trying to start a game twice.");

            state = LandManagerState.loading;

            land = new Land(9);
            //activeBox = new LandActiveBox(land, world, new Point(600, 600));
            //landCamera = activeBox.camera;

            //LandCameraControl control = new LandCameraControl(land, landCamera);
            //HumanPlayer human = new HumanPlayer(land);
            
            //PlayerManager.getPlayer("player1").addControlable(human);
            
            //control.setAnchor(human);
            
            //land.addContentRequirer(human);
            //land.addUpdateNode(human);

            minimap = new MapPreviewer(this.world, land, new Vector2(400f, 400f));
            minimap.position = UserInterfaceManager.getCurrentLowerLeftCorner() + UserInterfaceManager.getDiagonalOnWorld()/2f + new Vector2(0f,-20f);

            minimapLoopAnimation = new LoopingAnimation(world, minimap, minimap.position, minimap.position, minimap.rotation, minimap.rotation, new Vector2(395f,395f), new Vector2(400f,400f), 5f, LoopingAnimation.InterpolationType.Trignometric, true, 0f);

            //activeBox.position.X = UserInterfaceManager.getCurrentUpperLeftCorner().X + 200f + 300f;

            land.loadMap();
            loadPreviewer = new LandLoadPreviewer(world, land);
            loadPreviewer.position = new Vector2(0f, 220f);
        }

        public override void Update(UpdateState state)
        {
            switch (this.state)
            {
                case LandManagerState.loading:

                    if (land.isLoaded())
                    {
                        if (minimapFinishLoadAnimation == null)
                        {
                            minimapLoopAnimation.destroy();
                            minimapLoopAnimation = null;
                            Vector2 minimapPosition = UserInterfaceManager.getCurrentUpperLeftCorner() + new Vector2(100f, 100f);
                            minimapFinishLoadAnimation = new Animation(world, minimap, minimapPosition, 0f, new Vector2(200f, 200f), 1.2f, Animation.InterpolationType.linear, true);
                        }
                        else if(minimapFinishLoadAnimation.isFinished())
                        {
                            minimapFinishLoadAnimation.destroy();
                            minimapFinishLoadAnimation = null;

                            loadPreviewer = null;

                            Rectangle activeBoxArea = gameManager.getActiveBoxArea();
                            activeBox = new LandActiveBox(land, world, new Point(activeBoxArea.Width, activeBoxArea.Height));
                            landCamera = activeBox.camera;

                            LandCameraControl control = new LandCameraControl(land, landCamera);
                            HumanPlayer metaHuman = new HumanPlayer(land);
                            LandHumanPlayer human = new LandHumanPlayer(land, metaHuman);

                            PlayerManager.getPlayer("player1").addControlable(human);

                            control.setAnchor(human);

                            land.addContentRequirer(human);
                            land.addUpdateNode(human);

                            activeBox.position.X = UserInterfaceManager.getCurrentUpperLeftCorner().X + 200f + activeBoxArea.Width/2;

                            this.state = LandManagerState.inland;
                        }
                    }
                    break;
                case LandManagerState.inland:
                    land.Update(state);
                    break;
            }
        }
        public override void Draw(DrawState state)
        {
            switch (this.state)
            {
                case LandManagerState.loading:
                    background.Draw(state);
                    break;
                case LandManagerState.inland:
                    background.Draw(state);
                    break;
            }
        }
    }
}
