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
        public Noise mountainChance;
        public Noise mountainChainNoise;
        public Noise treeChanceNoise;

        private Random rdm;

        List<LandContentRequirer> contentRequirers;
        List<LandUpdateNode> updateNodes;

        TerrainTile[,] tiles;
        List<Mountain> mountainList;
        List<Tree> treeList;

        public Land(int seed)
        {
            rdm = new Random(seed);
            contentRequirers = new List<LandContentRequirer>();
            updateNodes = new List<LandUpdateNode>();
            widthInTiles = 50;
            heightInTiles = 50;
            tileSize = new Vector2(100f, 100f);


            //Topografy defining Noises
            grassLandGreeness = Noise.RegularNoise(800f, 2, rdm.Next());
            grassLandGreeness.filter = (float a, Vector2 b) => (a*1.5f + 1f)/2f;

            dirtPatches = Noise.TurbulenceNoise(800f, 6, rdm.Next());
            dirtPatches.filter = (float a, Vector2 b) => ((-a) * 1.5f + 1f) / 2f;

            mountainChance = Noise.RegularNoise(width / 4f, 1, rdm.Next());
            mountainChance.filter = (float a, Vector2 b) => (a + 1f)/2f;

            mountainChainNoise = Noise.TurbulenceNoise(width/2f, 2, rdm.Next());
            mountainChainNoise.filter = (float a, Vector2 b) => (a + 1)/2f;

            treeChanceNoise = Noise.RegularNoise(width / 4f, 1, rdm.Next());
            treeChanceNoise.filter = (float a, Vector2 b) => (a + 1f) / 2f;

            //map structures
            tiles = new TerrainTile[widthInTiles, heightInTiles];
            mountainList = new List<Mountain>();
            treeList = new List<Tree>();



            //Tile creation
            Vector2 center = new Vector2(width, height) / 2f;
            for(int i=0;i<widthInTiles;i++)
                for (int j = 0; j < heightInTiles; j++)
                {
                    tiles[i,j] = new TerrainTile(TerrainTile.TerrainType.Grassland, new Vector2(i*tileWidth, j*tileHeight) - center, tileSize, this,rdm.Next());
                    objectAddList.Remove(tiles[i, j]);
                }


            //Mountain Creation
            Mountain m1 = null;
            Vector2 mountainPos;
            float radius, dist, minDist = float.MaxValue;
            float[,] mountainChainValues = mountainChainNoise.getValues(new Point(200, 200), Vector2.Zero - new Vector2(width,height)/2f, new Vector2(width, height));
            float[,] mountainChanceValues = mountainChance.getValues(new Point(200, 200), Vector2.Zero - new Vector2(width, height) / 2f, new Vector2(width, height));
            int arrayX, arrayY;
            int mountainTryCounter = 0;
            for (int i = 0; i < widthInTiles*heightInTiles; i++)
            {
                mountainTryCounter = 5;
                do
                {
                    m1 = null;

                    mountainPos = new Vector2((i % widthInTiles) * tileSize.X, (i / widthInTiles) * tileSize.Y);

                    arrayX = (int)((mountainPos.X / width) * 200f);
                    arrayY = (int)((mountainPos.Y / height) * 200f);

                    mountainPos = tiles[(i % widthInTiles), (i / widthInTiles)].position;
                    mountainPos += new Vector2((float)rdm.NextDouble() * tileWidth, (float)rdm.NextDouble() * tileHeight);// -new Vector2(width, height) / 2f;

                    //check if chance for mountains is critical
                    if (mountainChainValues[arrayX, arrayY] < 0.1f && mountainChanceValues[arrayX, arrayY] < 0.75f)
                    {
                        minDist = float.MaxValue;

                        //search the smallest distance to a mountain
                        for (int j = 0; j < mountainList.Count; j++)
                        {
                            dist = (mountainList[j].position - mountainPos).Length() - mountainList[j].radius*0.7f;
                            if (dist < minDist)
                                minDist = dist;
                        }

                        //test value to avoid collision among mountains - should be 0
                        if (minDist > 20f)
                        {
                            //Create random radius
                            radius = MathHelper.getNextNormalDistributedFloat(3f, 1f, rdm) * (10f + 20f*(1f - mountainChainValues[arrayX, arrayY]));

                            //if radius collides with other mountain, reduce the radius.
                            if (radius > minDist)
                                if (minDist > 20f)
                                    radius = minDist;
                                else
                                    radius = 20f;

                            //Create new mountain
                            m1 = new Mountain(rdm.Next(), this, radius);
                            m1.position = mountainPos;
                            objectAddList.Remove(m1);
                            mountainList.Add(m1);
                        }
                    }

                    //se não foi gerada nenhuma montanha, reduzir um valor no counter.
                    if (m1 == null)
                        mountainTryCounter--;
                }while(mountainTryCounter != 0);
            }


            //tree generation
            Tree t1 = null;
            Vector2 treePos;
            //float radius, dist, minDist = float.MaxValue;
            minDist = float.MaxValue;
            float[,] treeChanceValues = mountainChance.getValues(new Point(widthInTiles, heightInTiles), Vector2.Zero - new Vector2(width, height) / 2f, new Vector2(width, height));
            //int arrayX, arrayY;
            int treeTryCounter = 0;
            for (int i = 0; i < widthInTiles * heightInTiles; i++)
            {
                treeTryCounter = 5;
                do
                {
                    t1 = null;

                    //treePos = new Vector2((i % widthInTiles) * tileSize.X, (i / widthInTiles) * tileSize.Y);

                    arrayX = (int)(i%widthInTiles);
                    arrayY = (int)(i/widthInTiles);

                    treePos = tiles[(i % widthInTiles), (i / widthInTiles)].position;
                    treePos += new Vector2((float)rdm.NextDouble() * tileWidth, (float)rdm.NextDouble() * tileHeight);// -new Vector2(width, height) / 2f;

                    //check chance for trees
                    if (treeChanceValues[arrayX,arrayY] > 0.6f)
                    {
                        minDist = float.MaxValue;

                        //search the smallest distance to a tree or mountain
                        for (int j = 0; j < treeList.Count; j++)
                        {
                            dist = (treeList[j].position - treePos).Length() - treeList[j].radius * 0.7f;
                            if (dist < minDist)
                                minDist = dist;
                        }
                        for (int j = 0; j < mountainList.Count; j++)
                        {
                            dist = (mountainList[j].position - treePos).Length() - mountainList[j].radius * 0.7f;
                            if (dist < minDist)
                                minDist = dist;
                        }

                        //test value to avoid collision with trees and mountains
                        if (minDist > 20f)
                        {
                            //Create random radius
                            //radius = MathHelper.getNextNormalDistributedFloat(3f, 1f, rdm) * (10f + 20f * (1f - mountainChainValues[arrayX, arrayY]));
                            radius = (MathHelper.getNextNormalDistributedFloat(4, 1, rdm)) * 5f;

                            //if radius collides with other tree, reduce the radius.
                            if (radius > minDist)
                                if (minDist > 20f)
                                    radius = minDist;
                                else
                                    radius = 20f;

                            //Create new tree
                            t1 = new Tree(rdm.Next(), radius, this);
                            t1.position = treePos;
                            objectAddList.Remove(t1);
                            treeList.Add(t1);
                        }
                    }

                    //se não foi gerada nenhuma montanha, reduzir um valor no counter.
                    if (t1 == null)
                        treeTryCounter--;
                } while (treeTryCounter != 0);
            }

            //AssetTexture astt = new AssetTexture(getTreeChanceTexture(), "astt");
            //Dummy dum = new Dummy(this, astt);
            //dum.texture.size = new Vector2(200f, 200f);

            arrayX = 2;
        }

        private Microsoft.Xna.Framework.Graphics.Texture2D getMountainChanceTexture()
        {
            float[,] array2 = mountainChainNoise.getValues(new Point(200, 200), Vector2.Zero, new Vector2(width, height));
            float[,] array3 = mountainChance.getValues(new Point(200, 200), Vector2.Zero, new Vector2(width, height));
            Color[] array = new Color[200 * 200];
            float x, y;
            for (int i = 0; i < array.Length; i++)
            {
                x = width * (1f / 200f) * (float)(i % 200);
                y = height * (1f / 200f) * (float)(i / 200);
                //array[i] = Color.Lerp(Color.Red, Color.Black, mountainChance.getValueAt(new Vector2(x, y)));

                //por deifeito a cor é um misto dos dois componentes
                array[i] = Color.Lerp(Color.Black, Color.Salmon, (1f - array3[i % 200, i / 200]) * (1f - array2[i % 200, i / 200]));


                //marcar zonas criticas
                if (array2[i % 200, i / 200] < 0.06f)
                    if (array3[i % 200, i / 200] < 0.7f)
                        array[i] = Color.Red;

                ////Chance multiplicativa mista não é viavel
                //if (array2[i % 200, i / 200] < 0.15f)
                //    if (array3[i % 200, i / 200] < 0.8f)
                //        if((1f - array3[i % 200, i / 200]) * (1f - array2[i % 200, i / 200]) > 0.3f)
                //            array[i] = Color.Red;
            }
            Microsoft.Xna.Framework.Graphics.Texture2D text = new Microsoft.Xna.Framework.Graphics.Texture2D(Graphics.device, 200, 200);
            text.SetData<Color>(array);

            return text;
        }

        private Microsoft.Xna.Framework.Graphics.Texture2D getTreeChanceTexture()
        {
            float[,] array2 = treeChanceNoise.getValues(new Point(200, 200), Vector2.Zero, new Vector2(width, height));
            Color[] array = new Color[200 * 200];
            float x, y;
            for (int i = 0; i < array.Length; i++)
            {
                x = width * (1f / 200f) * (float)(i % 200);
                y = height * (1f / 200f) * (float)(i / 200);
                //array[i] = Color.Lerp(Color.Red, Color.Black, mountainChance.getValueAt(new Vector2(x, y)));

                //por deifeito a cor é um misto dos dois componentes
                array[i] = Color.Lerp(Color.Black, Color.LawnGreen, array2[i%200,i/200]);


                //marcar zonas criticas
                if (array2[i % 200, i / 200] > 0.6f)
                        array[i] = Color.LawnGreen;

                ////Chance multiplicativa mista não é viavel
                //if (array2[i % 200, i / 200] < 0.15f)
                //    if (array3[i % 200, i / 200] < 0.8f)
                //        if((1f - array3[i % 200, i / 200]) * (1f - array2[i % 200, i / 200]) > 0.3f)
                //            array[i] = Color.Red;
            }
            Microsoft.Xna.Framework.Graphics.Texture2D text = new Microsoft.Xna.Framework.Graphics.Texture2D(Graphics.device, 200, 200);
            text.SetData<Color>(array);

            return text;
        }

        public override void Update(UpdateState state)
        {
            base.Update(state);
            LoadRequiredContent();
        }
        private void LoadRequiredContent()
        {
            foreach (LandContentRequirer requester in contentRequirers)
            {
                foreach (TerrainTile tile in tiles)
                {
                    if ((tile.position - requester.LandContentRequirerPosition(this)).Length() < tileSize.Length() / 2f + requester.LandContentRequirerRadius(this))
                    {
                        tile.loadTexture();
                    }
                }

                foreach (Mountain mountain in mountainList)
                {
                    if ((mountain.position - requester.LandContentRequirerPosition(this)).Length() < requester.LandContentRequirerRadius(this) + mountain.radius)
                        mountain.loadTexture();
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

            for (int i = 0; i < mountainList.Count; i++)
                if ((mountainList[i].position - state.currentCamera.position).Length() < state.currentCamera.diagonal.Length() + mountainList[i].radius)
                    mountainList[i].Draw(state);

            for (int i = 0; i < treeList.Count; i++)
                if ((treeList[i].position - state.currentCamera.position).Length() < state.currentCamera.diagonal.Length() + treeList[i].radius)
                    treeList[i].Draw(state);

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
