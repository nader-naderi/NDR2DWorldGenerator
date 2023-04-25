using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NDR2DWorldGenerator
{
    [CreateAssetMenu(fileName = "TileAtlas", menuName = "Tile Atlas")]
    public class TileAtlas : ScriptableObject
    {
        [Header("Environment")]
        public Tile grass;
        public Tile dirt;
        public Tile stone;
        public Tile tree_Start;
        public Tile tree_Start_Left;
        public Tile tree_Start_Right;
        public Tile tree_middle;
        public Tile leaf;
        public Tile tree_Branch;
        public Tile tree_Branch_End;
        public Tile tree_Branch_End_Lower;
        public Tile tree_Branch_End_Upper;
        public Tile tallGrass;
        public Tile snow;
        public Tile sand;

        [Header("ore")]
        public Tile coal;
        public Tile iron;
        public Tile gold;
        public Tile diamond;
    }
}