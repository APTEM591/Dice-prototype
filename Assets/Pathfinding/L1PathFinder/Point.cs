using System;
using TriInspector;

namespace L1PathFinder
{
    [Serializable]
    public class Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        [ShowInInspector] public int X { get; set; }
        [ShowInInspector] public int Y { get; set; }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}