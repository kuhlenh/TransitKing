using System;

namespace BusInfo
{
    public class Direction : IEquatable<Direction>
    {
        public string Compass { get; set; }
        public Direction(string v) => Compass = v;
        //add equals override. misspell Length and correct it.show intellisense

        public override bool Equals(object obj) => Equals(obj as Direction);
        public bool Equals(Direction other)
        {
            if (other.Compass.Length == 1)
            {
                return Compass.Contains(other.Compass);
            }
            else if (other.Compass.Length == 2)
            {
                return Compass == other.Compass;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
