using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Migracja
{
    //DynamicGeoPointArray = List<GeoPoint>;
    //TDynamicPointArray = Vector_Gen[];
    //TDynamicPxColorPointArray = ColorPx[,];

    class EdgeList : Dictionary<int, EdgeSliceList>
    {
        private int fMaxKey;

        internal int maxKey {
            get { return fMaxKey; }
        }
  
        internal int NextKey()
        {
            return ++fMaxKey;
        }

        public EdgeList()
        {
            fMaxKey = -1;
        }
    }

    //Grupa rectangli tworzacych jedną płąszczyznę. Pozwala obliczyć swoją granicę
    partial class VectoredRectangleGroup : Dictionary<int, Vector_Rectangle>
    {
        //cztery punkty geograficzne określanące rogi obrazka
        //leftTopGeo, rightTopGeo, leftBottomGeo, rightBottomGeo: Double;
        //lista obiektów Vector_Rectangle tworzących krawędź (self, czyli grupy obiektów Vector_Rectangle). 
        //Kolejnośc wyznaczają klucze
        public EdgeSliceList edgeSliceList { get; set; }
        //lista wewnętrznych krawędzi. Każda z nich ma konstrukcję jak edgePxList
        public EdgeList innerEdgesList { get; set; }
        //lista krawędzi punktów Double
        public Dictionary<int, GeoPoint> edgeGeoList { get; set; }
        //lista krawędzi punktów-pixeli (kolejnych), które zostały poddane uproszczaniu - jest to okrojona edgePxList
        public EdgeSliceList simplifiedEdgeList { get; set; }
        //lista uroszczonych wewnętrznych krawędzi. Każda z nich ma konstrukcję jak edgePxList
        public Dictionary<int, EdgeSliceList> simplifiedInnerEdgesList { get; set; }

        //niepotrzebe bo obiekt jedt grupą samą w sobie
        //lista 'kwadratów' należących do grupy
        //frectList: TIntList;
        //kolor testowy - tym kolorem wypełniana jest grupa gdy włączymy opcję testu
        public Color testColor{get;set;}
        //kolor oryginalny
        public Color sourceColor{get;set;}
        //lp utworzonej grupy. Później utworzona ma wyższy numer
        public int lpGroup{get;set;}
        //rodzic
        public MapFactory parentMapFactory{get;set;}

        internal Point[] pointArrFromFullEdge = null;
        internal Point[] pointArrFromSimplifiedEdge = null;

        Point[] GetEdgeListAsArray(float aScale) 
        {
            Debug.Assert(aScale >= 1, "GetEdgeListAsArray nie może utworzyć polygonu dla skali <= 1.");

            Point[] result = new Point[edgeSliceList.vectorRectangleFullList.Count];
            int i = 0;
            foreach (int key in edgeSliceList.vectorRectangleFullList.GetSortedKeyList())
            {
                result[i] = edgeSliceList.vectorRectangleFullList[key].p1;
                i++;
            }
            return result;
            return null;
        }

        //in: dwa KOLEJNE punkty poruszające się po liniach Hor i Ver
        //out: Cst.fromLeft, Cst.fromTop, Cst.fromRight, Cst.fromBottom
        private int Direction(Point p1, Point p2)
        {
            if (p1.X > p2.X)
                return Cst.fromRight;
            else if (p1.X < p2.X)
                return Cst.fromLeft;
            else if (p1.Y > p2.Y)
                return Cst.fromBottom;
            else
                return Cst.fromTop;
        }

        //tworzy krawędź z 3 kolejnych punktów
        //  multi = mnożnik: dla grafiki bedzie to zoom, dla geo będzie to szerokośc
        //          geograficzna jednego px
        private void MakePartEdge(  Point aPrvPoint, Point aActPoint, Point aNextPoint,
                                    ref int lpCounter, List<GeoPoint> aGeoArr,
                                    float aMultiX, float aMultiY, float aDisplaceX, float aDisplaceY,
                                    ColorPx[][] aColorArr,
                                    bool aBlOnlyFillColorArr)
        {
            ColorPx colorPx = aColorArr[aActPoint.X][aActPoint.Y] as ColorPx;
            
//-------------------
            GeoPoint nextGeoPoint;
            GeoPoint lastGeoPoint = null;
            if( aGeoArr.Count >= 1) 
                lastGeoPoint = aGeoArr.Last();
            if (Direction(aPrvPoint, aActPoint) == Cst.fromLeft) 
            {
                if (Direction(aActPoint, aNextPoint) == Cst.fromBottom) 
                {
                    nextGeoPoint = new GeoPoint(aActPoint.X * aMultiX + aDisplaceX, aActPoint.Y * aMultiY + aDisplaceY);//!!!
                    if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                        aGeoArr.Add(nextGeoPoint);
                }
                else if (Direction(aActPoint, aNextPoint) == Cst.fromTop) 
                {
                    if (!aBlOnlyFillColorArr) 
                    {
                        nextGeoPoint = new GeoPoint(aActPoint.X * aMultiX + aDisplaceX, aActPoint.Y * aMultiY + aDisplaceY);
                        if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                            aGeoArr.Add(nextGeoPoint);
                        aGeoArr.Add(new GeoPoint((aActPoint.X+1)*aMultiX + aDisplaceX, aActPoint.Y*aMultiY + aDisplaceY));
                        lpCounter = lpCounter + 2;
                    };
                    colorPx.borderWE = true;
                    colorPx.borderNS = true;
                }
                else if (Direction(aActPoint, aNextPoint) == Cst.fromLeft) 
                {
                    if (!aBlOnlyFillColorArr) 
                    {
                        nextGeoPoint = new GeoPoint((aActPoint.X) * aMultiX + aDisplaceX, (aActPoint.Y) * aMultiY + aDisplaceY);
                        if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                            aGeoArr.Add(nextGeoPoint);
                        lpCounter = lpCounter + 1;
                    };
                    colorPx.borderWE = true;
                }
                else
                {
                    if (!aBlOnlyFillColorArr) 
                    {
                        nextGeoPoint = new GeoPoint((aActPoint.X)*aMultiX + aDisplaceX, (aActPoint.Y)*aMultiY + aDisplaceY);
                        if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                            aGeoArr.Add(nextGeoPoint);
                        aGeoArr.Add(new GeoPoint((aActPoint.X+1)*aMultiX + aDisplaceX, aActPoint.Y*aMultiY + aDisplaceY));
                        aGeoArr.Add(new GeoPoint((aActPoint.X+1)*aMultiX + aDisplaceX, (aActPoint.Y+1)*aMultiY + aDisplaceY));
                        lpCounter = lpCounter + 3;
                    };
                    colorPx.borderWE = true;
                    colorPx.borderNS = true;
                    colorPx.borderEW = true;
                };
            }

            else if (Direction(aPrvPoint, aActPoint) == Cst.fromRight) 
            {
                if (Direction(aActPoint, aNextPoint) == Cst.fromBottom) 
                {
                    if (!aBlOnlyFillColorArr) 
                    {
                        nextGeoPoint = new GeoPoint((aActPoint.X + 1) * aMultiX + aDisplaceX, (aActPoint.Y + 1) * aMultiY + aDisplaceY);
                        if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                            aGeoArr.Add(nextGeoPoint);
                        aGeoArr.Add(new GeoPoint(aActPoint.X*aMultiX + aDisplaceX, (aActPoint.Y+1)*aMultiY + aDisplaceY));
                        lpCounter = lpCounter + 2;
                    };
                    colorPx.borderEW = true;
                    colorPx.borderSN = true;
                }
                else if (Direction(aActPoint, aNextPoint) == Cst.fromTop) 
                {
                    nextGeoPoint = new GeoPoint((aActPoint.X + 1) * aMultiX + aDisplaceX, (aActPoint.Y + 1) * aMultiY + aDisplaceY);//!!!
                    if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                        aGeoArr.Add(nextGeoPoint);
                }
                else if (Direction(aActPoint, aNextPoint) == Cst.fromRight) 
                {
                    if (!aBlOnlyFillColorArr) 
                    {
                        nextGeoPoint = new GeoPoint((aActPoint.X + 1) * aMultiX + aDisplaceX, (aActPoint.Y + 1) * aMultiY + aDisplaceY);
                        if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                            aGeoArr.Add(nextGeoPoint); 
                        lpCounter = lpCounter + 1;
                    };
                    colorPx.borderEW = true;
                }
                else
                {
                    if (!aBlOnlyFillColorArr) 
                    {
                        nextGeoPoint = new GeoPoint((aActPoint.X + 1) * aMultiX + aDisplaceX, (aActPoint.Y + 1) * aMultiY + aDisplaceY);
                        if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                            aGeoArr.Add(nextGeoPoint);
                        aGeoArr.Add(new GeoPoint(aActPoint.X*aMultiX + aDisplaceX, (aActPoint.Y+1)*aMultiY + aDisplaceY));
                        aGeoArr.Add(new GeoPoint(aActPoint.X*aMultiX + aDisplaceX, (aActPoint.Y)*aMultiY + aDisplaceY));
                        lpCounter = lpCounter + 3;
                    };
                    colorPx.borderEW = true;
                    colorPx.borderNS = true;
                    colorPx.borderWE = true;
                };
            }

            else if (Direction(aPrvPoint, aActPoint) == Cst.fromTop) 
            {
                if (Direction(aActPoint, aNextPoint) == Cst.fromLeft) 
                {
                    nextGeoPoint = new GeoPoint((aActPoint.X + 1) * aMultiX + aDisplaceX, (aActPoint.Y) * aMultiY + aDisplaceY); //!!!
                    if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                        aGeoArr.Add(nextGeoPoint);
                }
                else if (Direction(aActPoint, aNextPoint) == Cst.fromRight) 
                {
                    if (!aBlOnlyFillColorArr) 
                    {
                        nextGeoPoint = new GeoPoint((aActPoint.X + 1) * aMultiX + aDisplaceX, aActPoint.Y * aMultiY + aDisplaceY);
                        if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                            aGeoArr.Add(nextGeoPoint);
                        aGeoArr.Add(new GeoPoint((aActPoint.X+1)*aMultiX + aDisplaceX, (aActPoint.Y+1)*aMultiY + aDisplaceY));
                        lpCounter = lpCounter + 2;
                    };
                    colorPx.borderNS = true;
                    colorPx.borderEW = true;
                }
                else if (Direction(aActPoint, aNextPoint) == Cst.fromTop) 
                {
                    if (!aBlOnlyFillColorArr) 
                    {
                        nextGeoPoint = new GeoPoint((aActPoint.X + 1) * aMultiX + aDisplaceX, aActPoint.Y * aMultiY + aDisplaceY);
                        if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                            aGeoArr.Add(nextGeoPoint); 
                        lpCounter = lpCounter + 1;
                    };
                    colorPx.borderNS = true;
                }
                else
                {
                    if (!aBlOnlyFillColorArr) 
                    {
                        nextGeoPoint = new GeoPoint((aActPoint.X + 1) * aMultiX + aDisplaceX, aActPoint.Y * aMultiY + aDisplaceY);
                        if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                            aGeoArr.Add(nextGeoPoint);
                        aGeoArr.Add(new GeoPoint((aActPoint.X+1)*aMultiX + aDisplaceX, (aActPoint.Y+1)*aMultiY + aDisplaceY));
                        aGeoArr.Add(new GeoPoint(aActPoint.X*aMultiX + aDisplaceX, (aActPoint.Y+1)*aMultiY + aDisplaceY));
                        lpCounter = lpCounter + 3;
                    };
                    colorPx.borderNS = true;
                    colorPx.borderEW = true;
                    colorPx.borderSN = true;
                };
            }

            else //from bottom
            {
                if (Direction(aActPoint, aNextPoint) == Cst.fromLeft) 
                {
                    if (!aBlOnlyFillColorArr) 
                    {
                        nextGeoPoint = new GeoPoint(aActPoint.X * aMultiX + aDisplaceX, (aActPoint.Y + 1) * aMultiY + aDisplaceY);
                        if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                            aGeoArr.Add(nextGeoPoint);
                        aGeoArr.Add(new GeoPoint(aActPoint.X*aMultiX + aDisplaceX, aActPoint.Y*aMultiY + aDisplaceY));
                        lpCounter = lpCounter + 2;
                    };
                    colorPx.borderSN = true;
                    colorPx.borderWE = true;
                }
                else if (Direction(aActPoint, aNextPoint) == Cst.fromRight) 
                {
                    nextGeoPoint = new GeoPoint(aActPoint.X * aMultiX + aDisplaceX, (aActPoint.Y + 1) * aMultiY + aDisplaceY);//!!!
                    if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                        aGeoArr.Add(nextGeoPoint);
                }
                else if (Direction(aActPoint, aNextPoint) == Cst.fromBottom) 
                {
                    if (!aBlOnlyFillColorArr) 
                    {
                        nextGeoPoint = new GeoPoint(aActPoint.X * aMultiX + aDisplaceX, (aActPoint.Y + 1) * aMultiY + aDisplaceY);
                        if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                            aGeoArr.Add(nextGeoPoint);
                        lpCounter = lpCounter + 1;
                    };
                    colorPx.borderSN = true;
                }
                else
                {
                    if (!aBlOnlyFillColorArr) 
                    {
                        nextGeoPoint = new GeoPoint(aActPoint.X * aMultiX + aDisplaceX, (aActPoint.Y + 1) * aMultiY + aDisplaceY);
                        if (lastGeoPoint == null || lastGeoPoint.GetX() != nextGeoPoint.GetX() || lastGeoPoint.GetY() != nextGeoPoint.GetY())
                            aGeoArr.Add(nextGeoPoint);
                        aGeoArr.Add(new GeoPoint(aActPoint.X*aMultiX + aDisplaceX, aActPoint.Y*aMultiY + aDisplaceY));
                        aGeoArr.Add(new GeoPoint((aActPoint.X+1)*aMultiX + aDisplaceX, aActPoint.Y*aMultiY + aDisplaceY));
                        lpCounter = lpCounter + 3;
                    };
                    colorPx.borderSN = true;
                    colorPx.borderWE = true;
                    colorPx.borderNS = true;
            };
        }

//-------------------
        }

        //tworzy krawędzie dla pojedynczego punktu
        private void MakePartEdgeForOnePoint( Point aPoint, List<GeoPoint> aGeoArr,
                                            float aDpMultiX, float aDPMultiY, float aDpDisplaceX, float aDpDisplaceY,
                                            ColorPx[][] aColorArr,
                                            bool aBlOnlyFillColorArr)
        {
            if (!aBlOnlyFillColorArr)
            {
                aGeoArr.Add(new GeoPoint((aPoint.X)*aDpMultiX + aDpDisplaceX, (aPoint.Y)*aDPMultiY + aDpDisplaceY));
                aGeoArr.Add(new GeoPoint((aPoint.X+1)*aDpMultiX + aDpDisplaceX, aPoint.Y*aDPMultiY + aDpDisplaceY));
                aGeoArr.Add(new GeoPoint((aPoint.X+1)*aDpMultiX + aDpDisplaceX, (aPoint.Y+1)*aDPMultiY + aDpDisplaceY));
                aGeoArr.Add(new GeoPoint((aPoint.X)*aDpMultiX + aDpDisplaceX, (aPoint.Y+1)*aDPMultiY + aDpDisplaceY));
            }
            ColorPx colorPx = aColorArr[aPoint.X][aPoint.Y];
            colorPx.borderNS = true;
            colorPx.borderSN = true;
            colorPx.borderEW = true;
            colorPx.borderWE = true;            
        }

        private void GetLine(Point p1, Point p2, ref float A, ref float C, ref float mian)
        {
            var GetLineA = new Func<float>
            (
                () => (p2.Y - p1.Y) / (p2.X - p1.X)
            );
            var GetLineC = new Func<float, float>
            (
                (innerA) => p1.Y - innerA*p1.X
            );
            var GetMianownik = new Func<float, float>
            (
                (innerA) => (float)Math.Sqrt(Math.Pow((double)innerA,(double)2) + 1)
            );
            
            A = GetLineA();
            C = GetLineC(A);
            mian = GetMianownik(A);            
        }

        private int SrcHeight()
        {
            return parentMapFactory.srcHeight;
        }

        private int  SrcWidth()
        {
            return parentMapFactory.srcWidth;
        }

        private Vector_Rectangle[][] GetVectorArr()
        {
            return parentMapFactory.vectArr;
        }

        private ColorPx[][] GetColorArr()
        {
            return parentMapFactory.colorArr;
        }

        private GeoPoint PxPointToGeoPoint(Vector_Rectangle aPxPoint)
        {
            //P1 i P2 będą takie same, bo aPxPoint reprezentuje pojedynczy pixel, więc wartośći x i y możemy wziąć z p1
            return new GeoPoint(aPxPoint.p1.X, aPxPoint.p1.Y);
        }

        public VectoredRectangleGroup()
        {
            edgeSliceList = new EdgeSliceList(this);
            innerEdgesList = new EdgeList();
        }

        //tworzy tablicę punktów z punktów zawartych w edgeSliceList
        public List<GeoPoint> MakeVectorEdge(VectorRectangleList aEdgeVectorRectangleList,
                                             ColorPx[][] aColorArr,
                                             bool aBlOnlyFillColorArr,
                                             float aMultiX = 1, float aMultiY = 1,
                                             float aDisplaceX = 0, float aDisplaceY = 0)
        {
            List<GeoPoint> result = new List<GeoPoint>(aEdgeVectorRectangleList.Count * 3);

            int counter = 0;
            //if (!aBlOnlyFillColorArr)
            //    SetLength(result, aEdgePxList.Count*3);

            if (aEdgeVectorRectangleList.Count > 1)
            {
                VectorRectangleList list;
                //foreach(KeyValuePair<int, EdgeSlice> pair in aEdgeSliceList)
                //{
                    //list = pair.Value.vectorRectangleList(aEdgeSliceList.parent);
                //if (aEdgeVectorRectangleList.Count >= 3)
                    //{
                        //SetLength(result, self.rectList.Count+30);
                        Point o1 = aEdgeVectorRectangleList[aEdgeVectorRectangleList.Count - 1].GetP(0);
                        Point o2 = aEdgeVectorRectangleList[0].GetP(0);
                        Point o3 = aEdgeVectorRectangleList[1].GetP(0);
                        MakePartEdge(o1, o2, o3, ref counter, result, aMultiX, aMultiY, aDisplaceX, aDisplaceY,
                                     aColorArr, aBlOnlyFillColorArr);

                        for (int i = 1; i < aEdgeVectorRectangleList.Count - 1; i++)
                        {
                            o1 = aEdgeVectorRectangleList[i - 1].GetP(0);
                            o2 = aEdgeVectorRectangleList[i].GetP(0);
                            o3 = aEdgeVectorRectangleList[i + 1].GetP(0);
                            MakePartEdge(o1, o2, o3, ref counter, result, aMultiX, aMultiY, aDisplaceX, aDisplaceY,
                                         aColorArr, aBlOnlyFillColorArr);
                        }
                        ;

                        o1 = aEdgeVectorRectangleList[aEdgeVectorRectangleList.Count - 2].GetP(0);
                        o2 = aEdgeVectorRectangleList[aEdgeVectorRectangleList.Count - 1].GetP(0);
                        o3 = aEdgeVectorRectangleList[0].GetP(0);
                        MakePartEdge(o1, o2, o3, ref counter, result, aMultiX, aMultiY, aDisplaceX, aDisplaceY,
                                     aColorArr, aBlOnlyFillColorArr);
                        //SetLength(result, Min(counter,10));
                        //if (!aBlOnlyFillColorArr) then
                        //   SetLength(result, counter);
                    //}
                //}
            } 
            else
            {
                //if (!aBlOnlyFillColorArr) 
                //    SetLength(result, 4);
                MakePartEdgeForOnePoint(aEdgeVectorRectangleList[0].GetP(0),
                                        result, aMultiX, aMultiY, aDisplaceX, aDisplaceY,
                                        aColorArr, aBlOnlyFillColorArr);
                counter = 4;
            }
            return result;
        }

        internal void MakePointArrFromFullEdge(float aDpScale, float aDisplaceX, float aDisplaceY)
        {
            pointArrFromFullEdge = MakePointArrFromEdge(edgeSliceList.vectorRectangleFullList, aDpScale, aDisplaceX, aDisplaceY);
        }

        internal void MakePointArrFromSimplifiedEdge(float aDpScale, float aDisplaceX, float aDisplaceY)
        {
            pointArrFromSimplifiedEdge = MakePointArrFromEdge(edgeSliceList.simplifiedVectorRectangleFullList, aDpScale, aDisplaceX, aDisplaceY);
        }

        private Point[] MakePointArrFromEdge(VectorRectangleList aEdgeList, float aDpScale, float aDisplaceX, float aDisplaceY)
            {
                //Point[] result = new Point[aEdgePxList.Count * 3];
                List<GeoPoint> pxPointList = MakeVectorEdge(aEdgeList, GetColorArr(), false, aDpScale, aDpScale, aDisplaceX, aDisplaceY);
                return PointList2PxArray(pxPointList);
            }


        internal Point[] GetScaledPointArrFromFullEdge(float adpScale, float aDisplaceX, float aDisplaceY)
        {
            return GetScaledPointArrFromEdge(adpScale, aDisplaceX, aDisplaceY, pointArrFromFullEdge);
        }

        internal Point[] GetScaledPointArrFromSimplifiedEdge(float adpScale, float aDisplaceX, float aDisplaceY)
        {
            return GetScaledPointArrFromEdge(adpScale, aDisplaceX, aDisplaceY, pointArrFromSimplifiedEdge);
        }

            private Point[] GetScaledPointArrFromEdge(float adpScale, float aDisplaceX, float aDisplaceY, Point[] aPointArr)
            {
                Point[] result = new Point[aPointArr.Length];
                for(int i=0; i< aPointArr.Length; i++)
                {
                    result[i] = new Point((int)Math.Round(aPointArr[i].X * adpScale / Cst.maxZoom + aDisplaceX), 
                                          (int)Math.Round(aPointArr[i].Y * adpScale / Cst.maxZoom + aDisplaceY));
                }
                return result;
            }

                public Point[] PointList2PxArray(List<GeoPoint> aGeoList)
        {
            Point[] result = new Point[aGeoList.Count];
            for (int i = 0; i < aGeoList.Count; i++)
               result[i] = aGeoList[i].ToPoint();
            return result;
        }

                    
    }
}
