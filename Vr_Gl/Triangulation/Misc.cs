﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Triangulation
{
    class Misc
    {
        public static int GetOrientation(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 normal)
        {
           
            var res = (v0 - v1).Cross(v2 - v1);
            if (res.LengthSquared() == 0)
                return 0;
           
            if (Math.Sign(res.X) != Math.Sign(normal.X) || Math.Sign(res.Y) != Math.Sign(normal.Y) || Math.Sign(res.Z) != Math.Sign(normal.Z))
                return 1;
            return -1;
        }

        // Is testPoint between a and b in ccw order?
        // > 0 if strictly yes
        // < 0 if strictly no
        // = 0 if testPoint lies either on a or on b
        public static int IsBetween(Vector3 Origin, Vector3 a, Vector3 b, Vector3 testPoint, Vector3 normal)
        {
            var psca = GetOrientation(Origin, a, testPoint, normal);
            var pscb = GetOrientation(Origin, b, testPoint, normal);

            // where does b in relation to a lie? Left, right or collinear?
            var psb = GetOrientation(Origin, a, b, normal);
            if (psb > 0) // left
            {
                // if left then testPoint lies between a and b iff testPoint left of a AND testPoint right of b
                if (psca > 0 && pscb < 0)
                    return 1;
                if (psca == 0)
                {
                    var t = a - Origin;
                    var t2 = testPoint - Origin;
                    if (Math.Sign(t.X) != Math.Sign(t2.X) || Math.Sign(t.Y) != Math.Sign(t2.Y))
                        return -1;
                    return 0;
                }
                else if (pscb == 0)
                {
                    var t = b - Origin;
                    var t2 = testPoint - Origin;
                    if (Math.Sign(t.X) != Math.Sign(t2.X) || Math.Sign(t.Y) != Math.Sign(t2.Y))
                        return -1;
                    return 0;
                }
            }
            else if (psb < 0) // right
            {
                // if right then testPoint lies between a and b iff testPoint left of a OR testPoint right of b
                if (psca > 0 || pscb < 0)
                    return 1;
                if (psca == 0)
                {
                    var t = a - Origin;
                    var t2 = testPoint - Origin;
                    if (Math.Sign(t.X) != Math.Sign(t2.X) || Math.Sign(t.Y) != Math.Sign(t2.Y))
                        return 1;
                    return 0;
                }
                else if (pscb == 0)
                {
                    var t = b - Origin;
                    var t2 = testPoint - Origin;
                    if (Math.Sign(t.X) != Math.Sign(t2.X) || Math.Sign(t.Y) != Math.Sign(t2.Y))
                        return 1;
                    return 0;
                }
            }
            else if (psb == 0)
            {
                if (psca > 0)
                    return 1;
                else if (psca < 0)
                    return -1;
                else
                    return 0;
            }
            return -1;
        }

        public static bool PointInOrOnTriangle(Vector3 prevPoint, Vector3 curPoint, Vector3 nextPoint, Vector3 nonConvexPoint, Vector3 normal)
        {
            var res0 = Misc.GetOrientation(prevPoint, nonConvexPoint, curPoint, normal);
            var res1 = Misc.GetOrientation(curPoint, nonConvexPoint, nextPoint, normal);
            var res2 = Misc.GetOrientation(nextPoint, nonConvexPoint, prevPoint, normal);
            return res0 != 1 && res1 != 1 && res2 != 1;
        }

        public static double PointLineDistance(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return (p2 - p1).Cross(p3 - p1).LengthSquared();
        }
    }
}