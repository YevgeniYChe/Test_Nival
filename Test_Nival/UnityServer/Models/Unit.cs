using System;

namespace UnityServer.Models
{
    [Serializable]
    public class Unit
    {

        public int X;
        public int Y;

        public bool IsMoving;
        public bool IsSelect;

        public int TargetX;
        public int TargetY;

        public Unit(int _x, int _y)
        {
            X = _x;
            Y = _y;
        }
    }
}