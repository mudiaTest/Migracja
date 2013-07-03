using System;
using System.Collections.Generic;
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

        public static List<int> GetSortedKeyList(this Dictionary<int, Vector_Rectangle> dict)
        {
            List<int> result = dict.Keys.ToList();
            result.Sort();
            return result;
        }
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
            //to do
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
            }
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

        private VectorRectangleEdgePoint GetNextEdge(Vector_Rectangle aPrevEdge, ref int aArrDir, bool aBlInnerBorder,
                                                int aOuterGroup)
        {
            Vector_Rectangle Result = null;
            int j = 0;
            //for (int i = 0; i < 4; i++)
            {
                if (aArrDir + j == 4)
                    j = -aArrDir;
                Result = CheckNextEdge(aPrevEdge, aArrDir + j, aBlInnerBorder, aOuterGroup);
                if (Result != null)
                {
                    aArrDir = aArrDir + 1;
                    Result = aPrevEdge;
                    /* if (aArrDir + j == Cst.fromLeft)
                        aArrDir = Cst.fromBottom;
                    else if (aArrDir + j == Cst.fromBottom)
                        aArrDir = Cst.fromRight;
                    else if (aArrDir + j == Cst.fromTop)
                        aArrDir = Cst.fromLeft;
                    else if (aArrDir + j == Cst.fromRight)
                        aArrDir = Cst.fromTop;
                    break;*/

                }
                ;
                //j++;
            }
            ;
            if ((Result != null) &&
                //przeszliśmy z prawa na lewo
                (aArrDir == Cst.fromRight))
                GetColorArr()[Result.p1.X][Result.p1.Y].borderEW = true;
            //return Result;
            return new VectorRectangleEdgePoint(Result, aArrDir);
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
                        Vector_Rectangle point = list[j];
                        if (point.p1.Y >= 1)
                            GetColorArr()[point.p1.X][point.p1.Y - 1].used = true;
                    }
                }
            ;
        }

        private Vector_Rectangle CheckNextEdge(Vector_Rectangle aPrevEdge2, int aArrDir, bool aBlInnerBorder,
                                                int aOuterGroup)
        {
            Vector_Rectangle Result;
            if (aArrDir == Cst.goTop)
                Result = CheckTop(aPrevEdge2, aBlInnerBorder, aOuterGroup);
            else if (aArrDir == Cst.goRight)
                Result = CheckRight(aPrevEdge2, aBlInnerBorder, aOuterGroup);
            else if (aArrDir == Cst.goBottom)
                Result = CheckBottom(aPrevEdge2, aBlInnerBorder, aOuterGroup);
            else if (aArrDir == Cst.goLeft)
                Result = CheckLeft(aPrevEdge2, aBlInnerBorder, aOuterGroup);
            else
            {
                Result = null;
                //Assert(False, 'checkNextEdge');
            }
            ;
            return Result;
        }

        internal bool MultiEdge(Vector_Rectangle aNextEdgePoint, int aArrivDir)
        {
            if (aArrivDir == Cst.fromRight)
                return GetVectorArr()[aNextEdgePoint.p1.X + 1][aNextEdgePoint.p1.Y].parentVectorGroupId !=
                        GetVectorArr()[aNextEdgePoint.p1.X][aNextEdgePoint.p1.Y].parentVectorGroupId;
            else if (aArrivDir == Cst.fromLeft)
                return GetVectorArr()[aNextEdgePoint.p1.X - 1][aNextEdgePoint.p1.Y].parentVectorGroupId !=
                        GetVectorArr()[aNextEdgePoint.p1.X][aNextEdgePoint.p1.Y].parentVectorGroupId;
            else if (aArrivDir == Cst.fromTop)
                return GetVectorArr()[aNextEdgePoint.p1.X][aNextEdgePoint.p1.Y - 1].parentVectorGroupId !=
                        GetVectorArr()[aNextEdgePoint.p1.X][aNextEdgePoint.p1.Y].parentVectorGroupId;
            else if (aArrivDir == Cst.fromBottom)
                return GetVectorArr()[aNextEdgePoint.p1.X][aNextEdgePoint.p1.Y + 1].parentVectorGroupId !=
                        GetVectorArr()[aNextEdgePoint.p1.X][aNextEdgePoint.p1.Y].parentVectorGroupId;
            else
            {
                Debug.Assert(false, "Nieznany kierunek aArrivDir: " + aArrivDir.ToString());
                return false;
            }
        }

        //buduje krawędź
        //VectorRectangeGroup to mapa (kluczem jest int - kolejne wartości wyznaczają kolejność) obiektów Vector_Rectangle 
        public void MakeEdge(EdgeSliceList aEdgeSliceList, bool aBlInnerBorder = false, int aOuterGroup = 0)
        {
            //var
            //startEdgePoint, nextEdgePoint, prevEdgePoint: TVectRectangle;
            //arrivDir: integer;
            //dummyArr: TDynamicGeoPointArray;


            aEdgeSliceList.ClearReset();
            //startEdgePoint to pierwszy punkt na liście, bo idziemy od lewej strony
            //w najwyższym wierszu
            VectorRectangleEdgePoint startEdgePoint = new VectorRectangleEdgePoint(this[0], Cst.fromLeft);
            VectorRectangleEdgePoint prevEdgePoint = startEdgePoint;
            int arrivDir = Cst.goRight; //zaczynamy od max lewego ponktu na górnej linji
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
                        .Add(actSlice.vectorRectangleList(aEdgeSliceList.parent).NextKey(), null);
                // na razie ustawiamy null, ale potem przypiszemy tutaj pierwszy obiekt
                while (
                    ((nextEdgePoint != startEdgePoint) && (prevEdgePoint != null)) ||
                    (CheckBottomPX(startEdgePoint.vectorRectangle) && (arrivDir == Cst.fromRight))
                    //przypadek gdy wracamy się do punktu startu, ale mamy do prawdzenia to co jest pod nim
                    )
                {
                    if (nextEdgePoint == startEdgePoint)
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
                    }
                    ;
                    nextEdgePoint = GetNextEdge(prevEdgePoint.vectorRectangle, ref arrivDir, aBlInnerBorder, aOuterGroup);
                    //powstanie gdy nie możemy oddać następnej krawędzi, ale wyjątkikem jest gdy jest to pojedynczy pixel
                    if ((nextEdgePoint == null) && (aEdgeSliceList.Count != 0))
                        Debug.Assert(false, "Oddany edge jest nil (" + prevEdgePoint.p1.X.ToString() +
                                            "," + prevEdgePoint.p1.Y.ToString() +
                                            "), liczba znalezionych kreawędzi:" +
                                            aEdgeSliceList.Count.ToString());

                    /*if (MultiEdge(nextEdgePoint, arrivDir))
                    {
                        if (firstSlice != actSlice)
                            actSlice.FillSimplifyVectorRectangleList();
                        actSlice = new EdgeSlice(aEdgeSliceList.parent);
                        aEdgeSliceList.Add(aEdgeSliceList.NextKey(), actSlice);
                    }*/

                    actSlice.vectorRectangleList(aEdgeSliceList.parent)
                            .Add(actSlice.vectorRectangleList(aEdgeSliceList.parent).NextKey(), nextEdgePoint);
                    //MakeUsed(nextEdgePoint aBlInnerBorder);
                    prevEdgePoint = nextEdgePoint;
                }
                firstSlice.vectorRectangleList(aEdgeSliceList.parent)[0] =
                    actSlice.vectorRectangleList(aEdgeSliceList.parent)[
                        actSlice.vectorRectangleList(aEdgeSliceList.parent).maxKey];
                firstSlice.FillSimplifyVectorRectangleList();
                if (firstSlice != actSlice)
                {
                    actSlice.vectorRectangleList(aEdgeSliceList.parent)
                            .Remove(actSlice.vectorRectangleList(aEdgeSliceList.parent).maxKey);
                    if (actSlice.vectorRectangleList(aEdgeSliceList.parent).Count > 0)
                    {
                        actSlice.FillSimplifyVectorRectangleList();
                    }
                    else
                    {
                        aEdgeSliceList.Remove(aEdgeSliceList.maxKey);
                        aEdgeSliceList.DecreaseMaxKey();
                    }
                }
            }
                //dla obiektu 1-pixelowego
            else
            {
                //dodajemy punkty graniczne do listy
                actSlice.vectorRectangleList(aEdgeSliceList.parent)[0] = startEdgePoint;
                actSlice.FillSimplifyVectorRectangleList();
                // MakeUsed(startEdgePoint aBlInnerBorder);
            }
            ;

            //dla uproszczenia niepocięta granica dla grupy rectancli
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

            //tu powinno wyć wywołane upraszczanie granic



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
                    edgePoint = list[j];
                    geoPoint = PxPointToGeoPoint(edgePoint);
                    edgeGeoList.Add(i, geoPoint); //przepisujemy klucz z edgePxList
                }
            }
        }

    }
}
