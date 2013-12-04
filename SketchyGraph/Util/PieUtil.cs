using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SketchyGraph.GraphClasses.Charts;
using System.Windows.Ink;

namespace SketchyGraph.Util
{
    public static class PieUtil
    {
        private static double threshold = 0.90;

        /*
         * Helper method to determine if a user directly creates one slice, instead of a straight line in the PieChart
         */
        public static bool trueSlice(Point[] pts, PieChart chart, double radius)
        {
            double totalDist = 0.0;
            double doubleRad = 2*radius;
            double ratio = 0.0;
            double startPoint = 0.0;
            double endPoint = 0.0;

            for (int i = 0; i < pts.Count() - 1; i++)
            {
                totalDist = totalDist + GeneralUtil.EuclideanDistance(pts[i], pts[i + 1]);
            }

            if (doubleRad > totalDist)
            {
                ratio = totalDist / doubleRad;
            }
            else
            {
                ratio = doubleRad / totalDist;
            }

            if (ratio > threshold)
            {
                startPoint = GeneralUtil.EuclideanDistance(pts[0], chart.GetCenterPoint());
                endPoint = GeneralUtil.EuclideanDistance(pts[pts.Count() - 1], chart.GetCenterPoint());
                if ((FindStartRatio(radius, startPoint) > threshold) && (FindStartRatio(radius, endPoint) > threshold))
                {
                    return true;
                }
            }

            return false;
        }

        /*
         * Helper method to find the Starting Point of a draw and to see if it is accurately inside
         * the PieChart
         */ 
        public static double FindStartRatio(double radius, double startPoint)
        {
            if (startPoint >= radius)
            {
                return radius / startPoint;
            }
            else
            {
                return startPoint / radius;
            }
        }
        /*
         * Helper method to find the Starting Point of a draw and to see if it is accurately inside
         * the PieChart
         */
        public static double FindEndRatio(double radius, double endPoint)
        {
            if (endPoint >= radius)
            {
                return radius / endPoint;
            }
            else
            {
                return endPoint / radius;
            }
        }

        public static bool IsInsideCheck(Point[] pts, Rect boundBox)
        {
            foreach (Point pt in pts)
            {
                if ((pt.X > boundBox.TopLeft.X) && (pt.X < boundBox.BottomRight.X) &&
                    (pt.Y > boundBox.TopLeft.Y) && (pt.Y < boundBox.BottomRight.Y))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
