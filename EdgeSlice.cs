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
            //uwagi odnośnie punktów z kierunkiem Dir.noFromEdge. Z ich natury wynika, że jeśli taki punkt 
            // -pojawi się w fVectorRectangleListFirst, to nie powinien być przenoszony do second - to jest sytuacja 
            //  gdy z "wewnętrnego" konta budujemy " wypukły"
            //- przypadki z Dir.noFromEdge na końcach nas nie interesują
            //- przypadki, gdy należy dodać dodatkowy punkit z kierunkiem Dir.noFromEdge sa obsługiwane przez AddOppositeDirectionEdgePoint 
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
                        if (fVectorRectangleListFirst[i].directionNull != Dir.noFromEdge)
                            edgePoint = AddOppositeDirectionEdgePoint(fVectorRectangleListFirst[i], prvEdgePoint, fVectorRectangleListSecond);
                        if (edgePoint != null)
                            prvEdgePoint = edgePoint;
                    }
                    //ostatni punkt opisujemy jako końcowy
                    Debug.Assert(parentVectoredRectangleGroupSecond != null, "Podobiekt parentVectoredRectangleGroupSecond jest null.");
                    prvEdgePoint.GroupsEndingPoint = parentVectoredRectangleGroupSecond;
                }
                return fVectorRectangleListSecond;
            }
        }

        //dodajemy następny punkt do listy poprzedzając go dodatkowym punktema kierunkiem Dir.noFromEdge jeśli to koniecznie
        private VectorRectangleEdgePoint AddOppositeDirectionEdgePoint(VectorRectangleEdgePoint aPoint,
                                                   VectorRectangleEdgePoint aPrvPoint,
                                                   VectorRectangleEdgePointList aVectorRectangleListSecond)
        {
            VectorRectangleEdgePoint result = GetOppositeDirectionEdgePoint(aPoint);
            if (result != null)
            {
                //zakładamy, że aPrvPoint jest null gdy 
                // - jest to pierwszy punkt z listy
                // - poprzedni punkt aPoint miał Direction = Dir.noFromEdge (wewnętrzna cześć granicy)
                //W obu przypadkach nie ma potrzeby dla dodawania punktu z granicą Dir.noFromEdge
                if (aPrvPoint != null && Dir.NextCheck(aPrvPoint.Direction()) == result.Direction())
                    aVectorRectangleListSecond.Add(aVectorRectangleListSecond.NextKey(),
                                                   new VectorRectangleEdgePoint(result.vectorRectangle, Dir.noFromEdge)
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
                if (aPoint.Direction() == Dir.fromLeftToRight)
                {
                    tmpPoint = new Point(aPoint.vectorRectangle.p1.X, aPoint.vectorRectangle.p1.Y - 1);
                    tmpDirection = Dir.fromRightToLeft;
                }
                else if (aPoint.Direction() == Dir.fromRightToLeft)
                {
                    tmpPoint = new Point(aPoint.vectorRectangle.p1.X, aPoint.vectorRectangle.p1.Y + 1);
                    tmpDirection = Dir.fromLeftToRight;
                }
                else if (aPoint.Direction() == Dir.fromTopToBottom)
                {
                    tmpPoint = new Point(aPoint.vectorRectangle.p1.X + 1, aPoint.vectorRectangle.p1.Y);
                    tmpDirection = Dir.fromBottomToTop;
                }
                else if (aPoint.Direction() == Dir.fromBottomToTop)
                {
                    tmpPoint = new Point(aPoint.vectorRectangle.p1.X - 1, aPoint.vectorRectangle.p1.Y);
                    tmpDirection = Dir.fromTopToBottom;
                }
                else if (aPoint.Direction() == Dir.noFromEdge)
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
            return (int)edgeVectRectanglesCount;
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
            return (int)simplifiedEdgeVectRectanglesCount;
        }

        internal VectoredRectangleGroup parent = null;
    }

    class EdgeList : Dictionary<int, EdgeSliceList>
    {
        private int fMaxKey;

        internal int maxKey
        {
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
}
