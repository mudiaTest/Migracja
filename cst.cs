﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Migracja
{
    class Cst
    {
        //ważna jest kolejność - zgodnie z ruchem wskazówek zegara!
        internal const int noFromEdge = 0;
        internal const int fromLeftToRight = 1;
        internal const int fromTopToBottom = 2;
        internal const int fromRightToLeft = 3;
        internal const int fromBottomToTop = 4;

        internal const int goTop = 0;
        internal const int goRight = 1;
        internal const int goBottom = 2;
        internal const int goLeft = 3;

        internal const string progName = "Ras2Vec";

        internal const int maxZoom = 10;
    }

    class Dir
    {
        internal static int Next(int aDir)
        {
            /* if (aDir == Cst.fromLeftToRight)
                return Cst.fromTopToBottom;
            else if (aDir == Cst.fromTopToBottom)
                return Cst.fromRightToLeft;
            else if (aDir == Cst.fromRightToLeft)
                return Cst.fromBottomToTop;
            else if (aDir == Cst.fromBottomToTop)
                return Cst.fromLeftToRight;
            else
                Debug.Assert(false, "Nieznana wartość kierunku: " + aDir.ToString());*/
            if (aDir > 0 && aDir < 4)
                return ++aDir;
            else if (aDir == 4)
                return Cst.fromLeftToRight;
            else
            {
                Debug.Assert(false, "Niepoprawna wartość kierunku: " + aDir.ToString());
                return 0;
            }
        }

        internal static int Next(int? aDir)
        {
            int result = 0;
            if (aDir == null)
                Debug.Assert(false, "Nie można wskazać kolejnego kierunku dla wartości null");
            else
            {
                result = Next((int) aDir);
            }
            return result;
        }

        internal static int Prv(int aDir)
        {
            if (aDir > 1 && aDir <= 4)
                return aDir--;
            else if (aDir == 1)
                return Cst.fromBottomToTop;
            else
            {
                Debug.Assert(false, "Niepoprawna wartość kierunku: " + aDir.ToString());
                return 0;
            }
        }

        internal static int NextCheck(int aDir)
        {
             if (aDir == Cst.fromLeftToRight)
                return Cst.fromBottomToTop;
            else if (aDir == Cst.fromTopToBottom)
                return Cst.fromLeftToRight;
            else if (aDir == Cst.fromRightToLeft)
                return Cst.fromTopToBottom;
             else if (aDir == Cst.fromBottomToTop)
                 return Cst.fromRightToLeft;
             else
             {
                 Debug.Assert(false, "Nieznana wartość kierunku: " + aDir.ToString());
                 return 0;
             }
        }

        internal static int NextCheck(int? aDir)
        {
            int result = 0;
            if (aDir == null)
                Debug.Assert(false, "Nie można wskazać kolejnego kierunku dla wartości null");
            else
            {
                result = NextCheck((int)aDir);
            }
            return result;
        }

        internal static int OuterCornerCheck(int aDir)
        {
            if (aDir == Cst.fromLeftToRight)
                return Cst.fromTopToBottom;
            else if (aDir == Cst.fromTopToBottom)
                return Cst.fromRightToLeft;
            else if (aDir == Cst.fromRightToLeft)
                return Cst.fromBottomToTop;
            else if (aDir == Cst.fromBottomToTop)
                return Cst.fromLeftToRight;
            else
            {
                Debug.Assert(false, "Nieznana wartość kierunku: " + aDir.ToString());
                return 0;
            }
        }

        internal static int OuterCornerCheck(int? aDir)
        {
            int result = 0;
            if (aDir == null)
                Debug.Assert(false, "Nie można wskazać kolejnego kierunku dla wartości null");
            else
            {
                result = OuterCornerCheck((int)aDir);
            }
            return result;
        }
    }
}
