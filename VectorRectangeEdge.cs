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

        private EdgeSlice GetSliceEdgeFromEdgePoint(VectorRectangleEdgePoint aPoint)
        {
            EdgeSlice result = null;
            if (aPoint.Direction() == Cst.noFromEdge)
                return null;
            Vector_Rectangle point = GetVectorRectangleFromDirection(aPoint.vectorRectangle, aPoint.Direction());
            //null oznacza, że aPoint jest na granicy rastra i nie ma "nad" nim innego rectangla
            if (point != null)
            {
                int tmpDir = Dir.OppositeDirection(aPoint.Direction());
                if (point.parentVectorRectangleEdgePointList.ContainsKey(tmpDir))
                {
                    EdgeSlice tmpEdgeSlice = point.parentVectorRectangleEdgePointList[tmpDir].edgeSlice;
                    if (tmpEdgeSlice != null)
                    {
                        result = tmpEdgeSlice;
                    }
                }
            }
            return result;
        }
            //pobiera obiekt z punktu "nad" granicą -strzałką- (należący do innej grupy rectangli) 
            private Vector_Rectangle GetVectorRectangleFromDirection(Vector_Rectangle aPoint, int? aDir)
            {
                Vector_Rectangle result = null;
                Point tmpPoint;
                if (aDir == Cst.fromLeftToRight)
                {
                    tmpPoint = new Point(aPoint.p1.X, aPoint.p1.Y - 1);
                }
                else if (aDir == Cst.fromTopToBottom)
                {
                    tmpPoint = new Point(aPoint.p1.X + 1, aPoint.p1.Y);
                }
                else if (aDir == Cst.fromRightToLeft)
                {
                    tmpPoint = new Point(aPoint.p1.X, aPoint.p1.Y + 1);
                }
                else if (aDir == Cst.fromBottomToTop)
                {
                    tmpPoint = new Point(aPoint.p1.X - 1, aPoint.p1.Y);
                }
                else
                {
                    result = null;
                    tmpPoint = new Point(0, 0);
                    Debug.Assert(false, "Nieznany kierunek: " + aDir.ToString());
                }

                if (tmpPoint.X < 0 || tmpPoint.X >= GetVectorArr().Length || tmpPoint.Y < 0 || tmpPoint.Y >= GetVectorArr()[0].Length)
                    result = null;
                else
                    result = GetVectorArr()[tmpPoint.X][tmpPoint.Y];
                return result;
            }

        private VectorRectangleEdgePoint GetNextEdge(VectorRectangleEdgePoint aPrevEdgePoint, ref int aPrvDir, bool aBlInnerBorder,
                                                int aOuterGroup)
        {
            VectorRectangleEdgePoint result = null;

            //szukamy kolejnego punktu do granicy
            //Vector_Rectangle firstNextPoint = null;
            Vector_Rectangle firstNextPoint = FindNextPoint(aPrevEdgePoint.vectorRectangle, aPrvDir, aBlInnerBorder, aOuterGroup);
            Debug.Assert(firstNextPoint != null, "Nie znaleziono kolejnego punktu dla " + 
                                                 aPrevEdgePoint.vectorRectangle.DbgStr() + " " +
                                                 aPrevEdgePoint.directionNull.ToString());

            /*if ((nextEdgePoint != null) &&
                //przeszliśmy z prawa na lewo
                (aPrvEdgeDir == Cst.fromRightToLeft))
                GetColorArr()[nextEdgePoint.p1.X][nextEdgePoint.p1.Y].borderEW = true;*/

            //jeśli kierunek poprzedniej krawędzi wskaże na firstNextPoint, tzn że
            //kolejnym elementem będzie nextEdgePoint
            int dir = GetDir(aPrevEdgePoint.vectorRectangle, firstNextPoint, aPrevEdgePoint.Direction());
            //z natury budowy granicy dla aPrevEdgePoint.direction zawsze wpadniemy w (firstNextPoint, dir) 
            if (dir == aPrevEdgePoint.Direction() || (aPrevEdgePoint.Direction() == 0)) 
            {
                Vector_Rectangle secondNextPoint = FindNextPoint(firstNextPoint, dir, aBlInnerBorder,
                                                                 aOuterGroup);
                int nextDir = GetDir(firstNextPoint, secondNextPoint, null);
                if (Dir.NextCheck(dir) == nextDir)
                    result = new VectorRectangleEdgePoint(firstNextPoint, Cst.noFromEdge);
                else /*if (dir == nextDir)*/
                    result = new VectorRectangleEdgePoint(firstNextPoint, dir);
                aPrvDir = dir;    
            }
            else
            {
                result = new VectorRectangleEdgePoint(aPrevEdgePoint.vectorRectangle, Dir.Next(aPrevEdgePoint.Direction()));
            }            
            return result;
        }            

            private Vector_Rectangle FindNextPoint(Vector_Rectangle aPrevEdgePoint, int aPrvEdgeDir, bool aBlInnerBorder,
                                                   int aOuterGroup)
            {
                Vector_Rectangle Result = null;
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
                if (Result == null)
                {
                    Result = aPrevEdgePoint;
                }
                return Result;
            }

            private int GetDir(Vector_Rectangle aFirstEdgePoint, Vector_Rectangle aSecondEdgePoint, int? aPrvDir)
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
                else if (aFirstEdgePoint.p1 == aSecondEdgePoint.p1)
                {
                    Debug.Assert(aPrvDir != null, "aPrvDir nie może być == null");
                    result = Dir.Next(aPrvDir);
                }
                else
                    Debug.Assert(false, "Brak możliwości ustalenia następnego direction dla (" +
                                        aFirstEdgePoint.p1.X.ToString() + "," +
                                        aFirstEdgePoint.p1.Y.ToString() + "), (" +
                                        aSecondEdgePoint.p1.X.ToString() + "," +
                                        aSecondEdgePoint.p1.Y.ToString() + "), " +
                                        aPrvDir.ToString());
                Debug.Assert(result >= Cst.fromLeftToRight && result <= Cst.fromBottomToTop, "Brak krawędzi pomiędzy " + aFirstEdgePoint.DbgStr() + " i " + aSecondEdgePoint.DbgStr());
                return result;
            }

            private VectorRectangleEdgePoint FindEgdeDir(Vector_Rectangle aEdgePoint, Vector_Rectangle aPrvEdgePoint,
                                             int aPrvEdgeDir, bool aBlInnerBorder, int aOuterGroup)
            {


                //jeśli kierunek poprzedniej krawędzi NIE wskaże na kolejny punkt (aEdgePoint), tzn że
                //kolejnym elementem będzie poprzedni punkt z kolejną krawędzią
                VectorRectangleEdgePoint result = null;
                int prvDir = GetDir(aPrvEdgePoint, aEdgePoint, null);
                if (prvDir != aPrvEdgeDir)
                {
                    result = new VectorRectangleEdgePoint(aPrvEdgePoint, Dir.Next(aPrvEdgeDir));
                }
                else
                {
                    Vector_Rectangle nextPoint = FindNextPoint(aEdgePoint, prvDir, aBlInnerBorder,
                                                               aOuterGroup);
                    int nextDir = GetDir(aEdgePoint, nextPoint, null);
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
            Vector_Rectangle tmpVectorRectanglePrv;
            Vector_Rectangle tmpVectorRectangleNext;

            if (aPrevEdgePoint.directionNull == Cst.noFromEdge)
                return false;
            tmpVectorRectanglePrv = GetVectorRectangleFromDirection(aPrevEdgePoint.vectorRectangle, aPrevEdgePoint.Direction());

            //jeśli jesteśmw w trakcie kontaktu z sąsiadem, to może sią zdarzyć trafiamy na jego początkowy wierzołek
            if (aNextEdgePoint.Direction() == Cst.noFromEdge &&
                aPrevEdgePoint.Direction() == Cst.fromRightToLeft &&
                tmpVectorRectanglePrv.firstInGroup
                )
            {
                return true;
            }

            if (aNextEdgePoint.directionNull == Cst.noFromEdge)
                return false;           
            tmpVectorRectangleNext = GetVectorRectangleFromDirection(aNextEdgePoint.vectorRectangle, aNextEdgePoint.Direction());

            //podstawowe sprawdzenie, to porównanie grup, bedacych "na zewnątrz" punktów tworzących granice.
            if (tmpVectorRectanglePrv == null && tmpVectorRectangleNext == null)
                result = false;
            else
                result = !IsTheSameNeightbour(tmpVectorRectanglePrv.p1, tmpVectorRectangleNext.p1);

            if (Dir.OuterCornerCheck(aPrevEdgePoint.directionNull) == aNextEdgePoint.directionNull)
            {
                switch (aPrevEdgePoint.Direction())
                {
                    case Cst.fromLeftToRight:
                        result |= !IsTheSameNeightbour(new Point(prvP.X+1, prvP.Y-1), new Point(nextP.X+1, nextP.Y));
                        break;
                    case Cst.fromTopToBottom:
                        result |= !IsTheSameNeightbour(new Point(prvP.X+1, prvP.Y+1), new Point(nextP.X, nextP.Y+1));
                        break;
                    case Cst.fromRightToLeft:
                        result |= !IsTheSameNeightbour(new Point(prvP.X-1, prvP.Y+1), new Point(nextP.X-1, nextP.Y));
                        break;
                    case Cst.fromBottomToTop:
                        result |= !IsTheSameNeightbour(new Point(prvP.X-1, prvP.Y-1), new Point(nextP.X, nextP.Y-1));
                        break;
                    default:
                        Debug.Assert(false, "Nieznany kierunek aArrivDir: " + aPrevEdgePoint.directionNull.ToString());
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
                return idP1 == idP2;
            }

        //buduje krawędź
        //VectorRectangeGroup to mapa (kluczem jest int - kolejne wartości wyznaczają kolejność) obiektów Vector_Rectangle 
            public void MakeEdge(EdgeSliceList aEdgeSliceList, UpdateInfoBoxTimeDelegate aInfoBoxUpdateFunct, bool aBlInnerBorder = false, int aOuterGroup = 0)
        {
            aEdgeSliceList.ClearReset();
            //startEdgePoint to pierwszy punkt na liście, bo idziemy od lewej strony w najwyższym wierszu
            VectorRectangleEdgePoint startEdgePoint = new VectorRectangleEdgePoint(this[0], Cst.fromLeftToRight);
            this[0].parentVectorRectangleEdgePointList.Add(startEdgePoint.Direction(), startEdgePoint);
            VectorRectangleEdgePoint prevEdgePoint = startEdgePoint;
            int arrivDir = Cst.fromLeftToRight; //zaczynamy od max lewego ponktu na górnej linji
            //Każemy zacząć szukanie od prawej
            VectorRectangleEdgePoint nextEdgePoint = null;

            EdgeSlice actSlice = new EdgeSlice(aEdgeSliceList.parent);
            EdgeSlice firstSlice = actSlice;

            //kończymy jeśli trafiamy na początek, lub na 1-pixelowy obiekt
            /*Debug.Assert(
                aEdgeSliceList.Count == 1 &&
                aEdgeSliceList[0].vectorRectangleList(aEdgeSliceList.parent).Count == 0,
                "aEdgeSliceList.Count: " + aEdgeSliceList.Count.ToString() + "vectorRectangleList: " +
                aEdgeSliceList[0].vectorRectangleList(aEdgeSliceList.parent).Count.ToString());*/
 
            EdgeSlice exisingSlice = GetSliceEdgeFromEdgePoint(startEdgePoint);
            //jeśli nie znaleźliśmy odpowiedniego EdgeSlice dla startEdgePoint
            if (exisingSlice == null)
            {
                aEdgeSliceList.Add(aEdgeSliceList.NextKey(), actSlice);
                actSlice.vectorRectangleList(aEdgeSliceList.parent)
                        .Add(actSlice.vectorRectangleList(aEdgeSliceList.parent).NextKey(), startEdgePoint);
                startEdgePoint.edgeSlice = actSlice;
            }
            //jeśli jest odpowiednia granica
            else
            {
                aEdgeSliceList.Add(aEdgeSliceList.NextKey(), exisingSlice);
                exisingSlice.parentVectoredRectangleGroupSecond = this;
                //jeśli trafiliśmy na granicę, której możemy użyć, to skaczemy do ostatniego punktu
                prevEdgePoint = exisingSlice.GetLastSecondPoint();
                actSlice = null;
            }

            do
            {
                nextEdgePoint = GetNextEdge(prevEdgePoint, ref arrivDir, aBlInnerBorder, aOuterGroup);
                //if (nextEdgePoint.Direction() != Cst.noFromEdge){
                exisingSlice = GetSliceEdgeFromEdgePoint(nextEdgePoint);
                //jeśli nie znaleźliśmy odpowiedniego EdgeSlice dla nextEdgePoint
                if (exisingSlice == null)
                {
                    if (actSlice == null) 
                        actSlice = new EdgeSlice(aEdgeSliceList.parent);
                    //powstanie gdy nie możemy oddać następnej krawędzi, ale wyjątkikem jest gdy jest to pojedynczy pixel
                    if ((nextEdgePoint == null) && (aEdgeSliceList.Count != 0))
                        Debug.Assert(false,
                                        "Oddany edge jest nil (" + prevEdgePoint.vectorRectangle.p1.X.ToString() +
                                        "," + prevEdgePoint.vectorRectangle.p1.Y.ToString() +
                                        "), liczba znalezionych kreawędzi:" +
                                        aEdgeSliceList.Count.ToString());
                    //jeśli zaczymany robić nowy kawałek granicy, czyli :
                    // - albo rozpoczynamy kontakt z nowym sąsiadem
                    // - albo kontakt trwa, ale trafiliśmy na początkowy wierzchołek nowego sąsiada
                    if (OtherNeightbour(prevEdgePoint, nextEdgePoint))
                    {
                        //ustawiamy ostatni punkt jako końcowy
                        prevEdgePoint.GroupsEndingPoint = aEdgeSliceList.parent;
                        if (firstSlice != actSlice)
                            actSlice.FillSimplifyVectorRectangleList();
                        if (!nextEdgePoint.Eq(startEdgePoint))
                        {
                            actSlice = new EdgeSlice(aEdgeSliceList.parent);
                            aEdgeSliceList.Add(aEdgeSliceList.NextKey(), actSlice);
                        }
                    }
                    if (!nextEdgePoint.Eq(startEdgePoint))
                    {
                        actSlice.vectorRectangleList(aEdgeSliceList.parent)
                                .Add(actSlice.vectorRectangleList(aEdgeSliceList.parent).NextKey(), nextEdgePoint);
                         nextEdgePoint.vectorRectangle.parentVectorRectangleEdgePointList.Add(nextEdgePoint.Direction(), nextEdgePoint);
                        nextEdgePoint.edgeSlice = actSlice;
                        //MakeUsed(nextEdgePoint aBlInnerBorder);
                        prevEdgePoint = nextEdgePoint;
                    }
                }
                //jeśli jest odpowiednia granica
                else
                {
                    exisingSlice.parentVectoredRectangleGroupSecond = this;
                    //jeśli trafiliśmy na granicę, której możemy użyć, to skaczemy do ostatniego punktu
                    prevEdgePoint = exisingSlice.GetLastSecondPoint();
                    if (!prevEdgePoint.Eq(startEdgePoint))
                    {
                        aEdgeSliceList.Add(aEdgeSliceList.NextKey(), exisingSlice);
                        break;
                    }
                    actSlice = null;
                }
            }
            while (!nextEdgePoint.Eq(startEdgePoint));

            
            firstSlice.FillSimplifyVectorRectangleList();

            //ostatni obiekt prevEdgePoint jest na powno końcowym - zamykamy sprawę ostatniej granicy
            prevEdgePoint.GroupsEndingPoint = aEdgeSliceList.parent;
            



            //budowanie granicy zakończone - na jej podstawie wypełnimy inne obiekty

            //dla uproszczenia niepocięta granica dla grupy rectancli = true
            VectorRectangleEdgePointList list;
            aEdgeSliceList.vectorRectangleEdgePointFullList = new VectorRectangleEdgePointList();
                aInfoBoxUpdateFunct('\n'+"Grupa " + lpGroup.ToString(), true);
            for (int i = 0; i < aEdgeSliceList.Count; i++)
            {
                list = aEdgeSliceList[i].vectorRectangleList(aEdgeSliceList.parent);
                aInfoBoxUpdateFunct(i.ToString() + " edgeSlice: " + aEdgeSliceList[i].GetHashCode().ToString(), false);
                aInfoBoxUpdateFunct(i.ToString() + " VectorRectangleEdgePointList: " + list.GetHashCode().ToString(), false);
                for (int j = 0; j < list.Count; j++)
                {
                    aEdgeSliceList.vectorRectangleEdgePointFullList.Add(aEdgeSliceList.vectorRectangleEdgePointFullList.NextKey(),
                                                                list[j]);
                    aInfoBoxUpdateFunct("  " + list[j].GetHashCode().ToString() +
                                        " ("+list[j].vectorRectangle.p1.X.ToString() +
                                        ","+list[j].vectorRectangle.p1.Y.ToString() +
                                        "), "+list[j].directionNull.ToString()
                                        , false);
                }
            }




            //tu powinno wyć wywołane upraszczanie granic wypełniające simplifiedVectorRectangleList dla każdego fragmentu granicy

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
