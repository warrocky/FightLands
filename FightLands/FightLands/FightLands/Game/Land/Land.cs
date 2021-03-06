﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;

namespace FightLands
{
    class Land : World
    {
        private Object beingLoadedLock = new Object();
        private bool beingLoaded;

        private Object loadedLock = new Object();
        private bool loaded;

        private Object checkLoadStatusLock = new Object();
        private String loadStatus = "Unloaded";

        Zone[,] zones;
        int horizontalZoneCount;
        int verticalZoneCount;
        Vector2 zoneSize;

        public int widthInTiles;
        public int heightInTiles;
        public Vector2 tileSize;


        //Propertys
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

        int seed;


        //Topography defining noises and values
        public Noise grassLandGreeness;
        public Noise dirtPatches;

        public Noise mountainChanceNoise;
        public float mountainChanceUpperValue;
        float[,] mountainChanceValues;
        public Noise mountainChainNoise;
        public float mountainChainUpperValue;
        float[,] mountainChainValues;

        public Noise treeChanceNoise;
        public float treeChanceTreshold;
        float[,] treeChanceValues;

        public Noise waterChanceNoise;
        public float waterChanceTreshold;
        public float beachPercentileTreshold;
        float[,] waterChanceValues;
        public Noise3D waterWavesNoise;
        public float waterWavesDistancePeriod;
        public int waterWavesFrameCount;
        public Point3 waterWavesLoop;
        public float[, ,] waterWaveValues;
        public float waterWavesPhase;
        public float waterWavesTimePeriod;

        Point terrainEvaluationPrecision;


        //Map structure
        TerrainTile[,] tiles;
        List<WaterTile> waterTiles;
        List<Mountain> mountainList;
        List<Tree> treeList;
        List<Town> townList;
        List<LandCreature> mobList;

        //Content structures
        List<AssetTexture> treeTextures;

        
        //Life Managers
        public FactionManager factionManager;
        
        //dynamic structures
        List<ZoneChangingObject> zoneChanges;

        List<LandContentRequirer> contentRequirers;
        List<LandUpdateNode> updateNodes;

        public Land(int seed)
        {
            this.seed = seed;
            Random rdm = new Random(seed);

            horizontalZoneCount = 5;
            verticalZoneCount = 5;
            widthInTiles = 10*horizontalZoneCount; //10 width tiles per zone
            heightInTiles = 10*verticalZoneCount; //10 height tiles per zone
            tileSize = new Vector2(100f, 100f);
            zoneSize = new Vector2(width / (float)horizontalZoneCount, height / (float)verticalZoneCount);
            zones = new Zone[horizontalZoneCount,verticalZoneCount];
            terrainEvaluationPrecision = new Point((int)(width / 8f), (int)(height / 8f)); //cant be changed here (WHY?: arrays are being created by cowboying)



            zoneChanges = new List<ZoneChangingObject>();

            contentRequirers = new List<LandContentRequirer>();
            updateNodes = new List<LandUpdateNode>();


            Vector2 zonePosition;
            for(int i=0;i<horizontalZoneCount;i++)
                for (int j = 0; j < verticalZoneCount; j++)
                {
                    zonePosition = new Vector2(i * (width / (float)horizontalZoneCount), j * height / (float)verticalZoneCount) - new Vector2(width, height) / 2f + zoneSize/2f;
                    zones[i, j] = new Zone(zonePosition, zoneSize.Length()/2f,this,i,j);
                    zones[i, j].effectRadius = zones[i, j].zoneRadius;
                }


            //Topografy defining Noises
            grassLandGreeness = Noise.RegularNoise(800f, 2, rdm.Next());
            grassLandGreeness.filter = (float a, Vector2 b) => (a*1.5f + 1f)/2f;

            dirtPatches = Noise.TurbulenceNoise(800f, 6, rdm.Next());
            dirtPatches.filter = (float a, Vector2 b) => ((-a) * 1.5f + 1f) / 2f;

            mountainChanceNoise = Noise.RegularNoise(width / 4f, 1, rdm.Next());
            mountainChanceNoise.filter = (float a, Vector2 b) => (a + 1f)/2f;

            mountainChainNoise = Noise.TurbulenceNoise(width/2f, 3, rdm.Next());
            mountainChainNoise.filter = (float a, Vector2 b) => (a + 1)/2f;

            treeChanceNoise = Noise.RegularNoise(width / 4f, 2, rdm.Next());
            treeChanceNoise.filter = (float a, Vector2 b) => (a + 1f) / 2f;

            float waterMassesPeriodFactor = (float)rdm.NextDouble() + 0.25f;
            waterChanceNoise = Noise.RegularNoise(width*waterMassesPeriodFactor, 6, rdm.Next());
            waterChanceNoise.filter = (float a, Vector2 b) => (a + 1f) / 2f;

            waterWavesDistancePeriod = 20f; // Convem que FrameCount divida waterWavesDistancePeriod*waterWavesLoop.X para evitar um salto no loop da sprite.
            waterWavesFrameCount = 50;
            waterWavesTimePeriod = 4f;
            waterWavesLoop = new Point3(20, 20, 3);
            waterWavesNoise = Noise3D.TurbulenceNoise(waterWavesDistancePeriod, 2, waterWavesLoop, rdm.Next());
            waterWavesNoise.filter = (float a, Vector3 b) => (a + 1) / 2f;


            //topography defining values
            mountainChanceUpperValue = 0.75f;
            mountainChainUpperValue = 0.1f;

            treeChanceTreshold = 0.6f;

            waterChanceTreshold = 0.6f;
            beachPercentileTreshold = 0.985f;


            //map structures
            treeTextures = new List<AssetTexture>();
            tiles = new TerrainTile[widthInTiles, heightInTiles];
            waterTiles = new List<WaterTile>();
            mountainList = new List<Mountain>();
            treeList = new List<Tree>();
            townList = new List<Town>();
            mobList = new List<LandCreature>();


            //Life managers
            factionManager = new FactionManager(this);
        }

        public bool isBeingLoaded()
        {
            lock (beingLoadedLock)
            {
                return beingLoaded;
            }
        }

        public bool isLoaded()
        {
            lock (loadedLock)
            {
                return loaded;
            }
        }

        private void setLoadStatus(String status)
        {
            lock (checkLoadStatusLock)
            {
                loadStatus = status;
            }
        }
        public String checkLoadStatus()
        {
            String returnValue;

            lock (checkLoadStatusLock)
            {
                returnValue = loadStatus;
            }

            return returnValue;
        }

        public void loadMap()
        {
            if (isBeingLoaded() || isLoaded())
                throw new Exception("Attempt at loading a map twice.");

            Thread mapLoader = new Thread(new ThreadStart(startMapLoad));
            mapLoader.Start();
        }

        private void startMapLoad()
        {
            lock (beingLoadedLock)
            {
                lock (loadedLock)
                {
                    if (beingLoaded || loaded)
                        throw new Exception("Attempt at loading map twice.(internal)");

                    beingLoaded = true;
                }
            }

            setLoadStatus("Evaluating Noises");
            noiseEvaluations();

            setLoadStatus("Creating Tiles");
            createTiles();

            setLoadStatus("Loading Mountains");
            loadMountains();

            setLoadStatus("Loading Trees");
            loadTrees();

            setLoadStatus("Loading Towns");
            loadTowns();

            setLoadStatus("Loading Mobs");
            loadMobs();

            lock (beingLoadedLock)
            {
                lock (loadedLock)
                {
                    beingLoaded = false;
                    loaded = true;
                }
            }
        }

        private void noiseEvaluations()
        {
            //Water evaluation
            waterChanceValues = waterChanceNoise.getValues(new Point((int)(width / 8f), (int)(height / 8f)), Vector2.Zero - new Vector2(width, height) / 2f, new Vector2(width, height));
            // WARNING: mountainChainValues and mountainChanceValues must have same array size because of evaluation function using same array coordinates for both.
            mountainChainValues = mountainChainNoise.getValues(new Point((int)(width / 8f), (int)(height / 8f)), Vector2.Zero - new Vector2(width, height) / 2f, new Vector2(width, height));
            mountainChanceValues = mountainChanceNoise.getValues(new Point((int)(width / 8f), (int)(height / 8f)), Vector2.Zero - new Vector2(width, height) / 2f, new Vector2(width, height));
            treeChanceValues = treeChanceNoise.getValues(new Point((int)(width / 8f), (int)(height / 8f)), Vector2.Zero - new Vector2(width, height) / 2f, new Vector2(width, height));

            Point3 waterWavesSampling = new Point3((int)(waterWavesDistancePeriod * waterWavesLoop.X), (int)(waterWavesDistancePeriod * waterWavesLoop.Y), waterWavesFrameCount);
            float waterWaveZArea = waterWavesDistancePeriod * waterWavesLoop.Z - waterWavesDistancePeriod * waterWavesLoop.Z / waterWavesFrameCount; //to destroy repeated last frame
            Vector3 waterWavesArea = new Vector3(waterWavesDistancePeriod * waterWavesLoop.X, waterWavesDistancePeriod * waterWavesLoop.Y, waterWaveZArea);
            waterWaveValues = waterWavesNoise.getValues(waterWavesSampling, Vector3.Zero, waterWavesArea);
        }

        private void createTiles()
        {
            Random rdm = new Random(seed + 3);

            float[,] waterTileChance = waterChanceNoise.getValues(new Point(widthInTiles, heightInTiles), Vector2.Zero - new Vector2(width, height) / 2f, new Vector2(width, height));

            //Tile creation
            Vector2 center = new Vector2(width, height) / 2f;
            Vector2 tilePosition;
            for (int i = 0; i < widthInTiles; i++)
                for (int j = 0; j < heightInTiles; j++)
                {
                    tilePosition = new Vector2(i * tileWidth, j * tileHeight) - center + tileSize / 2f;
                    tiles[i, j] = new TerrainTile(TerrainTile.TerrainType.Grassland, tilePosition, tileSize, this, rdm.Next());

                    if (waterTileChance[i, j] > waterChanceTreshold * 0.9f)
                    {
                        waterTiles.Add(new WaterTile(tilePosition, tileSize, this, rdm.Next()));
                    }
                }
        }

        private void loadMountains()
        {
            Random rdm = new Random(seed + 101);

            //Mountain Creation
            Mountain m1 = null;
            Vector2 mountainPos;
            float radius, dist, minDist = float.MaxValue;
            int mountainTryCounter = 0;
            Vector2 mountainChanceAndChain;
            float mountainDist;
            for (int i = 0; i < widthInTiles * heightInTiles; i++)
            {
                mountainTryCounter = 5;
                do
                {
                    m1 = null;

                    //mountainPos = new Vector2((i % widthInTiles) * tileSize.X, (i / widthInTiles) * tileSize.Y);

                    //arrayX = (int)((mountainPos.X / width) * 200f);
                    //arrayY = (int)((mountainPos.Y / height) * 200f);

                    mountainPos = tiles[(i % widthInTiles), (i / widthInTiles)].position;
                    mountainPos += new Vector2((float)rdm.NextDouble() * tileWidth, (float)rdm.NextDouble() * tileHeight);// -new Vector2(width, height) / 2f;

                    mountainChanceAndChain = getMountainChanceAndChainValue(mountainPos);


                    //check if water
                    if (checkIfBeachOrWater(mountainPos))
                    {
                        //TODO WaterMountains
                    }
                    else if (mountainChanceAndChain.X < mountainChanceUpperValue && mountainChanceAndChain.Y < mountainChainUpperValue)
                    {
                        minDist = float.MaxValue;

                        //search the smallest distance to a mountain
                        for (int j = 0; j < mountainList.Count; j++)
                        {
                            dist = (mountainList[j].position - mountainPos).Length() - mountainList[j].radius * 0.7f;
                            if (dist < minDist)
                                minDist = dist;
                        }

                        //test value to avoid collision among mountains - should be 0
                        if (minDist > 20f)
                        {
                            mountainDist = (mountainChanceUpperValue - mountainChanceAndChain.X) / (mountainChanceUpperValue);
                            if (mountainDist > (mountainChainUpperValue - mountainChanceAndChain.Y) / (mountainChainUpperValue))
                                mountainDist = (mountainChainUpperValue - mountainChanceAndChain.Y) / (mountainChainUpperValue);

                            //Create random radius
                            //radius = MathHelper.getNextNormalDistributedFloat(3f, 1f, rdm) * (10f + 20f * (1f - mountainChanceAndChain.Y));
                            radius = MathHelper.getNextNormalDistributedFloat(3f, 1f, rdm) * (10f + 20f * (mountainDist));

                            //if radius collides with other mountain, reduce the radius.
                            if (radius > minDist)
                                if (minDist > 20f)
                                    radius = minDist;
                                else
                                    radius = 20f;

                            //Create new mountain
                            m1 = new Mountain(rdm.Next(), this, radius);
                            m1.position = mountainPos;
                            //objectAddList.Remove(m1);
                            mountainList.Add(m1);
                        }
                    }

                    //se não foi gerada nenhuma montanha, reduzir um valor no counter.
                    if (m1 == null)
                        mountainTryCounter--;
                } while (mountainTryCounter != 0);
            }
        }

        private void loadTrees()
        {
            //tree generation
            generateTreeTextures();

            Random rdm = new Random(seed + 51);

            Tree t1 = null;
            Vector2 treePos;
            float minDist = float.MaxValue;
            float dist;
            int treeTryCounter = 0;
            float treeChance, radius;
            for (int i = 0; i < widthInTiles * heightInTiles; i++)
            {
                treeTryCounter = 5;
                do
                {
                    t1 = null;

                    treePos = tiles[(i % widthInTiles), (i / widthInTiles)].position;
                    treePos += new Vector2((float)rdm.NextDouble() * tileWidth, (float)rdm.NextDouble() * tileHeight);// -new Vector2(width, height) / 2f;

                    treeChance = getTreeChance(treePos);

                    //check if water
                    if (checkIfBeachOrWater(treePos))
                    {
                        //TODO: water trees
                    }
                    else if (treeChance > treeChanceTreshold)
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
                            radius = (MathHelper.getNextNormalDistributedFloat(4, 1, rdm)) * 10f;

                            //if radius collides with other tree, reduce the radius.
                            if (radius > minDist)
                                if (minDist > 20f)
                                    radius = minDist;
                                else
                                    radius = 20f;

                            //Create new tree
                            t1 = new Tree(rdm.Next(), radius, this);
                            t1.position = treePos;
                            //objectAddList.Remove(t1);
                            treeList.Add(t1);
                        }
                    }

                    //se não foi gerada nenhuma montanha, reduzir um valor no counter.
                    if (t1 == null)
                        treeTryCounter--;
                } while (treeTryCounter != 0);
            }
        }

        private void loadTowns()
        {
            //Town Creation
            Vector2 townPosition;
            int townID = 0;
            AssetManager.CreateAssetSpriteFont("townLabelFont", "defaultFont");

            Random rdm = new Random(seed + 3);

            float minDist, dist;
            for (int i = 0; i < widthInTiles * heightInTiles; i++)
            {

                townPosition = tiles[(i % widthInTiles), (i / widthInTiles)].position;
                townPosition += new Vector2((float)rdm.NextDouble() * tileWidth, (float)rdm.NextDouble() * tileHeight);// -new Vector2(width, height) / 2f;

                //check if water
                if (checkIfBeachOrWater(townPosition))
                {
                    //TODO: water towns
                }
                else if (rdm.NextDouble() < 0.03f)
                {
                    minDist = float.MaxValue;

                    //search the smallest distance to a tree or mountain or town
                    for (int j = 0; j < treeList.Count; j++)
                    {
                        dist = (treeList[j].position - townPosition).Length() - treeList[j].radius * 0.7f;
                        if (dist < minDist)
                            minDist = dist;
                    }
                    for (int j = 0; j < mountainList.Count; j++)
                    {
                        dist = (mountainList[j].position - townPosition).Length() - mountainList[j].radius * 0.7f;
                        if (dist < minDist)
                            minDist = dist;
                    }
                    for (int j = 0; j < townList.Count; j++)
                    {
                        dist = (townList[j].position - townPosition).Length() - 500f; //500f is the lowest distance a town is allowed to have to another one
                        if (dist < minDist)
                            minDist = dist;
                    }

                    //check if distance is enough to create another town
                    if (minDist > 50f)
                    {

                        //Create new town
                        townList.Add(new Town(this, townID));
                        townID++;
                        townList[townList.Count - 1].position = townPosition;
                    }
                }
            }

        }

        private void loadMobs()
        {
            //Mob Creation
            Vector2 mobPosition;
            LandCreature mob;
            float minDist, dist;

            Random rdm = new Random(seed + 29);

            for (int i = 0; i < widthInTiles * heightInTiles; i++)
            {

                mobPosition = tiles[(i % widthInTiles), (i / widthInTiles)].position;
                mobPosition += new Vector2((float)rdm.NextDouble() * tileWidth, (float)rdm.NextDouble() * tileHeight);// -new Vector2(width, height) / 2f;

                //check if water
                if (checkIfBeachOrWater(mobPosition))
                {
                    //TODO: water mobs
                }
                else if (rdm.NextDouble() < 0.1f)
                {
                    minDist = float.MaxValue;

                    //search the smallest distance to a tree or mountain or town
                    for (int j = 0; j < treeList.Count; j++)
                    {
                        dist = (treeList[j].position - mobPosition).Length() - treeList[j].radius * 0.4f;
                        if (dist < minDist)
                            minDist = dist;
                    }
                    for (int j = 0; j < mountainList.Count; j++)
                    {
                        dist = (mountainList[j].position - mobPosition).Length() - mountainList[j].radius * 0.7f;
                        if (dist < minDist)
                            minDist = dist;
                    }
                    for (int j = 0; j < townList.Count; j++)
                    {
                        dist = (townList[j].position - mobPosition).Length() - 400f; //500f is the lowest distance a mob can be from a town
                        if (dist < minDist)
                            minDist = dist;
                    }

                    //check if distance is enough to create a mob
                    if (minDist > 25f)
                    {

                        //Create new mob
                        Rat rattie = new Rat(this);
                        mob = new LandRat(this, rdm.Next(),rattie);
                        mob.position = mobPosition;
                        mobList.Add(mob);
                    }
                }
            }
        }

        //outdated
        private Microsoft.Xna.Framework.Graphics.Texture2D getMountainChanceTexture()
        {
            float[,] array2 = mountainChainNoise.getValues(new Point(200, 200), Vector2.Zero, new Vector2(width, height));
            float[,] array3 = mountainChanceNoise.getValues(new Point(200, 200), Vector2.Zero, new Vector2(width, height));
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
        //outdated
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

        public Microsoft.Xna.Framework.Graphics.Texture2D getMinimap(Point resolution)
        {
            float[,] waterChanceValues = waterChanceNoise.getValues(resolution, -new Vector2(width,height)/2f, new Vector2(width, height));
            float[,] mountainChanceValues = mountainChanceNoise.getValues(resolution, -new Vector2(width,height)/2f, new Vector2(width, height));
            float[,] mountainChainValues = mountainChainNoise.getValues(resolution, -new Vector2(width,height)/2f, new Vector2(width, height));
            float[,] treeChanceValues = treeChanceNoise.getValues(resolution, -new Vector2(width,height)/2f, new Vector2(width, height));

            Color[] colorArray = new Color[resolution.X*resolution.Y];

            bool[,] towns = new bool[resolution.X, resolution.Y];

            //upper left corner coordinates
            int x, y;

            Vector2 tempPos;
            for (int i = 0; i < townList.Count; i++)
            {
                // upper left corner position
                tempPos = townList[i].position + new Vector2(width, height) * 0.5f - new Vector2(50f,50f);
                // upper left corner coords in texture
                x = (int)MathHelper.Clamp(((tempPos.X / width) * resolution.X), 0, resolution.X);
                y = (int)MathHelper.Clamp(((tempPos.Y / height) * resolution.Y), 0, resolution.Y);

                for(int xx = 0;xx < 10;xx++)
                    for (int yy = 0; yy < 10; yy++)
                    {
                        if (x + xx < resolution.X && y + yy < resolution.Y)
                            towns[x + xx, y + yy] = true;
                    }
            }

            for (int i = 0; i < colorArray.Length; i++)
            {
                x = i % resolution.X;
                y = i / resolution.X;

                if (x == 0 || x == resolution.X - 1 || y == 0 || y == resolution.Y - 1)
                {
                    colorArray[i] = Color.Transparent;
                }
                else if (towns[x, y])
                {
                    colorArray[i] = Color.Orange;
                }
                else if (waterChanceValues[x, y] > waterChanceTreshold)
                {
                    colorArray[i] = Color.LightBlue;
                }
                else if (mountainChanceValues[x, y] < mountainChanceUpperValue && mountainChainValues[x, y] < mountainChainUpperValue)
                {
                    colorArray[i] = Color.Gray;
                }
                else if (treeChanceValues[x, y] > treeChanceTreshold)
                {
                    colorArray[i] = Color.ForestGreen;
                }
                else
                {
                    colorArray[i] = Color.LawnGreen;
                }
            }

            Microsoft.Xna.Framework.Graphics.Texture2D text = new Microsoft.Xna.Framework.Graphics.Texture2D(Graphics.device, resolution.X, resolution.Y);
            text.SetData<Color>(colorArray);

            return text;
        }

        public bool checkIfBeachOrWater(Vector2 position)
        {
            position.X = ((position.X + width / 2f)*(float)waterChanceValues.GetLength(0)) / width;
            position.Y = ((position.Y + height / 2f)*(float)waterChanceValues.GetLength(1)) / height;

            return waterChanceValues[MathHelper.Clamp((int)position.X, 0, waterChanceValues.GetLength(0)-1), MathHelper.Clamp((int)position.Y, 0, waterChanceValues.GetLength(1)-1)] > waterChanceTreshold * beachPercentileTreshold;
        }
        public float getWaterChance(Vector2 position)
        {
            position.X = ((position.X + width / 2f) * (float)waterChanceValues.GetLength(0)) / width;
            position.Y = ((position.Y + height / 2f) * (float)waterChanceValues.GetLength(1)) / height;

            return waterChanceValues[MathHelper.Clamp((int)position.X, 0, waterChanceValues.GetLength(0) - 1), MathHelper.Clamp((int)position.Y, 0, waterChanceValues.GetLength(1) - 1)];
        }

        public bool checkIfMountainArea(Vector2 position)
        {
            position.X = ((position.X + width / 2f) * (float)mountainChanceValues.GetLength(0)) / width;
            position.Y = ((position.Y + height / 2f) * (float)mountainChanceValues.GetLength(1)) / height;

            return mountainChanceValues[MathHelper.Clamp((int)position.X, 0, mountainChanceValues.GetLength(0) - 1), MathHelper.Clamp((int)position.Y, 0, mountainChanceValues.GetLength(1) - 1)] < mountainChanceUpperValue
                && mountainChainValues[MathHelper.Clamp((int)position.X, 0, mountainChainValues.GetLength(0) - 1), MathHelper.Clamp((int)position.Y, 0, mountainChainValues.GetLength(1) - 1)] < mountainChainUpperValue;
        }
        public float getMountainChainValue(Vector2 position)
        {
            position.X = ((position.X + width / 2f) * (float)mountainChanceValues.GetLength(0)) / width;
            position.Y = ((position.Y + height / 2f) * (float)mountainChanceValues.GetLength(1)) / height;

            return mountainChainValues[MathHelper.Clamp((int)position.X, 0, mountainChainValues.GetLength(0) - 1), MathHelper.Clamp((int)position.Y, 0, mountainChainValues.GetLength(1) - 1)];
        }
        public float getMountainChanceValue(Vector2 position)
        {
            position.X = ((position.X + width / 2f) * (float)mountainChanceValues.GetLength(0)) / width;
            position.Y = ((position.Y + height / 2f) * (float)mountainChanceValues.GetLength(1)) / height;

            return mountainChanceValues[MathHelper.Clamp((int)position.X, 0, mountainChanceValues.GetLength(0) - 1), MathHelper.Clamp((int)position.Y, 0, mountainChanceValues.GetLength(1) - 1)];
        }
        public Vector2 getMountainChanceAndChainValue(Vector2 position)
        {
            position.X = ((position.X + width / 2f) * (float)mountainChanceValues.GetLength(0)) / width;
            position.Y = ((position.Y + height / 2f) * (float)mountainChanceValues.GetLength(1)) / height;

            return new Vector2(
                mountainChanceValues[MathHelper.Clamp((int)position.X, 0, mountainChanceValues.GetLength(0) - 1), MathHelper.Clamp((int)position.Y, 0, mountainChanceValues.GetLength(1) - 1)],
                mountainChainValues[MathHelper.Clamp((int)position.X, 0, mountainChainValues.GetLength(0) - 1), MathHelper.Clamp((int)position.Y, 0, mountainChainValues.GetLength(1) - 1)]
                );
        }

        public float getTreeChance(Vector2 position)
        {
            position.X = ((position.X + width / 2f) * (float)treeChanceValues.GetLength(0)) / width;
            position.Y = ((position.Y + height / 2f) * (float)treeChanceValues.GetLength(1)) / height;

            return treeChanceValues[MathHelper.Clamp((int)position.X, 0, treeChanceValues.GetLength(0) - 1), MathHelper.Clamp((int)position.Y, 0, treeChanceValues.GetLength(1) - 1)];
        }


        private void generateTreeTextures()
        {
            Random rdm = new Random(seed);
            for (int i = 1; i <= 20; i++)
            {
                treeTextures.Add(new AssetTexture(Tree.createTexture(rdm.Next(),20 + i * 2f), "landTreeText" + i));
            }
        }
        public AssetTexture getTextureForTree(float radius)
        {
            for (int i = 0; i < treeTextures.Count; i++)
                if (radius * 2f < treeTextures[i].width)
                    return treeTextures[i];

            return treeTextures[treeTextures.Count - 1];
        }

        public override void Update(UpdateState state)
        {
            if (!isLoaded())
                return;

            //base.Update(state);
            waterWavesPhase += state.elapsedTime;

            foreach (LandUpdateNode node in updateNodes)
            {
                foreach (Zone zone in zones)
                {
                    if ((zone.position - node.LandUpdateNodePosition(this)).Length() < zone.effectRadius + node.LandUpdateNodeRadius(this))
                    {
                        zone.Update(state);
                        zone.checkCollisions(state);
                    }
                }
            }


            while (objectAddList.Count != 0)
            {
                getZoneFromPosition(objectAddList[0].position).objectList.Add(objectAddList[0]);
                objectAddList.RemoveAt(0);
            }

            while (objectRemoveList.Count != 0)
            {
                if (!getZoneFromPosition(objectRemoveList[0].position).objectList.Remove(objectRemoveList[0]))
                    foreach (Zone zone in zones)
                        if (zone.objectList.Remove(objectRemoveList[0]))
                            break;

                objectRemoveList.RemoveAt(0);
            }

            while (zoneChanges.Count != 0)
            {
                if (zoneChanges[0].previousZone.objectList.Remove(zoneChanges[0].gameObject))
                    zoneChanges[0].newZone.objectList.Add(zoneChanges[0].gameObject);

                zoneChanges.RemoveAt(0);
            }

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

                foreach (WaterTile tile in waterTiles)
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
            if (!isLoaded())
                return;

            foreach (Zone zone in zones)
            {
                if ((zone.position - state.currentCamera.position).Length() < zone.effectRadius + state.currentCamera.diagonal.Length()/2f)
                    zone.Draw(state);
            }

            //Vector2 drawArea = state.currentCamera.diagonal;
            //Vector2 drawPosition = state.currentCamera.position;

            //drawArea.X /= tileSize.X;
            //drawArea.Y /= tileSize.Y;

            //Point tileArea = new Point((int)Math.Ceiling(drawArea.X) + 2, (int)Math.Ceiling(drawArea.Y) + 2);
            //Point tileCenter = new Point((int)(drawPosition.X/tileWidth) + widthInTiles / 2, (int)(drawPosition.Y/tileHeight) + heightInTiles / 2);

            //int x, y;
            //for (int i = 0; i < tileArea.X; i++)
            //    for (int j = 0; j < tileArea.Y; j++)
            //    {
            //        x = i - tileArea.X / 2 + tileCenter.X;
            //        y = j - tileArea.Y / 2 + tileCenter.Y;

            //        if (x >= 0 && x < widthInTiles && y >= 0 && y < heightInTiles)
            //            tiles[x, y].Draw(state);
            //    }

            //for (int i = 0; i < mountainList.Count; i++)
            //    if ((mountainList[i].position - state.currentCamera.position).Length() < state.currentCamera.diagonal.Length() + mountainList[i].radius)
            //        mountainList[i].Draw(state);

            //for (int i = 0; i < treeList.Count; i++)
            //    if ((treeList[i].position - state.currentCamera.position).Length() < state.currentCamera.diagonal.Length() + treeList[i].radius)
            //        treeList[i].Draw(state);

            //base.Draw(state);
        }

        public Zone getZoneFromPosition(Vector2 position)
        {
            int x = (int)MathHelper.Clamp((position.X + width / 2f) / zoneSize.X, 0, horizontalZoneCount - 1);
            int y = (int)MathHelper.Clamp((position.Y + height / 2f) / zoneSize.Y, 0, verticalZoneCount - 1);

            return zones[x, y];
        }
        /// <summary>
        /// This function retrieves a zona by campling its coords on the beggining and length of the zone matrix.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Zone safeGetZoneFromCoords(int x, int y)
        {
            if (x < 0)
            {
                if (y < 0)
                {
                    return zones[0, 0];
                }
                else if (y >= zones.GetLength(1))
                {
                    return zones[0, zones.GetLength(1) - 1];
                }
                else
                {
                    return zones[0, y];
                }
            }
            else if (x >= zones.GetLength(0))
            {
                if (y < 0)
                {
                    return zones[zones.GetLength(0) - 1, 0];
                }
                else if (y >= zones.GetLength(1))
                {
                    return zones[zones.GetLength(0) - 1, zones.GetLength(1) - 1];
                }
                else
                {
                    return zones[zones.GetLength(0) - 1, y];
                }
            }
            else if (y < 0)
            {
                return zones[x, 0];
            }
            else if (y >= zones.GetLength(1))
            {
                return zones[x, zones.GetLength(1) - 1];
            }
            else
            {
                return zones[x, y];
            }
        }
        public bool zoneExists(int x, int y)
        {
            if (x < 0 || x >= zones.GetLength(0) || y < 0 || y >= zones.GetLength(1))
                return false;
            else
                return true;
        }
        public List<Zone> getAdjacentZones(int zoneCoordsX, int zoneCoordsY)
        {
            List<Zone> adjacents = new List<Zone>();

            if (zoneExists(zoneCoordsX - 1, zoneCoordsY - 1))
                adjacents.Add(zones[zoneCoordsX - 1, zoneCoordsY - 1]);

            if (zoneExists(zoneCoordsX, zoneCoordsY - 1))
                adjacents.Add(zones[zoneCoordsX, zoneCoordsY - 1]);

            if (zoneExists(zoneCoordsX + 1, zoneCoordsY - 1))
                adjacents.Add(zones[zoneCoordsX + 1, zoneCoordsY - 1]);

            if (zoneExists(zoneCoordsX - 1, zoneCoordsY))
                adjacents.Add(zones[zoneCoordsX - 1, zoneCoordsY]);

            if (zoneExists(zoneCoordsX + 1, zoneCoordsY))
                adjacents.Add(zones[zoneCoordsX + 1, zoneCoordsY]);

            if (zoneExists(zoneCoordsX - 1, zoneCoordsY + 1))
                adjacents.Add(zones[zoneCoordsX - 1, zoneCoordsY + 1]);

            if (zoneExists(zoneCoordsX, zoneCoordsY + 1))
                adjacents.Add(zones[zoneCoordsX, zoneCoordsY + 1]);

            if (zoneExists(zoneCoordsX + 1, zoneCoordsY + 1))
                adjacents.Add(zones[zoneCoordsX + 1, zoneCoordsY + 1]);

            return adjacents;
        }
        public void changeObjectFromZone(Zone previousZone, Zone newZone, GameObject gameObject)
        {
            ZoneChangingObject change = new ZoneChangingObject();
            change.previousZone = previousZone;
            change.newZone = newZone;
            change.gameObject = gameObject;

            zoneChanges.Add(change);
        }
        public class Zone
        {
            public readonly Land land;
            public List<GameObject> objectList;
            public readonly Vector2 position;
            public float effectRadius;
            public readonly float zoneRadius;
            public readonly int zoneCoordsX;
            public readonly int zoneCoordsY;

            public Zone(Vector2 position, float zoneRadius, Land land, int zoneCoordsX, int zoneCoordsY)
            {
                this.land = land;
                this.position = position;
                this.zoneRadius = zoneRadius;
                this.zoneCoordsX = zoneCoordsX;
                this.zoneCoordsY = zoneCoordsY;

                objectList = new List<GameObject>();
            }

            public void Update(UpdateState state)
            {
                Zone newZone;
                for (int i = 0; i < objectList.Count; i++)
                {
                    objectList[i].Update(state);
                    newZone = land.getZoneFromPosition(objectList[i].position);
                    if (newZone != this)
                    {
                        land.changeObjectFromZone(this, newZone, objectList[i]);
                    }
                }
            }
            public void checkCollisions(UpdateState state)
            {
                List<Zone> adjacentZones = land.getAdjacentZones(zoneCoordsX, zoneCoordsY);
                LandObject checkingObject;
                LandObject reactionObject;

                for (int i = 0; i < objectList.Count; i++)
                {
                    checkingObject = (LandObject)objectList[i];
                    if (checkingObject.physicalProperties != null && checkingObject.physicalProperties.activelyColliding)
                    {
                        for (int j = 0; j < objectList.Count; j++)
                        {
                            if (j == i)
                                continue;

                            reactionObject = (LandObject)objectList[j];

                            if (reactionObject.physicalProperties != null)
                            {
                                if ((reactionObject.position - checkingObject.position).Length() < checkingObject.physicalProperties.radius + reactionObject.physicalProperties.radius)
                                {
                                    if (reactionObject.AuthorizeCollision(checkingObject) && checkingObject.AuthorizeCollision(reactionObject))
                                    {
                                        checkingObject.CollideEffect(reactionObject);
                                        reactionObject.CollideEffect(checkingObject);
                                        PhysicalEngine.processLandCollision(checkingObject, reactionObject);
                                    }
                                }
                            }
                        }

                        for (int k = 0; k < adjacentZones.Count;k++)
                        {
                            for(int j=0;j<adjacentZones[k].objectList.Count;j++)
                            {
                                reactionObject = (LandObject)adjacentZones[k].objectList[j];
                                if (reactionObject.physicalProperties != null)
                                {
                                    if ((reactionObject.position - checkingObject.position).Length() < checkingObject.physicalProperties.radius + reactionObject.physicalProperties.radius)
                                    {
                                        if (reactionObject.AuthorizeCollision(checkingObject) && checkingObject.AuthorizeCollision(reactionObject))
                                        {
                                            checkingObject.CollideEffect(reactionObject);
                                            reactionObject.CollideEffect(checkingObject);
                                            PhysicalEngine.processLandCollision(checkingObject, reactionObject);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            public void Draw(DrawState state)
            {
                for (int i = 0; i < objectList.Count; i++)
                    objectList[i].Draw(state);
            }
        }
        public struct ZoneChangingObject
        {
            public Zone previousZone;
            public Zone newZone;
            public GameObject gameObject;
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

        public List<T> findObjectsInArea<T>(Vector2 position, float radius) where T : LandObject
        {
            List<Zone> areaZones = getZonesFromArea(position, radius);
            List<T> objectsInArea = new List<T>();

            Zone testZone;
            for (int i = 0; i < areaZones.Count; i++)
            {
                testZone = areaZones[i];
                for (int j = 0; j < testZone.objectList.Count; j++)
                {
                    if (testZone.objectList[j] is T)
                    {
                        if ((position - testZone.objectList[j].position).Length() < radius)
                        {
                            objectsInArea.Add((T)testZone.objectList[j]);
                        }
                    }
                }
            }

            return objectsInArea;
        }
        public List<Zone> getZonesFromArea(Vector2 position, float radius)
        {
            //TODO: optimization
            List<Zone> areaZones = new List<Zone>();
            Vector2 basePosition = position - new Vector2(radius, radius);
            Zone baseZone = getZoneFromPosition(basePosition);

            int xLength = (int)((2f*radius)/zoneSize.X) + 2;
            int yLength = (int)((2f*radius)/zoneSize.Y) + 2;
            Zone testZone;
            for (int i = 0; i < xLength; i++)
            {
                if (i + baseZone.zoneCoordsX < zones.GetLength(0))
                {
                    for (int j = 0; j < yLength; j++)
                    {
                        if (j + baseZone.zoneCoordsY < zones.GetLength(1))
                        {
                            testZone = zones[i + baseZone.zoneCoordsX, j + baseZone.zoneCoordsY];
                            if ((testZone.position - position).Length() < radius + testZone.zoneRadius)
                            {
                                areaZones.Add(testZone);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return areaZones;
        }
    }
}
