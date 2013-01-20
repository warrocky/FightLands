using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FightLands
{
    class Land : World
    {
        public int widthInTiles;
        public int heightInTiles;

        public Vector2 tileSize;

        public float tileWidth
        {
            get { return tileSize.X; }
            set { tileSize.X = value; }
        }
        public float tileHeight
        {
            get { return tileSize.Y; }
            set { tileSize.Y = value; }
        }

        public float width
        {
            get { return widthInTiles * tileWidth; }
        }
        public float height
        {
            get { return heightInTiles * tileHeight; }
        }

        public Noise grassLandGreeness;
        public Noise dirtPatches;

        private Random rdm;

        List<LandContentRequirer> contentRequirers;
        List<LandUpdateNode> updateNodes;

        TerrainTile[,] tiles;

        public Land(int seed)
        {
            rdm = new Random(seed);
            contentRequirers = new List<LandContentRequirer>();
            updateNodes = new List<LandUpdateNode>();
            widthInTiles = 50;
            heightInTiles = 50;
            tileSize = new Vector2(100f, 100f);


            grassLandGreeness = Noise.RegularNoise(800f, 2, rdm.Next());
            grassLandGreeness.filter = (float a, Vector2 b) => (a*1.5f + 1f)/2f;

            dirtPatches = Noise.TurbulenceNoise(800f, 6, rdm.Next());
            dirtPatches.filter = (float a, Vector2 b) => ((-a) * 1.5f + 1f) / 2f;

            Vector2 center = new Vector2(width, height) / 2f;
            tiles = new TerrainTile[widthInTiles, heightInTiles];
            for(int i=0;i<widthInTiles;i++)
                for (int j = 0; j < heightInTiles; j++)
                {
                    tiles[i,j] = new TerrainTile(TerrainTile.TerrainType.Grassland, new Vector2(i*tileWidth, j*tileHeight) - center, tileSize, this,rdm.Next());
                    objectAddList.Remove(tiles[i, j]);
                }

        }

        public override void Update(UpdateState state)
        {
            base.Update(state);
            LoadRequiredContent();
        }
        private void LoadRequiredContent()
        {
            foreach (TerrainTile tile in tiles)
            {
                foreach (LandContentRequirer requester in contentRequirers)
                {
                    if ((tile.position - requester.LandContentRequirerPosition(this)).Length() < tileSize.Length() / 2f + requester.LandContentRequirerRadius(this))
                    {
                        tile.loadTexture();
                    }
                }
            }
        }


        public override void Draw(DrawState state)
        {
            Vector2 drawArea = state.currentCamera.diagonal;
            Vector2 drawPosition = state.currentCamera.position;

            drawArea.X /= tileSize.X;
            drawArea.Y /= tileSize.Y;

            Point tileArea = new Point((int)Math.Ceiling(drawArea.X) + 2, (int)Math.Ceiling(drawArea.Y) + 2);
            Point tileCenter = new Point((int)(drawPosition.X/tileWidth) + widthInTiles / 2, (int)(drawPosition.Y/tileHeight) + heightInTiles / 2);

            int x, y;
            for (int i = 0; i < tileArea.X; i++)
                for (int j = 0; j < tileArea.Y; j++)
                {
                    x = i - tileArea.X / 2 + tileCenter.X;
                    y = j - tileArea.Y / 2 + tileCenter.Y;

                    if (x >= 0 && x < widthInTiles && y >= 0 && y < heightInTiles)
                        tiles[x, y].Draw(state);
                }

            base.Draw(state);
        }

        public void addContentRequirer(LandContentRequirer requirer)
        {
            contentRequirers.Add(requirer);
        }
        public void removeContentRequirer(LandContentRequirer requirer)
        {
            contentRequirers.Remove(requirer);
        }
        public interface LandContentRequirer
        {
            float LandContentRequirerRadius(Land land);
            Vector2 LandContentRequirerPosition(Land land);
        }

        public void addUpdateNode(LandUpdateNode node)
        {
            updateNodes.Add(node);
        }
        public void removeUpdateNode(LandUpdateNode node)
        {
            updateNodes.Remove(node);
        }
        public interface LandUpdateNode
        {
            float LandUpdateNodeRadius(Land land);
            Vector2 LandUpdateNodePosition(Land land);
        }
    }
}
