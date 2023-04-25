using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NDR2DWorldGenerator
{
    [System.Serializable]
    public class Biome
    {
        public string biomeName;

        public Color biomeColor;

        public TileAtlas tileAtlas;
        
        [Header("Noise Properties")]
        public float caveFrequency = 0.05f;
        public float terrianFrequency = 0.05f;
        public Texture2D caveNoiseTexture;

        [Header("Generation Properties")]
        public bool isGeneratingCavesAllowed = true;
        public int dirtLayerHeight = 5;
        public float surfaceValue = 0.25f;
        public float heightMultiplier = 4f;

        [Header("Trees Properties")]
        public int treeChance = 10;
        public int minimumTreeHeight = 2;
        public int maximumTreeHeight = 6;

        [Header("Addons Properties")]
        public int tallGrassChance = 10;

        [Header("Ore Properties")]
        public Ore[] ores;

    }
}