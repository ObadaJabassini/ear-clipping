﻿using GeoAPI.Geometries;
using GeoAPI.Geometries.Prepared;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.IO;
using NetTopologySuite.Operation.Polygonize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vr_Gl
{
    public class SplitPolygonExample
    {
        internal static IGeometry Polygonize(IGeometry geometry)
        {
            var lines = LineStringExtracter.GetLines(geometry);
            var polygonizer = new Polygonizer();
            polygonizer.Add(lines);
            var polys = polygonizer.GetPolygons();
            var polyArray = GeometryFactory.ToGeometryArray(polys);
            return geometry.Factory.CreateGeometryCollection(polyArray);
        }

        internal static IGeometry PolygonizeForClip(IGeometry geometry, IPreparedGeometry clip)
        {
            var lines = LineStringExtracter.GetLines(geometry);
            var clippedLines = new List<IGeometry>();
            foreach (ILineString line in lines)
            {
                if (clip.Contains(line))
                    clippedLines.Add(line);
            }
            var polygonizer = new Polygonizer();
            polygonizer.Add(clippedLines);
            var polys = polygonizer.GetPolygons();
            var polyArray = GeometryFactory.ToGeometryArray(polys);
            return geometry.Factory.CreateGeometryCollection(polyArray);
        }

        internal static IGeometry SplitPolygon(IGeometry polygon, IGeometry line)
        {
            var nodedLinework = polygon.Boundary.Union(line);
            var polygons = Polygonize(nodedLinework);

            // only keep polygons which are inside the input
            var output = new List<IGeometry>();
            for (var i = 0; i < polygons.NumGeometries; i++)
            {
                var candpoly = (IPolygon)polygons.GetGeometryN(i);
                if (polygon.Contains(candpoly.InteriorPoint))
                    output.Add(candpoly);
            }
            /*
            return polygon.Factory.CreateGeometryCollection(
                GeometryFactory.ToGeometryArray(output));
             */
            return polygon.Factory.BuildGeometry(output);
        }

        internal static IGeometry ClipPolygon(IGeometry polygon, IPolygonal clipPolygonal)
        {
            var clipPolygon = (IGeometry)clipPolygonal;
            var nodedLinework = polygon.Boundary.Union(clipPolygon.Boundary);
            var polygons = Polygonize(nodedLinework);

            /*
            // Build a prepared clipPolygon
            var prepClipPolygon = NetTopologySuite.Geometries.Prepared.PreparedGeometryFactory.Prepare(clipPolygon);
                */

            // only keep polygons which are inside the input
            var output = new List<IGeometry>();
            for (var i = 0; i < polygons.NumGeometries; i++)
            {
                var candpoly = (IPolygon)polygons.GetGeometryN(i);
                var interiorPoint = candpoly.InteriorPoint;
                if (polygon.Contains(interiorPoint) &&
                    /*prepClipPolygon.Contains(candpoly)*/
                    clipPolygon.Contains(interiorPoint))
                    output.Add(candpoly);
            }
            /*
            return polygon.Factory.CreateGeometryCollection(
                GeometryFactory.ToGeometryArray(output));
                */
            return polygon.Factory.BuildGeometry(output);
        }
        

        internal void Run()
        {
            var reader = new WKTReader();
            var polygon = reader.Read("POLYGON((0 0, 0 100, 100 100, 100 0, 0 0), (10 10, 90 10, 90 90, 10 90, 10 10))");

            var lineWkts = new[]
                               {
                                   "LINESTRING(50 -10, 50 110)",
                                   "LINESTRING(5 -10, 5 110)",
                                   "LINESTRING(5 -10, 5 95, 110 95)",
                                   "LINESTRING(5 -10, 5 110, 110 50)"
                               };

            Console.WriteLine(string.Format("Splitting\n{0}", polygon));
            foreach (var lineWkt in lineWkts)
            {
                var line = reader.Read(lineWkt);
                Console.WriteLine(string.Format("\nwith\n{0}", lineWkt));
                var splitPolygons = SplitPolygon(polygon, line);
                Console.WriteLine(string.Format("results in:\n{0}", splitPolygons));
            }
        }

        internal void RunClip()
        {
            var reader = new WKTReader();
            var polygon = reader.Read("POLYGON((0 0, 0 100, 100 100, 100 0, 0 0), (10 10, 90 10, 90 90, 10 90, 10 10))");

            var clipPolygonWkts = new[]
                               {
                                   "POLYGON((-10 45, 110 45, 110 55, -10 55, -10 45))",
                               };

            Console.WriteLine(string.Format("Clipping\n{0}", polygon));
            foreach (var lineWkt in clipPolygonWkts)
            {
                var polygonal = (IPolygonal)reader.Read(lineWkt);
                Console.WriteLine(string.Format("\nwith\n{0}", lineWkt));
                var clippedPolygons = ClipPolygon(polygon, polygonal);
                Console.WriteLine(string.Format("results in:\n{0}", clippedPolygons));
            }
        }
    }
}
