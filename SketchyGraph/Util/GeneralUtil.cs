using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SketchyGraph.Util
{
    public static class GeneralUtil
    {
        public static double EuclideanDistance(Point a, Point b)
        {
            double deltX = b.X - a.X;
            double deltY = b.Y - a.Y;
            return Math.Sqrt(Math.Pow(deltX, 2.0) + Math.Pow(deltY, 2.0));
        }
    }
}
