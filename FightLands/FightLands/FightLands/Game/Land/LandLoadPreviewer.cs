using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class LandLoadPreviewer : GameObject
    {
        Land loadingLand;

        DrawableText statusText;
        DrawableTexture statusBar;
        float barTime;

        float fade = 1f;
        int dots = 0;

        public LandLoadPreviewer(World world, Land loadingLand)
            : base(world)
        {
            this.loadingLand = loadingLand;

            AssetSpriteFont font = AssetManager.CreateAssetSpriteFont("landLoaderFont", "defaultFont");
            statusText = new DrawableText(font, this, "", Color.White);

            statusBar = new DrawableTexture("whiteSquare", this);
            statusBar.size = new Vector2(20f, 10f);
            statusBar.position = new Vector2(0f, 30f);
        }

        public override void Update(UpdateState state)
        {
            if (loadingLand.isBeingLoaded())
            {
                dots++;
                int dotNumber = (dots / 40) % 4;
                switch (dotNumber)
                {
                    case 0:
                        statusText.text = loadingLand.checkLoadStatus() + "";
                        break;
                    case 1:
                        statusText.text = " " + loadingLand.checkLoadStatus() + ".";
                        break;
                    case 2:
                        statusText.text = "  " + loadingLand.checkLoadStatus() + "..";
                        break;
                    case 3:
                        statusText.text = "   " + loadingLand.checkLoadStatus() + "...";
                        break;
                }

                barTime += state.elapsedTime;
                statusBar.position.X = 120f * (float)Math.Sin(barTime*Math.PI);
            }
            else if(loadingLand.isLoaded())
            {
                fade -= state.elapsedTime;

                if (fade < 0f)
                    fade = 0f;

                statusText.text = "Loaded";
                statusText.filter = Color.Lerp(Color.Transparent, Color.White, fade);
                statusBar.filter = Color.Lerp(Color.Transparent, Color.White, fade);

                if (fade == 0f)
                    destroy();
            }
        }

        public override void Draw(DrawState state)
        {
            statusText.Draw(state);
            statusBar.Draw(state);
        }

    }
}
