using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameData
{
    public enum Direction
    {
        UP = 1,
        DOWN = 2,
        LEFT = 3,
        RIGHT = 4
    }

    public enum TempoType
    {
        Quarter,
        Half,
        Whole
    }

    [System.Serializable]
    public class RouteData
    {
        public Direction direction;
        public float distance = 0f;
    }

    [System.Serializable]
    public class BulletData
    {
        public float speed = 0f;
        public Direction direction;
        public float distance = 0f;
        public GameData.TempoType tempoType;
    };

    class Map
    {
        static public Dictionary<int, Vector2> directionMap = new Dictionary<int, Vector2> {
        {(int)Direction.UP, Vector2.up },
        { (int)Direction.DOWN, Vector2.down },
        { (int)Direction.LEFT, Vector2.left },
        { (int)Direction.RIGHT, Vector2.right }
        };

    }
}