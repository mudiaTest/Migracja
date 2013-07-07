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
        internal int? direction = null;
        internal bool endingPoint = false;
        
        internal bool Eq(VectorRectangleEdgePoint aPoint)
        {
            return vectorRectangle == aPoint.vectorRectangle && direction == aPoint.direction;
        }
        
        public VectorRectangleEdgePoint(Vector_Rectangle aVectorRectangle, int? aDirection)
        {
            direction = aDirection;
            vectorRectangle = aVectorRectangle;
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

    internal class EdgeSliceList : Dictionary<int, EdgeSlice>
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

        public EdgeSliceList(VectoredRectangleGroup aParent)
        {
            fmaxKey = -1;
            parent = aParent;
        }

        public void ClearReset()
        {
            Clear();
            fmaxKey = -1;
        }

        internal void DecreaseMaxKey()
        {
            fmaxKey--;
        }

        internal VectorRectangleEdgePointList vectorRectangleEdgePointFullList = null;
        internal VectorRectangleEdgePointList simplifiedVectorRectangleEdgePointFullList = null;

        private int? edgeVectRectanglesCount = null;
        private int? simplifiedEdgeVectRectanglesCount = null;

        internal int CountEdgeVectRectangles()
        {
            if (edgeVectRectanglesCount == null)
            {
                edgeVectRectanglesCount = 0;
                foreach (KeyValuePair<int, EdgeSlice> pair in this)
                {
                    edgeVectRectanglesCount += pair.Value.vectorRectangleList(parent).Count;
                }
            }
            return (int) edgeVectRectanglesCount;
        }

        internal int CountSimplifiedEdgeVectRectangles()
        {
            if (simplifiedEdgeVectRectanglesCount == null)
            {
                simplifiedEdgeVectRectanglesCount = 0;
                foreach (KeyValuePair<int, EdgeSlice> pair in this)
                {
                    simplifiedEdgeVectRectanglesCount += pair.Value.simplifiedVectorRectangleList(parent).Count;
                }
            }
            return (int) simplifiedEdgeVectRectanglesCount;
        }

        internal VectoredRectangleGroup parent = null;
    }

    internal class EdgeSlice
    {
        private VectoredRectangleGroup parentVectoredRectangleGroupUp = null;
        private VectoredRectangleGroup parentVectoredRectangleGroupDown = null;

        // !! fVectorRectangleListUp i fSimplifiedVectorRectangleListUp oraz
        //    fVectorRectangleListUp i fSimplifiedVectorRectangleListUp
        //    współdzielą te same obiekty VectRect !!
        private VectorRectangleEdgePointList fVectorRectangleListUp = new VectorRectangleEdgePointList();
        private VectorRectangleEdgePointList fVectorRectangleListDown = null;

        private VectorRectangleEdgePointList vectorRectangleListDown
        {
            get
            {
                if (fVectorRectangleListDown == null)
                {
                    fVectorRectangleListDown = new VectorRectangleEdgePointList();
                    for (int i = fVectorRectangleListUp.Count - 1; i >= 0; i--)
                    {
                        fVectorRectangleListDown.Add(fVectorRectangleListDown.NextKey(), fVectorRectangleListUp[i]);
                    }
                }
                return fVectorRectangleListDown;
            }
        }

        internal VectorRectangleEdgePointList vectorRectangleList(VectoredRectangleGroup aGroup)
        {
            if (aGroup == parentVectoredRectangleGroupUp)
                return fVectorRectangleListUp;
            else if (aGroup == parentVectoredRectangleGroupDown)
                return vectorRectangleListDown;
            else if (aGroup == null)
            {
                Debug.Assert(false, "aGroup jest Null");
                return null;
            }
            else
            {
                Debug.Assert(false, "Niezidentyfikowane aGroup: " + aGroup.lpGroup.ToString());
                return null;
            }
        }

        private VectorRectangleEdgePointList fSimplifiedVectorRectangleListUp = new VectorRectangleEdgePointList();
        private VectorRectangleEdgePointList fSimplifiedVectorRectangleListDown = null;

        private VectorRectangleEdgePointList simplifiedVectorRectangleListDown
        {
            get
            {
                if (fSimplifiedVectorRectangleListDown == null)
                {
                    fSimplifiedVectorRectangleListDown = new VectorRectangleEdgePointList();
                    for (int i = fSimplifiedVectorRectangleListUp.Count - 1; i >= 0; i--)
                    {
                        fSimplifiedVectorRectangleListDown.Add(fSimplifiedVectorRectangleListDown.NextKey(),
                                                               fSimplifiedVectorRectangleListUp[i]);
                    }
                }
                return fSimplifiedVectorRectangleListDown;
            }
        }

        internal VectorRectangleEdgePointList simplifiedVectorRectangleList(VectoredRectangleGroup aGroup)
        {
            if (aGroup == parentVectoredRectangleGroupUp)
                return fSimplifiedVectorRectangleListUp;
            else if (aGroup == parentVectoredRectangleGroupDown)
                return simplifiedVectorRectangleListDown;
            else if (aGroup == null)
            {
                Debug.Assert(false, "aGroup jest Null");
                return null;
            }
            else
            {
                Debug.Assert(false, "Niezidentyfikowane aGroup: " + aGroup.lpGroup.ToString());
                return null;
            }
        }

        /*public List<int> GetSortedKeyList()
        {
            List<int> result = Keys.ToList();
            result.Sort();
            return result;
        }*/

        public EdgeSlice(VectoredRectangleGroup aGroup)
        {
            parentVectoredRectangleGroupUp = aGroup;
        }

        public void ClearReset()
        {
            if (fVectorRectangleListUp != null)
                fVectorRectangleListUp.ClearReset();
            if (fVectorRectangleListDown != null)
                fVectorRectangleListDown.ClearReset();
            if (fSimplifiedVectorRectangleListUp != null)
                fSimplifiedVectorRectangleListUp.ClearReset();
            if (fSimplifiedVectorRectangleListDown != null)
                fSimplifiedVectorRectangleListDown.ClearReset();
        }

        /*internal void FillSimplifiedVectorRectangleList()
        {
            Debug.Assert(fSimplifiedVectorRectangleListDown == null,
                         "EdgeSliceList (Simplified) jest ponownie wypełniane");
            for (int i = fVectorRectangleListUp.Count - 1; i <= 0; i--)
                fSimplifiedVectorRectangleListUp.Add(fSimplifiedVectorRectangleListUp.NextKey(), 
                                                     fVectorRectangleListUp[i].Clone());
        }*/

        internal void FillSimplifyVectorRectangleList()
        {
            /*//to do
            //simplifiedEdgeList = new EdgeSliceList(this);
            List<int> sortedKeyList = fVectorRectangleListUp.GetSortedKeyList(); 
            Vector_Rectangle startRect = fVectorRectangleListUp[sortedKeyList[0]];
            // punkt, wobec którego sprawdzamy położenie kolejnych
            Vector_Rectangle middleRect = null;
            Vector_Rectangle endRect = null;
            int lastKey;
            //usunięcie punktów z linii poziomych i pionowych
            if (sortedKeyList.Count >= 3)
            {
                lastKey = fSimplifiedVectorRectangleListUp.NextKey();
                fSimplifiedVectorRectangleListUp.Add(lastKey, startRect);
                for (var i = 1; i < sortedKeyList.Count - 1; i++)
                {
                    middleRect = fVectorRectangleListUp[sortedKeyList[i]];
                    endRect = fVectorRectangleListUp[sortedKeyList[i + 1]];
                    if (!InLineHorizontal(startRect, middleRect, endRect) &&
                        !InLineVertical(startRect, middleRect, endRect))
                    {
                        lastKey = fSimplifiedVectorRectangleListUp.NextKey();
                        fSimplifiedVectorRectangleListUp.Add(lastKey, middleRect);
                        startRect = middleRect;
                        //jeśli krawędź zakręciła w kierunku poziomym, to prevDiff też w tym kierunku obliczamy
                    }
                }


                startRect = fSimplifiedVectorRectangleListUp[lastKey];
                middleRect = fVectorRectangleListUp[sortedKeyList[sortedKeyList.Count - 1]];
                endRect = fVectorRectangleListUp[sortedKeyList[0]];

                if (!InLineHorizontal(startRect, middleRect, endRect) &&
                    !InLineVertical(startRect, middleRect, endRect))
                    fSimplifiedVectorRectangleListUp.Add(fSimplifiedVectorRectangleListUp.NextKey(),
                                                         fVectorRectangleListUp[sortedKeyList[sortedKeyList.Count - 1]]);
            }
            else
            {
                for (var i = 0; i < sortedKeyList.Count; i++)
                {
                    fSimplifiedVectorRectangleListUp.Add(fSimplifiedVectorRectangleListUp.NextKey(),
                                                         fVectorRectangleListUp[sortedKeyList[i]]);
                }
            }*/
        }

        //obiekty "podążają" w jednym kierunku w linii poziomej
        internal bool InLineHorizontal(Vector_Rectangle aStartRect,
                                       Vector_Rectangle aMiddleRect,
                                       Vector_Rectangle aEndRect)
        {
            bool result = (aStartRect.p1.Y - aMiddleRect.p1.Y > 0 && aMiddleRect.p1.Y - aEndRect.p1.Y > 0) ||
                          (aStartRect.p1.Y - aMiddleRect.p1.Y < 0 && aMiddleRect.p1.Y - aEndRect.p1.Y < 0);
            return result && aStartRect.p1.X == aMiddleRect.p1.X && aStartRect.p1.X == aEndRect.p1.X;
        }

        //obiekty "podążają" w jednym kierunku w linii pionowej
        internal bool InLineVertical(Vector_Rectangle aStartRect,
                                     Vector_Rectangle aMiddleRect,
                                     Vector_Rectangle aEndRect)
        {
            bool result = (aStartRect.p1.X - aMiddleRect.p1.X > 0 && aMiddleRect.p1.X - aEndRect.p1.X > 0) ||
                          (aStartRect.p1.X - aMiddleRect.p1.X < 0 && aMiddleRect.p1.X - aEndRect.p1.X < 0);
            return result && aStartRect.p1.Y == aMiddleRect.p1.Y && aStartRect.p1.Y == aEndRect.p1.Y;
        }
    }

    partial class VectoredRectangleGroup : Dictionary<int, Vector_Rectangle>
    {
        private int NextDirection(int aDirection)
        {
            long dummy;
            aDirection++;
            return (int) Math.DivRem((long) aDirection, (long) 4, out dummy);
        }

        private bool CheckBottomPX(Vector_Rectangle aStartEdgePoint)
        {
            bool result = false;
            if (aStartEdgePoint.p1.Y < SrcHeight() - 1)
            {
                Vector_Gen bottomVectorRectangle = GetVectorArr()[aStartEdgePoint.p1.X][aStartEdgePoint.p1.Y + 1];
                result = (bottomVectorRectangle != null) &&
                            (bottomVectorRectangle.parentVectorGroupId == aStartEdgePoint.parentVectorGroupId);
            }
            return result;
        }

        private Vector_Rectangle CheckTop(Vector_Rectangle aPrevEdge3, bool aBlInnerBorder, int aOuterGroup)
        {
            if (aPrevEdge3.p1.Y > 0)
            {
                Vector_Rectangle Result3 = GetVectorArr()[aPrevEdge3.p1.X][aPrevEdge3.p1.Y - 1];
                if ((!aBlInnerBorder && (aPrevEdge3.parentVectorGroupId == Result3.parentVectorGroupId)) ||
                    (aBlInnerBorder && (Result3.parentVectorGroupId != aOuterGroup)))
                    return Result3;
            }
            return null;
        }

        private Vector_Rectangle CheckRight(Vector_Rectangle aPrevEdge3, bool aBlInnerBorder, int aOuterGroup)
        {
            if (aPrevEdge3.p1.X < SrcWidth() - 1)
            {
                Vector_Rectangle Result3 = GetVectorArr()[aPrevEdge3.p1.X + 1][aPrevEdge3.p1.Y];
                if ((!aBlInnerBorder && (aPrevEdge3.parentVectorGroupId == Result3.parentVectorGroupId)) ||
                    (aBlInnerBorder && (Result3.parentVectorGroupId != aOuterGroup)))
                    return Result3;
            }
            return null;
        }

        private Vector_Rectangle CheckBottom(Vector_Rectangle aPrevEdge3, bool aBlInnerBorder, int aOuterGroup)
        {
            if (aPrevEdge3.p1.Y < SrcHeight() - 1)
            {
                Vector_Rectangle Result3 = GetVectorArr()[aPrevEdge3.p1.X][aPrevEdge3.p1.Y + 1];
                if ((!aBlInnerBorder && (aPrevEdge3.parentVectorGroupId == Result3.parentVectorGroupId)) ||
                    (aBlInnerBorder && (Result3.parentVectorGroupId != aOuterGroup)))
                    return Result3;
            }
            return null;
        }

        private Vector_Rectangle CheckLeft(Vector_Rectangle aPrevEdge3, bool aBlInnerBorder, int aOuterGroup)
        {
            if (aPrevEdge3.p1.X > 0)
            {
                Vector_Rectangle Result3 = GetVectorArr()[aPrevEdge3.p1.X - 1][aPrevEdge3.p1.Y];
                if ((!aBlInnerBorder && (aPrevEdge3.parentVectorGroupId == Result3.parentVectorGroupId)) ||
                    (aBlInnerBorder && (Result3.parentVectorGroupId != aOuterGroup)))
                    return Result3;
            }
            return null;
        }

        private VectorRectangleEdgePoint GetNextEdge(VectorRectangleEdgePoint aPrevEdgePoint, ref int aPrvDir, bool aBlInnerBorder,
                                                int aOuterGroup)
        {
            VectorRectangleEdgePoint result = null;

            //szukamy kolejnego punktu do granicy
            Vector_Rectangle firstNextPoint = null;
            firstNextPoint = FindNextPoint(aPrevEdgePoint.vectorRectangle, aPrvDir, aBlInnerBorder, aOuterGroup);
            Debug.Assert(firstNextPoint != null, "Nie znaleziono kolejnego punktu dla " + 
                                                 aPrevEdgePoint.vectorRectangle.DbgStr() + " " +
                                                 aPrevEdgePoint.direction.ToString());

            /*if ((nextEdgePoint != null) &&
                //przeszliśmy z prawa na lewo
                (aPrvEdgeDir == Cst.fromRightToLeft))
                GetColorArr()[nextEdgePoint.p1.X][nextEdgePoint.p1.Y].borderEW = true;*/

            //jeśli kierunek poprzedniej krawędzi wskaże na firstNextPoint, tzn że
            //kolejnym elementem będzie nextEdgePoint
            int dir = GetDir(aPrevEdgePoint.vectorRectangle, firstNextPoint);
            //z natury budowy granicy dla aPrevEdgePoint.direction zawsze wpadniemy w (firstNextPoint, dir) 
            if (dir == aPrevEdgePoint.direction || (aPrevEdgePoint.direction == 0)) 
            {
                Vector_Rectangle secondNextPoint = FindNextPoint(firstNextPoint, dir, aBlInnerBorder,
                                                                 aOuterGroup);
                int nextDir = GetDir(firstNextPoint, secondNextPoint);
                if (Dir.NextCheck(dir) == nextDir)
                    result = new VectorRectangleEdgePoint(firstNextPoint, Cst.noFromEdge);
                else /*if (dir == nextDir)*/
                    result = new VectorRectangleEdgePoint(firstNextPoint, dir);
                aPrvDir = dir;    
            }
            else
            {
                result = new VectorRectangleEdgePoint(aPrevEdgePoint.vectorRectangle, Dir.Next(aPrevEdgePoint.direction));
            }
            return result;
        }

            private Vector_Rectangle FindNextPoint(Vector_Rectangle aPrevEdgePoint, int aPrvEdgeDir, bool aBlInnerBorder,
                                                   int aOuterGroup)
            {
                Vector_Rectangle Result = null;
                /*if (aPrvEdgeDir == Cst.noFromEdge)
                {
                    Result = CheckTop(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                    if (Result == null)
                    {
                        Result = CheckRight(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                        if (Result == null)
                        {
                            Result = CheckBottom(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                            if (Result == null)
                            {
                                Result = CheckLeft(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                            }
                        }
                    }
                }*/
                //else
                if (aPrvEdgeDir == Cst.fromLeftToRight)
                {
                    Result = CheckTop(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                    if (Result == null)
                    {
                        Result = CheckRight(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                        if (Result == null)
                        {
                            Result = CheckBottom(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                            if (Result == null)
                            {
                                Result = CheckLeft(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                                if (Result == null)
                                {
                                    Result = CheckLeft(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                                }
                            }
                        }
                    }
                }
                else if (aPrvEdgeDir == Cst.fromTopToBottom)
                {
                    Result = CheckRight(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                    if (Result == null)
                    {
                        Result = CheckBottom(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                        if (Result == null)
                        {
                            Result = CheckLeft(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                            if (Result == null)
                            {
                                Result = CheckTop(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                            }
                        }
                    }
                }
                else if (aPrvEdgeDir == Cst.fromRightToLeft)
                {
                    Result = CheckBottom(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                    if (Result == null)
                    {
                        Result = CheckLeft(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                        if (Result == null)
                        {
                            Result = CheckTop(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                            if (Result == null)
                            {
                                Result = CheckRight(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                            }
                        }
                    }
                }
                else if (aPrvEdgeDir == Cst.fromBottomToTop)
                {
                    Result = CheckLeft(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                    if (Result == null)
                    {
                        Result = CheckTop(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                        if (Result == null)
                        {
                            Result = CheckRight(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                            if (Result == null)
                            {
                                Result = CheckBottom(aPrevEdgePoint, aBlInnerBorder, aOuterGroup);
                            }
                        }
                    }
                }
                return Result;
            }

            private int GetDir(Vector_Rectangle aFirstEdgePoint, Vector_Rectangle aSecondEdgePoint)
            {
                int result = Cst.fromLeftToRight;
                if (aFirstEdgePoint.p1.X < aSecondEdgePoint.p1.X)
                    result = Cst.fromLeftToRight;
                else if (aFirstEdgePoint.p1.X > aSecondEdgePoint.p1.X)
                    result = Cst.fromRightToLeft;
                else if (aFirstEdgePoint.p1.Y > aSecondEdgePoint.p1.Y)
                    result = Cst.fromBottomToTop;
                else if (aFirstEdgePoint.p1.Y < aSecondEdgePoint.p1.Y)
                    result = Cst.fromTopToBottom;
                Debug.Assert(result >= Cst.fromLeftToRight && result <= Cst.fromBottomToTop, "Brak krawędzi pomiędzy " + aFirstEdgePoint.DbgStr() + " i " + aSecondEdgePoint.DbgStr());
                return result;
            }

            private VectorRectangleEdgePoint FindEgdeDir(Vector_Rectangle aEdgePoint, Vector_Rectangle aPrvEdgePoint,
                                             int aPrvEdgeDir, bool aBlInnerBorder, int aOuterGroup)
            {


                //jeśli kierunek poprzedniej krawędzi NIE wskaże na kolejny punkt (aEdgePoint), tzn że
                //kolejnym elementem będzie poprzedni punkt z kolejną krawędzią
                VectorRectangleEdgePoint result = null;
                int prvDir = GetDir(aPrvEdgePoint, aEdgePoint);
                if (prvDir != aPrvEdgeDir)
                {
                    result = new VectorRectangleEdgePoint(aPrvEdgePoint, Dir.Next(aPrvEdgeDir));
                }
                else
                {
                    Vector_Rectangle nextPoint = FindNextPoint(aEdgePoint, prvDir, aBlInnerBorder,
                                                               aOuterGroup);
                    int nextDir = GetDir(aEdgePoint, nextPoint);
                    if (Dir.Next(prvDir) == nextDir)
                        result = new VectorRectangleEdgePoint(aEdgePoint, Cst.noFromEdge);
                    else
                        result = new VectorRectangleEdgePoint(aEdgePoint, aPrvEdgeDir);
                }
                return result;
            }

        private void MakeUsed(EdgeSliceList aEdgeSliceList, bool aBlInnerBorder)
        {
            VectorRectangleEdgePointList list;
            if (aBlInnerBorder)
                for (int i = 0; i < aEdgeSliceList.Count; i++)
                {
                    list = aEdgeSliceList[i].vectorRectangleList(aEdgeSliceList.parent);
                    for (int j = 0; j < list.Count; j++)
                    {
                        Vector_Rectangle point = list[j].vectorRectangle;
                        if (point.p1.Y >= 1)
                            GetColorArr()[point.p1.X][point.p1.Y - 1].used = true;
                    }
                }
            ;
        }


        internal bool OtherNeightbour(Vector_Rectangle aNextEdgePoint, int aArrivDir)
        {
            if (aArrivDir == Cst.fromRightToLeft)
                return GetVectorArr()[aNextEdgePoint.p1.X + 1][aNextEdgePoint.p1.Y].parentVectorGroupId !=
                        GetVectorArr()[aNextEdgePoint.p1.X][aNextEdgePoint.p1.Y].parentVectorGroupId;
            else if (aArrivDir == Cst.fromLeftToRight)
                return GetVectorArr()[aNextEdgePoint.p1.X - 1][aNextEdgePoint.p1.Y].parentVectorGroupId !=
                        GetVectorArr()[aNextEdgePoint.p1.X][aNextEdgePoint.p1.Y].parentVectorGroupId;
            else if (aArrivDir == Cst.fromTopToBottom)
                return GetVectorArr()[aNextEdgePoint.p1.X][aNextEdgePoint.p1.Y - 1].parentVectorGroupId !=
                        GetVectorArr()[aNextEdgePoint.p1.X][aNextEdgePoint.p1.Y].parentVectorGroupId;
            else if (aArrivDir == Cst.fromBottomToTop)
                return GetVectorArr()[aNextEdgePoint.p1.X][aNextEdgePoint.p1.Y + 1].parentVectorGroupId !=
                        GetVectorArr()[aNextEdgePoint.p1.X][aNextEdgePoint.p1.Y].parentVectorGroupId;
            else
            {
                Debug.Assert(false, "Nieznany kierunek aArrivDir: " + aArrivDir.ToString());
                return false;
            }
        }

        internal bool OtherNeightbour(VectorRectangleEdgePoint aPrevEdgePoint, VectorRectangleEdgePoint aNextEdgePoint)
        {
            bool result;
            Point prvP = aPrevEdgePoint.vectorRectangle.p1;
            Point nextP = aNextEdgePoint.vectorRectangle.p1;
            if (aNextEdgePoint.direction == Cst.noFromEdge || aPrevEdgePoint.direction == Cst.noFromEdge)
                return false;
            switch (aPrevEdgePoint.direction)
            {
                case Cst.fromLeftToRight:
                    result = IsTheSameNeightbour(new Point(prvP.X, prvP.Y-1), new Point(nextP.X, nextP.Y-1));
                    break;
                case Cst.fromTopToBottom:
                    result = IsTheSameNeightbour(new Point(prvP.X+1, prvP.Y), new Point(nextP.X+1, nextP.Y));
                    break;
                case Cst.fromRightToLeft:
                    result = IsTheSameNeightbour(new Point(prvP.X, prvP.Y+1), new Point(nextP.X, nextP.Y+1));
                    break;
                case Cst.fromBottomToTop:
                    result = IsTheSameNeightbour(new Point(prvP.X-1, prvP.Y), new Point(nextP.X-1, nextP.Y));
                    break;
                default:
                    Debug.Assert(false, "Nieznany kierunek aArrivDir: " + aPrevEdgePoint.direction.ToString());
                    return false;
            }
            ;
            if (Dir.OuterCornerCheck(aPrevEdgePoint.direction) == aNextEdgePoint.direction)
            {
                switch (aPrevEdgePoint.direction)
                {
                    case Cst.fromLeftToRight:
                        result &= IsTheSameNeightbour(new Point(prvP.X+1, prvP.Y-1), new Point(nextP.X+1, nextP.Y));
                        break;
                    case Cst.fromTopToBottom:
                        result &= IsTheSameNeightbour(new Point(prvP.X+1, prvP.Y+1), new Point(nextP.X, nextP.Y+1));
                        break;
                    case Cst.fromRightToLeft:
                        result &= IsTheSameNeightbour(new Point(prvP.X-1, prvP.Y+1), new Point(nextP.X-1, nextP.Y));
                        break;
                    case Cst.fromBottomToTop:
                        result &= IsTheSameNeightbour(new Point(prvP.X-1, prvP.Y-1), new Point(nextP.X, nextP.Y-1));
                        break;
                    default:
                        Debug.Assert(false, "Nieznany kierunek aArrivDir: " + aPrevEdgePoint.direction.ToString());
                        return false;
                }
            }
            return result;
        }
            internal bool IsTheSameNeightbour(Point aP1, Point aP2)
            {
                int? idP1;
                if (aP1.X < 0 || aP1.X >= GetVectorArr().Length || aP1.Y < 0 || aP1.Y >= GetVectorArr()[0].Length)
                    idP1 = null;
                else
                    idP1 = GetVectorArr()[aP1.X][aP1.Y].parentVectorGroupId;
                int? idP2;
                if (aP2.X < 0 || aP2.X >= GetVectorArr().Length || aP2.Y < 0 || aP2.Y >= GetVectorArr()[0].Length)
                    idP2 = null;
                else
                    idP2 = GetVectorArr()[aP2.X][aP2.Y].parentVectorGroupId;
                return idP1 != idP2;
            }

        //buduje krawędź
        //VectorRectangeGroup to mapa (kluczem jest int - kolejne wartości wyznaczają kolejność) obiektów Vector_Rectangle 
        public void MakeEdge(EdgeSliceList aEdgeSliceList, bool aBlInnerBorder = false, int aOuterGroup = 0)
        {
            aEdgeSliceList.ClearReset();
            //startEdgePoint to pierwszy punkt na liście, bo idziemy od lewej strony w najwyższym wierszu
            VectorRectangleEdgePoint startEdgePoint = new VectorRectangleEdgePoint(this[0], Cst.fromLeftToRight);
            VectorRectangleEdgePoint prevEdgePoint = startEdgePoint;
            int arrivDir = Cst.fromLeftToRight; //zaczynamy od max lewego ponktu na górnej linji
            //Każemy zacząć szukanie od prawej
            VectorRectangleEdgePoint nextEdgePoint = null;

            EdgeSlice actSlice = new EdgeSlice(aEdgeSliceList.parent);
            EdgeSlice firstSlice = actSlice;
            aEdgeSliceList.Add(aEdgeSliceList.NextKey(), actSlice);
            //1-pixelowy obiekt traktujemy inaczej
            if (Count != 1)
            {
                //kończymy jeśli trafiamy na początek, lub na 1-pixelowy obiekt
                Debug.Assert(
                    aEdgeSliceList.Count == 1 &&
                    aEdgeSliceList[0].vectorRectangleList(aEdgeSliceList.parent).Count == 0,
                    "aEdgeSliceList.Count: " + aEdgeSliceList.Count.ToString() + "vectorRectangleList: " +
                    aEdgeSliceList[0].vectorRectangleList(aEdgeSliceList.parent).Count.ToString());
                actSlice.vectorRectangleList(aEdgeSliceList.parent)
                        .Add(actSlice.vectorRectangleList(aEdgeSliceList.parent).NextKey(), startEdgePoint);
                do
                {
                    /*if (nextEdgePoint == startEdgePoint)
                    {
                        if (CheckBottomPX(startEdgePoint.vectorRectangle) && (arrivDir == Cst.fromRight))
                        {
                            arrivDir = Cst.goBottom;
                        }
                        else
                        {
                            //arrivDir = -1;
                            break;
                        }
                        ;
                    }*/
                    ;
                    nextEdgePoint = GetNextEdge(prevEdgePoint, ref arrivDir, aBlInnerBorder, aOuterGroup);
                    //powstanie gdy nie możemy oddać następnej krawędzi, ale wyjątkikem jest gdy jest to pojedynczy pixel
                    if ((nextEdgePoint == null) && (aEdgeSliceList.Count != 0))
                        Debug.Assert(false, "Oddany edge jest nil (" + prevEdgePoint.vectorRectangle.p1.X.ToString() +
                                            "," + prevEdgePoint.vectorRectangle.p1.Y.ToString() +
                                            "), liczba znalezionych kreawędzi:" +
                                            aEdgeSliceList.Count.ToString());
                    if (OtherNeightbour(prevEdgePoint, nextEdgePoint))
                    {
                        prevEdgePoint.endingPoint = true;
                        if (firstSlice != actSlice)
                            actSlice.FillSimplifyVectorRectangleList();
                        actSlice = new EdgeSlice(aEdgeSliceList.parent);
                        aEdgeSliceList.Add(aEdgeSliceList.NextKey(), actSlice);
                    }
                    if (!nextEdgePoint.Eq(startEdgePoint))
                    {
                        actSlice.vectorRectangleList(aEdgeSliceList.parent)
                                .Add(actSlice.vectorRectangleList(aEdgeSliceList.parent).NextKey(), nextEdgePoint);
                        //MakeUsed(nextEdgePoint aBlInnerBorder);
                        prevEdgePoint = nextEdgePoint;
                    }
                }
                while (!nextEdgePoint.Eq(startEdgePoint));
                prevEdgePoint.endingPoint = true;
                firstSlice.FillSimplifyVectorRectangleList();
            }
                //dla obiektu 1-pixelowego
            else
            {
                //dodajemy punkty graniczne do listy
                actSlice.vectorRectangleList(aEdgeSliceList.parent)[0] = startEdgePoint;
                actSlice.vectorRectangleList(aEdgeSliceList.parent)[1] = new VectorRectangleEdgePoint(startEdgePoint.vectorRectangle, Cst.fromTopToBottom);
                actSlice.vectorRectangleList(aEdgeSliceList.parent)[2] = new VectorRectangleEdgePoint(startEdgePoint.vectorRectangle, Cst.fromRightToLeft);
                actSlice.vectorRectangleList(aEdgeSliceList.parent)[3] = new VectorRectangleEdgePoint(startEdgePoint.vectorRectangle, Cst.fromBottomToTop);
                actSlice.vectorRectangleList(aEdgeSliceList.parent)[3].endingPoint = true;
                actSlice.FillSimplifyVectorRectangleList();
                // MakeUsed(startEdgePoint aBlInnerBorder);
            }
            //ostatni obiekt prevEdgePoint jest na powno końcowym
            prevEdgePoint.endingPoint = true;
            ;

            //dla uproszczenia niepocięta granica dla grupy rectancli = true
            VectorRectangleEdgePointList list;
            aEdgeSliceList.vectorRectangleEdgePointFullList = new VectorRectangleEdgePointList();
            for (int i = 0; i < aEdgeSliceList.Count; i++)
            {
                list = aEdgeSliceList[i].vectorRectangleList(aEdgeSliceList.parent);
                for (int j = 0; j < list.Count; j++)
                {
                    aEdgeSliceList.vectorRectangleEdgePointFullList.Add(aEdgeSliceList.vectorRectangleEdgePointFullList.NextKey(),
                                                                list[j]);
                }
            }

            //tu powinno wyć wywołane upraszczanie granic wypełniające simplifiedVectorRectangleList dal każdefo fragmentu granicy

            //dla uproszczenia niepocięta uproszczona granica dla grupy rectancli
            aEdgeSliceList.simplifiedVectorRectangleEdgePointFullList = new VectorRectangleEdgePointList();
            for (int i = 0; i < aEdgeSliceList.Count; i++)
            {
                list = aEdgeSliceList[i].simplifiedVectorRectangleList(aEdgeSliceList.parent);
                for (int j = 0; j < list.Count; j++)
                {
                    aEdgeSliceList.simplifiedVectorRectangleEdgePointFullList.Add(
                        aEdgeSliceList.simplifiedVectorRectangleEdgePointFullList.NextKey(), list[j]);
                }
            }

            List<GeoPoint> geoPointList = MakeVectorEdge(aEdgeSliceList.vectorRectangleEdgePointFullList, GetColorArr(), true);
            MakeUsed(aEdgeSliceList, aBlInnerBorder);
            geoPointList.Clear();
            //PxListToGeoList;
        }

        //VectorRectangeGroup to mapa (kluczem jest int - kolejne wartości wyznaczają kolejność) obiektów Vector_Rectangle 
        private void PxListToGeoList(EdgeSliceList aEdgeSliceList)
        {
            Vector_Rectangle edgePoint;
            GeoPoint geoPoint;
            VectorRectangleEdgePointList list;

            for (int i = 0; i < aEdgeSliceList.Count; i++)
            {
                list = aEdgeSliceList[i].vectorRectangleList(aEdgeSliceList.parent);
                for (int j = 0; j < list.Count; j++)
                {
                    edgePoint = list[j].vectorRectangle;
                    geoPoint = PxPointToGeoPoint(edgePoint);
                    edgeGeoList.Add(i, geoPoint); //przepisujemy klucz z edgePxList
                }
            }
        }

    }
}
