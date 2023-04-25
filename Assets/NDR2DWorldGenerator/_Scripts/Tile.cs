using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NDR2DWorldGenerator
{
    [CreateAssetMenu(fileName = "NewTile", menuName = "Tile Class")]
    public class Tile : ScriptableObject
    {
        public string tileName;
        public Sprite[] tileSprites;

        public float rarity;
    }
}