using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Migracja
{

    internal class VectorRectangleEdgePoint
    {
        internal Vector_Rectangle vectorRectangle = null;
        internal int? directionNull = null;
        internal VectoredRectangleGroup GroupsEndingPoint = null;
        internal EdgeSlice edgeSlice = null;

        internal bool Eq(VectorRectangleEdgePoint aPoint)
        {
            return vectorRectangle == aPoint.vectorRectangle && directionNull == aPoint.directionNull;
        }
        
        public VectorRectangleEdgePoint(Vector_Rectangle aVectorRectangle, int? aDirection)
        {
            directionNull = aDirection;
            vectorRectangle = aVectorRectangle;
        }

        internal int Direction()
        {
            Debug.Assert(directionNull != null, "Direction nie może być null");
            return (int) directionNull;
        }
    }

    internal class VectorRectangleEdgePointList : Dictionary<int, VectorRectangleEdgePoint>
    {
        private int fmaxKey;

        internal int maxKey
        {
            get { return fmaxKey; }
        }

        internal int NextKey()
        {
            return ++fmaxKey;
        }

        public VectorRectangleEdgePointList()
        {
            fmaxKey = -1;
        }

        public void ClearReset()
        {
            Clear();
            fmaxKey = -1;
        }

        /*public static List<int> GetSortedKeyList(this Dictionary<int, Vector_Rectangle> dict)
        {
            List<int> result = dict.Keys.ToList();
            result.Sort();
            return result;
        }*/
    }   
}
