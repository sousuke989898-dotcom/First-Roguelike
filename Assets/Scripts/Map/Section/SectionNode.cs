using System.Collections.Generic;
using UnityEngine;

public enum SplitType {Vertical, Horizontal}

namespace Game.GridMap
{
    public class SectionNode
    {

        public RectInt area;
        public SplitType splitType;


        public SectionNode left;
        public SectionNode right;

        public RoomData roomData;
        public RectInt roomRect;

        public Dictionary<Direction, Vector2Int> doorPositions = new();


        public bool IsLeaf => left == null && right == null;

        public SectionNode(RectInt area)
        {
            this.area = area;
        }

    }
}