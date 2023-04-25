using System.Collections;
using System.Collections.Generic;
using System.Data;

using TMPro;

using UnityEngine;

namespace NDR2DWorldGenerator
{
    public class TerrainGeneration : MonoBehaviour
    {
        [SerializeField] private Biome[] biomes;

        [Header("Tile Atlas")]
        [SerializeField] private TileAtlas tileAtlas;

        [Header("Biomes Properties")]
        [SerializeField] float biomeFrequency;
        [SerializeField] Gradient biomeGradient;
        [SerializeField] Texture2D biomeMap;

        [Header("Generation Properties")]
        [SerializeField] private int chunkSize = 60;
        [SerializeField] private int worldSize = 100;
        [SerializeField] private float heightAddition = 25;

        private float seed;

        private GameObject[] worldChunks;
        private List<Vector2> worldTiles = new List<Vector2>();
        private Biome currentBiome;

        private void Start()
        {
            GenerateMap();
        }

        private void OnValidate()
        {
            DrawTextures();
            DrawCavesAndOres();
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RemoveChunks();
                GenerateMap();
            }
        }

        private void DrawCavesAndOres()
        {
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    currentBiome = GetCurrentBiome(x, y);
                    float v = Mathf.PerlinNoise((x + seed) * currentBiome.caveFrequency,
                        (y + seed) * currentBiome.caveFrequency);
                    if (v > currentBiome.surfaceValue)
                        currentBiome.caveNoiseTexture.SetPixel(x, y, Color.white);
                    else
                        currentBiome.caveNoiseTexture.SetPixel(x, y, Color.black);

                }
            }
            for (int i = 0; i < biomes.Length; i++)
            {
                biomes[i].caveNoiseTexture.Apply();
            }
        }

        private void GenerateMap()
        {
            seed = Random.Range(-10000, 10000);
            DrawTextures();
            DrawCavesAndOres();
            CreateChunk();
            GenerateTerrain();
        }

        private void DrawTextures()
        {
            biomeMap = new Texture2D(worldSize, worldSize);
            DrawBiomeTexture();

            for (int i = 0; i < biomes.Length; i++)
            {
                biomes[i].caveNoiseTexture = new Texture2D(worldSize, worldSize);
                GenerateNoiseTexture(biomes[i].caveFrequency, biomes[i].surfaceValue, biomes[i].caveNoiseTexture);


                //   for (int ore = 0; ore < biomes[i].ores.Length; ore++)

                for (int ore = 0; ore < biomes[i].ores.Length; ore++)
                {
                    biomes[i].ores[ore].spreadTexture = new Texture2D(worldSize, worldSize);
                    GenerateNoiseTexture(biomes[i].ores[ore].rarity, biomes[i].ores[ore].size, biomes[i].ores[ore].spreadTexture);
                }
            }
        }
        public void DrawBiomeTexture()
        {
            for (int x = 0; x < biomeMap.width; x++)
            {
                for (int y = 0; y < biomeMap.height; y++)
                {
                    float v = Mathf.PerlinNoise((x + seed) * biomeFrequency, (y + seed) * biomeFrequency);
                    Color color = biomeGradient.Evaluate(v);
                    biomeMap.SetPixel(x, y, color);
                }
            }

            biomeMap.Apply();
        }

        public void CreateChunk()
        {
            int numChunks = worldSize / chunkSize;
            worldChunks = new GameObject[numChunks];

            for (int i = 0; i < numChunks; i++)
            {
                GameObject newChunk = new GameObject();
                newChunk.name = "Chunk " + i;
                newChunk.transform.parent = transform;
                worldChunks[i] = newChunk;
            }
        }

        public void RemoveChunks()
        {
            int numChunks = worldSize / chunkSize;

            for (int i = 0; i < numChunks; i++)
            {
                Destroy(worldChunks[i]);
            }

            worldTiles = new List<Vector2>();
        }

        public Biome GetCurrentBiome(int x, int y)
        {
            for (int i = 0; i < biomes.Length; i++)
                if (biomes[i].biomeColor == biomeMap.GetPixel(x, y))
                    return biomes[i];

            return currentBiome;
        }

        public void GenerateTerrain()
        {
            Sprite[] tileSprites;

            for (int x = 0; x < worldSize; x++)
            {
                currentBiome = GetCurrentBiome(x, 0);
                float height = Mathf.PerlinNoise((x + seed) * currentBiome.terrianFrequency, seed * currentBiome.terrianFrequency)
                    * currentBiome.heightMultiplier + heightAddition;

                for (int y = 0; y < height; y++)
                {

                    if (y < height - currentBiome.dirtLayerHeight)
                    {
                        currentBiome = GetCurrentBiome(x, y);

                        tileSprites = currentBiome.tileAtlas.stone.tileSprites;

                        if (currentBiome.ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y < currentBiome.ores[0].maxSpawnHeight)
                            tileSprites = currentBiome.tileAtlas.coal.tileSprites;

                        if (currentBiome.ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y < currentBiome.ores[1].maxSpawnHeight)
                            tileSprites = currentBiome.tileAtlas.iron.tileSprites;

                        if (currentBiome.ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y < currentBiome.ores[2].maxSpawnHeight)
                            tileSprites = currentBiome.tileAtlas.gold.tileSprites;

                        if (currentBiome.ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y < currentBiome.ores[3].maxSpawnHeight)
                            tileSprites = currentBiome.tileAtlas.diamond.tileSprites;
                    }
                    else if (y < height - 1)
                    {
                        tileSprites = currentBiome.tileAtlas.dirt.tileSprites;
                    }
                    else // top layer
                    {
                        tileSprites = currentBiome.tileAtlas.grass.tileSprites;
                    }

                    if (currentBiome.isGeneratingCavesAllowed)
                    {
                        if (currentBiome.caveNoiseTexture.GetPixel(x, y).r < currentBiome.surfaceValue)
                        {
                            CreateTile(x, y, tileSprites);
                        }
                    }
                    else
                    {
                        CreateTile(x, y, tileSprites);
                    }

                    if (y >= height - 1)
                    {
                        int treeChanceOutcome = Random.Range(0, currentBiome.treeChance);
                        treeChanceOutcome = 0;
                        if (treeChanceOutcome == 1)
                        {
                            if (worldTiles.Contains(new Vector2(x, y)))
                                CreateTree(Random.Range(currentBiome.minimumTreeHeight, currentBiome.maximumTreeHeight), x, y + 1);
                        }
                        else
                        {
                            int i = Random.Range(0, currentBiome.tallGrassChance);
                            if (i == 1)
                                if (worldTiles.Contains(new Vector2(x, y)))
                                    if (currentBiome.tileAtlas.tallGrass != null)
                                        CreateTile(x, y + 1, currentBiome.tileAtlas.tallGrass.tileSprites);
                        }
                    }
                }
            }
        }

        private void CreateTree(int treeHeight, int x, int y)
        {
            CreateTile(x + 1, y, tileAtlas.tree_Start_Right.tileSprites);
            CreateTile(x, y, tileAtlas.tree_Start.tileSprites);
            CreateTile(x - 1, y, tileAtlas.tree_Start_Left.tileSprites);

            for (int i = 0; i < treeHeight; i++)
                CreateTile(x, y + i, tileAtlas.tree_middle.tileSprites);

            Vector2Int leaf = new Vector2Int(x, y + treeHeight);

            CreateTile(leaf.x, leaf.y, tileAtlas.leaf.tileSprites);
            leaf += Vector2Int.right;
            CreateTile(leaf.x, leaf.y, tileAtlas.leaf.tileSprites);
            leaf += Vector2Int.left;
            CreateTile(leaf.x, leaf.y, tileAtlas.leaf.tileSprites);
            leaf += Vector2Int.up;
            CreateTile(leaf.x, leaf.y, tileAtlas.leaf.tileSprites);
            leaf = new Vector2Int(x - 1, y + treeHeight);
            CreateTile(leaf.x, leaf.y, tileAtlas.leaf.tileSprites);
            leaf = new Vector2Int(x - 1, y + treeHeight - 1);
            CreateTile(leaf.x, leaf.y, tileAtlas.leaf.tileSprites);
            leaf = new Vector2Int(x + 1, y + treeHeight - 1);
            CreateTile(leaf.x, leaf.y, tileAtlas.leaf.tileSprites);
            leaf = new Vector2Int(x + 1, y + treeHeight - 2);
            CreateTile(leaf.x, leaf.y, tileAtlas.leaf.tileSprites);
            leaf = new Vector2Int(x - 1, y + treeHeight - 2);
            CreateTile(leaf.x, leaf.y, tileAtlas.leaf.tileSprites);

            leaf = new Vector2Int(x - 1, y + treeHeight + 1);
            CreateTile(leaf.x, leaf.y, tileAtlas.leaf.tileSprites);
            leaf = new Vector2Int(x + 1, y + treeHeight + 1);
            CreateTile(leaf.x, leaf.y, tileAtlas.leaf.tileSprites);
            leaf = new Vector2Int(x, y + treeHeight + 2);
            CreateTile(leaf.x, leaf.y, tileAtlas.leaf.tileSprites);


            int numberOfBranches = Random.Range(1, treeHeight - 1);
            for (int i = 0; i < numberOfBranches; i++)
            {
                int xOffset = 1;

                if (Random.value < 0.5f)
                {
                    xOffset = -1;
                }

                int newY = y + Random.Range(3, treeHeight - 1);
                int newX = x + xOffset;

                CreateTile(newX, newY, tileAtlas.tree_Branch.tileSprites, xOffset == -1);
                CreateTile(newX + 1, (newY - 1) + 1, tileAtlas.tree_Branch_End_Lower.tileSprites, xOffset == -1);
                CreateTile(newX + 1, newY + 1, tileAtlas.tree_Branch_End_Upper.tileSprites, xOffset == -1);

            }

            return;

            for (int i = 0; i < treeHeight; i++)
            {
                CreateTile(x, y + i, tileAtlas.tree_middle.tileSprites);
                int leafChance = Random.Range(1, 10);
                if (leafChance > 1)
                {
                    for (int j = 0; j < Random.Range(2, 10); j++)
                        CreateTile(x, y + treeHeight + i, tileAtlas.leaf.tileSprites);
                }
            }

            for (int i = 0; i < Random.Range(1, 10); i++)
            {
                for (int j = 0; j < Random.Range(2, 10); j++)
                {
                    CreateTile(x + i, y + treeHeight + j, tileAtlas.leaf.tileSprites);

                }
            }
            for (int i = 0; i < Random.Range(1, 10); i++)
            {
                for (int j = 0; j < Random.Range(2, 10); j++)
                {
                    CreateTile(x - i, y + treeHeight + j, tileAtlas.leaf.tileSprites);

                }
            }
        }

        private void CreateTile(int x, int y, Sprite[] tileSprites, bool invertTheX = false)
        {
            if (!worldTiles.Contains(new Vector2(x, y)))
            {
                GameObject newTile = new GameObject();

                float chunkCoord = (Mathf.Round(x / chunkSize) * chunkSize);

                chunkCoord /= chunkSize;


                newTile.transform.parent = worldChunks[(int)chunkCoord].transform;

                newTile.AddComponent<SpriteRenderer>();
                SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>();
                renderer.sprite = tileSprites[Random.Range(0, tileSprites.Length)];
                newTile.name = tileSprites[0].name + x + ", " + y;
                newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
                
                renderer.flipX = invertTheX;

                worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
            }
        }

        private void GenerateNoiseTexture(float frequency, float limit, Texture2D noiseTexture)
        {
            for (int x = 0; x < noiseTexture.width; x++)
            {
                for (int y = 0; y < noiseTexture.height; y++)
                {
                    float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                    if (v > limit)
                        noiseTexture.SetPixel(x, y, Color.white);
                    else
                        noiseTexture.SetPixel(x, y, Color.black);

                }
            }

            noiseTexture.Apply();
        }
    }
}