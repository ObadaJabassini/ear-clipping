﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triangulation;

namespace Intersection
{
    public class Line
    {
        public Vector3m O { private set; get; }

        public Vector3m D { private set; get; }

        public Vector3m Eval(double t)
        {
            return O + D * t;
        }

        public Line(Vector3m o, Vector3m d)
        {
            O = o;
            D = D;
        }

        public Line(Line other): this(other.O, other.D) { }

        public double Project(Vector3m v)
        {
            double max = Helper.Max(Math.Abs(D.X), Math.Abs(D.Y), Math.Abs(D.Z));
            if (max == Math.Abs(D.X))
                return v.X;
            if (max == Math.Abs(D.Y))
                return v.Y;
            return v.Z;
        }

        public double Interval(Segment seg, double d1, double d2)
        {
            double p1 = Project(seg.V1), p2 = Project(seg.V2);
            return p1 + (p2 - p1) * (d1 / (d1 - d2));
        }
    }
}