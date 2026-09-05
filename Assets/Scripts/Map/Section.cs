using System.Collections.Generic;
using UnityEngine;

namespace Game.GridMap
{
    public class Section
    {
        public RectInt SectionRect {get; private set;}

        public int X => SectionRect.xMin;
        public int Y => SectionRect.yMin;

        public Vector2Int Position => SectionRect.position;

        public int Width => SectionRect.width;
        public int Height => SectionRect.height;

        public int Size => Width * Height;
        public Vector2Int SectionCenter => new(X + (Width/2), Y + (Height/2));

        public RectInt RoomRect {get; private set;}


        public Section(RectInt sectionRect)
        {
            SectionRect = sectionRect;
        }

        public Section(int x, int y, int w, int h)
        {
            SectionRect = new RectInt(x,y,w,h);
        }

        bool IsContained(RectInt rectA, RectInt rectB) //本当はここじゃない方がいい？
        {
            return  rectB.xMin >= rectA.xMin &&
                    rectB.xMax <= rectA.xMax &&
                    rectB.yMin >= rectA.yMin &&
                    rectB.yMax <= rectA.yMax;
        }
    }

}
