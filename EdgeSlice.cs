using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;

namespace Migracja
{
    internal class EdgeSlice
    {
        internal VectoredRectangleGroup parentVectoredRectangleGroupFirst = null;
        internal VectoredRectangleGroup parentVectoredRectangleGroupSecond = null;

        // !! fVectorRectangleListUp i fSimplifiedVectorRectangleListUp oraz
        //    fVectorRectangleListUp i fSimplifiedVectorRectangleListUp
        //    współdzielą te same obiekty VectRect !!
        private VectorRectangleEdgePointList fVectorRectangleListFirst = new VectorRectangleEdgePointList();
        private VectorRectangleEdgePointList fVectorRectangleListSecond = null;

        private VectorRectangleEdgePointList vectorRectangleListSecond
        {
            get
            {
                VectorRectangleEdgePoint edgePoint = null;
                VectorRectangleEdgePoint prvEdgePoint = null;
                if (fVectorRectangleListSecond == null)
                {
                    fVectorRectangleListSecond = new VectorRectangleEdgePointList();
                    for (int i = fVectorRectangleListFirst.Count - 1; i >= 0; i--)
                    {
                        // edgePoint = GetOppositeDirectionEdgePoint(fVectorRectangleListFirst[i]);                      
                        //fVectorRectangleListSecond.Add(fVectorRectangleListSecond.NextKey(), edgePoint);                
                        edgePoint = AddOppositeDirectionEdgePoint(fVectorRectangleListFirst[i], prvEdgePoint, fVectorRectangleListSecond);
                        if (edgePoint != null)
                            prvEdgePoint = edgePoint;
                    }
                    //ostatni punkt opisyjemy jako końcowy
                    Debug.Assert(parentVectoredRectangleGroupSecond != null, "Podobiekt parentVectoredRectangleGroupSecond jest null.");
                    prvEdgePoint.GroupsEndingPoint = parentVectoredRectangleGroupSecond;
                }
                return fVectorRectangleListSecond;
            }
        }

        private VectorRectangleEdgePoint AddOppositeDirectionEdgePoint(VectorRectangleEdgePoint aPoint,
                                                   VectorRectangleEdgePoint aPrvPoint,
                                                   VectorRectangleEdgePointList aVectorRectangleListSecond)
        {
            VectorRectangleEdgePoint result = GetOppositeDirectionEdgePoint(aPoint);
            if (result != null)
            {
                //zakładamy, że aPrvPoint jest null gdy 
                // - jest to pierwszy punkt z listy
                // - poprzedni punkt aPoint miał Direction = Cst.noFromEdge (wewnętrzna cześć granicy)
                //W obu przypadkach nie ma potrzeby dla dodawania punktu z granicą Cst.noFromEdge
                if (aPrvPoint != null && aPrvPoint.Direction() == Dir.NextCheck(result.Direction()))
                    aVectorRectangleListSecond.Add(aVectorRectangleListSecond.NextKey(),
                                                   new VectorRectangleEdgePoint(result.vectorRectangle, Cst.noFromEdge)
                                                   );
                aVectorRectangleListSecond.Add(aVectorRectangleListSecond.NextKey(), result);
            }
            return result;
        }

        private VectorRectangleEdgePoint GetOppositeDirectionEdgePoint(VectorRectangleEdgePoint aPoint)
            {
                Point tmpPoint;
                int tmpDirection;
                Vector_Rectangle[][] tmpArr = parentVectoredRectangleGroupFirst.parentMapFactory.vectArr;
                if (aPoint.Direction() == Cst.fromLeftToRight)
                {
                    tmpPoint = new Point(aPoint.vectorRectangle.p1.X, aPoint.vectorRectangle.p1.Y - 1);
                    tmpDirection = Cst.fromRightToLeft;
                }
                else if (aPoint.Direction() == Cst.fromRightToLeft)
                {
                    tmpPoint = new Point(aPoint.vectorRectangle.p1.X, aPoint.vectorRectangle.p1.Y + 1);
                    tmpDirection = Cst.fromLeftToRight;
                }
                else if (aPoint.Direction() == Cst.fromTopToBottom)
                {
                    tmpPoint = new Point(aPoint.vectorRectangle.p1.X + 1, aPoint.vectorRectangle.p1.Y);
                    tmpDirection = Cst.fromBottomToTop;
                }
                else if (aPoint.Direction() == Cst.fromBottomToTop)
                {
                    tmpPoint = new Point(aPoint.vectorRectangle.p1.X - 1, aPoint.vectorRectangle.p1.Y);
                    tmpDirection = Cst.fromTopToBottom;
                }
                else if (aPoint.Direction() == Cst.noFromEdge)
                {
                    return null;
                }
                else
                {
                    Debug.Assert(false, "Nieznany kierunek: " + aPoint.directionNull.ToString());
                    //poniższe przypisania tylko dla uniknięcia warningów
                    tmpPoint = new Point(0, 0);
                    tmpDirection = 0;
                }
                if (tmpPoint.X < 0 || tmpPoint.X >= tmpArr.Length || tmpPoint.Y < 0 || tmpPoint.Y >= tmpArr[0].Length)
                    return null;

                VectorRectangleEdgePoint result = new VectorRectangleEdgePoint(tmpArr[tmpPoint.X][tmpPoint.Y],
                                                                               tmpDirection);
                return result;
            }

        internal VectorRectangleEdgePointList vectorRectangleList(VectoredRectangleGroup aGroup)
        {
            if (aGroup == parentVectoredRectangleGroupFirst)
                return fVectorRectangleListFirst;
            else if (aGroup == parentVectoredRectangleGroupSecond)
                return vectorRectangleListSecond;
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

        private VectorRectangleEdgePointList fSimplifiedVectorRectangleListFirst = new VectorRectangleEdgePointList();
        private VectorRectangleEdgePointList fSimplifiedVectorRectangleListSecond = null;

        private VectorRectangleEdgePointList simplifiedVectorRectangleListSecond
        {
            get
            {
                if (fSimplifiedVectorRectangleListSecond == null)
                {
                    fSimplifiedVectorRectangleListSecond = new VectorRectangleEdgePointList();
                    for (int i = fSimplifiedVectorRectangleListFirst.Count - 1; i >= 0; i--)
                    {
                        fSimplifiedVectorRectangleListSecond.Add(fSimplifiedVectorRectangleListSecond.NextKey(),
                                                               fSimplifiedVectorRectangleListFirst[i]);
                    }
                }
                return fSimplifiedVectorRectangleListSecond;
            }
        }

        internal VectorRectangleEdgePointList simplifiedVectorRectangleList(VectoredRectangleGroup aGroup)
        {
            if (aGroup == parentVectoredRectangleGroupFirst)
                return fSimplifiedVectorRectangleListFirst;
            else if (aGroup == parentVectoredRectangleGroupSecond)
                return simplifiedVectorRectangleListSecond;
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
            parentVectoredRectangleGroupFirst = aGroup;
        }

        public void ClearReset()
        {
            if (fVectorRectangleListFirst != null)
                fVectorRectangleListFirst.ClearReset();
            if (fVectorRectangleListSecond != null)
                fVectorRectangleListSecond.ClearReset();
            if (fSimplifiedVectorRectangleListFirst != null)
                fSimplifiedVectorRectangleListFirst.ClearReset();
            if (fSimplifiedVectorRectangleListSecond != null)
                fSimplifiedVectorRectangleListSecond.ClearReset();
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

        internal VectorRectangleEdgePoint GetLastSecondPoint()
        {
            return vectorRectangleList(parentVectoredRectangleGroupSecond)[fVectorRectangleListSecond.Count - 1];
        }

        internal VectorRectangleEdgePoint GetEdgeSliceForPointFromFirst(Point aPoint/*, VectoredRectangleGroup aGroup*/)
        {
            VectorRectangleEdgePoint result = null;
            VectorRectangleEdgePointList tmpEdgePointList = fVectorRectangleListFirst;// vectorRectangleList(aGroup);
            for (int i = 0; i < tmpEdgePointList.Count; i++)
            {
                if (tmpEdgePointList[i].vectorRectangle.p1.X == aPoint.X &&
                    tmpEdgePointList[i].vectorRectangle.p1.Y == aPoint.Y)
                    result = tmpEdgePointList[i];
            }
            return result;
        }
    }
}
