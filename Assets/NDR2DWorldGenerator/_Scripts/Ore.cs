using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NDR2DWorldGenerator
{
    [System.Serializable]
    public class Ore
    {
        public string name;
        public float rarity;
        public float size;
        public int maxSpawnHeight;
        public Texture2D spreadTexture;
    }
}