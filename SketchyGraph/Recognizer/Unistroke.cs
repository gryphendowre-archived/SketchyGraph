using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Collections;

namespace SketchyGraph
{
    public class Unistroke : IComparable
    {
        public List<Point> points;
        public List<Point> oldpoints;
        bool usingboundedrotationinvariance = true;
        public static int SIZE = 250;
        public double d = 0.3;
        public Point O = new Point(0d, 0d);
        public int I = 12;
        public Vector vector;
        public string value;
        private static readonly double Phi = 0.5 * (-1 + Math.Sqrt(5)); // Golden Ratio

        public Unistroke(){
            this.points = new List<Point>();
            this.oldpoints = new List<Point>();
        }

        public Unistroke(List<Point> pointssrc) { 
            this.points = pointssrc;
            this.oldpoints = pointssrc;
        }

        public Unistroke(List<Point> pointssrc, string val)
        {
            this.points = pointssrc;
            this.oldpoints = pointssrc;
            this.value = val;
        }

        public Unistroke(int size)
        {
            this.points = new List<Point>(size);
            this.oldpoints = new List<Point>(size);
        }

        // Nested class to do ascending sort on boundingbox property.
        private class sortStrokeAscendingHelperBB : IComparer
        {
            int IComparer.Compare(object a, object b)
            {
                Unistroke c1 = (Unistroke)a;
                Unistroke c2 = (Unistroke)b;

                double x = Unistroke.BoundingBox(c1.points).Left;
                double y = Unistroke.BoundingBox(c2.points).Left;

                if (x < y)
                    return 1;
                if (x > y)
                    return -1;
                else
                    return 0;
            }
        }

        // Implement IComparable CompareTo to provide default sort order.
        int IComparable.CompareTo(object obj)
        {
            //Unistroke c = (Unistroke)obj;
            //return String.Compare(this.make, c.make);
            return -1;
        }

        // Method to return IComparer object for sort helper.
        public static IComparer sortUnistrokeAscending()
        {
            return (IComparer)new sortStrokeAscendingHelperBB();
        }

        public List<Point> Resample(List<Point> pointssrc, int n)
        {
            double D = 0.0;
            List<Point> _points = new List<Point>(pointssrc);
            List<Point> sampledpoints = new List<Point>(n);
            double I = PathLength(_points) / (n - 1);
            sampledpoints.Add(_points[0]);
            for (int i = 1; i < _points.Count; i++)
            {

                double d = Utils.Distance(_points[i - 1], _points[i]); //distance
                if ((D + d) >= I)
                {
                    Point q = new Point();
                    q.X = _points[i - 1].X + ((I - D) / d) * (_points[i].X - _points[i - 1].X);
                    q.Y = _points[i - 1].Y + ((I - D) / d) * (_points[i].Y - _points[i - 1].Y);
                    sampledpoints.Add(q);
                    _points.Insert(i, q);
                    D = 0.0;
                }
                else // if the distance between points is not enough continue.
                    D = D + d;
            }
            if (sampledpoints.Count == n - 1)
                sampledpoints.Add(_points[points.Count - 1]);

            return sampledpoints;
        }

        private double PathLength(List<Point> A) {
            double d = 0;
            for (int i = 1; i < A.Count; i++)
                d += Utils.Distance(A[i - 1], A[i]);
            return d;
        }

        public double IndicativeAngle(List<Point> pointssrc)
        {
            Point c = Centroid(pointssrc);
            return Math.Atan2(c.Y - pointssrc[0].Y, c.X - pointssrc[0].X);
        }

        private Point Centroid(List<Point> pointssrc)
        {
            double xsum = 0.0;
            double ysum = 0.0;
            foreach (Point point in pointssrc)
            {
                xsum += point.X;
                ysum += point.Y;
            }
            return new Point(xsum / pointssrc.Count, ysum / pointssrc.Count);
        }

        public double[] DistanceAtBestAngle(List<Point> pointssrc, Unistroke T, double thetaa, double thetab, double thetadelta, double omega) {
            omega = Phi;
            double x1 = (omega * thetaa) + (1 - omega)*thetab;
            double f1 = DistanceAtAngle(pointssrc, T, x1);
            double x2 = ((1 - omega) * thetaa) + (omega * thetab);
            double f2 = DistanceAtAngle(pointssrc, T, x2);
            while (Math.Abs(thetab - thetaa) > thetadelta) {
                if (f1 < f2)
                {
                    thetab = x2;
                    x2 = x1;
                    f2 = f1;
                    x1 = (omega * thetaa) + ((1 - omega) * thetab);
                    f1 = DistanceAtAngle(pointssrc, T, x1);
                }
                else {
                    thetaa = x1;
                    x1 = x2;
                    f1 = f2;
                    x2 = ((1 - omega) * thetaa) + (omega * thetab);
                    f2 = DistanceAtAngle(pointssrc, T, x2);
                }
            }
            return new double[2] {Math.Min(f1, f2), Utils.Rad2Deg((thetaa + thetab)/2.0)};
        }

        public double[] GoldenSectionSearch(List<Point> pts1, List<Point> pts2, double a, double b, double threshold)
        {
            double x1 = Phi * a + (1 - Phi) * b;
            List<Point> newPoints = RotateBy(pts1, x1);
            double fx1 = PathDistance(newPoints, pts2);

            double x2 = (1 - Phi) * a + Phi * b;
            newPoints = RotateBy(pts1, x2);
            double fx2 = PathDistance(newPoints, pts2);

            double i = 2.0; // calls
            while (Math.Abs(b - a) > threshold)
            {
                if (fx1 < fx2)
                {
                    b = x2;
                    x2 = x1;
                    fx2 = fx1;
                    x1 = Phi * a + (1 - Phi) * b;
                    newPoints = RotateBy(pts1, x1);
                    fx1 = PathDistance(newPoints, pts2);
                }
                else
                {
                    a = x1;
                    x1 = x2;
                    fx1 = fx2;
                    x2 = (1 - Phi) * a + Phi * b;
                    newPoints = RotateBy(pts1, x2);
                    fx2 = PathDistance(newPoints, pts2);
                }
                i++;
            }
            return new double[3] { Math.Min(fx1, fx2), Utils.Rad2Deg((b + a) / 2.0), i }; // distance, angle, calls to pathdist
        }


        public double DistanceAtAngle(List<Point> pointssrc, Unistroke T, double theta) {
            double d;
            List<Point> newPoints = RotateBy(pointssrc, theta);
            d = PathDistance(newPoints, T.points);
            return d;
        }

        public double PathDistance(List<Point> A, List<Point> B) {
            double d = 0.0;
            for (int i = 0; i < A.Count; i++)
                d = d + Utils.Distance(A[i], B[i]);
            return d / A.Count;
        }

        public List<Point> RotateBy(List<Point> points, double radians)
        {
            List<Point> newPoints = new List<Point>(points.Count);
            Point c = Centroid(points);

            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            double cx = c.X;
            double cy = c.Y;

            for (int i = 0; i < points.Count; i++)
            {
                Point p = (Point)points[i];

                double dx = p.X - cx;
                double dy = p.Y - cy;

                Point q = new Point();
                q.X = dx * cos - dy * sin + cx;
                q.Y = dx * sin + dy * cos + cy;

                newPoints.Add(q);
            }
            return newPoints;
        }

        public List<Point> ScaleDimTo(List<Point> pointssrc, int size, double d)
        {
            List<Point> newPoints = new List<Point>();
            Rect B = BoundingBox(pointssrc);
            foreach (Point p in pointssrc)
            {
                double qx = 0d;
                double qy = 0d;
                if (Math.Min((B.Width / B.Height), (B.Height / B.Width)) <= d)
                {
                    qx = (p.X * size) / Math.Max(B.Width, B.Height);
                    qy = (p.Y * size) / Math.Max(B.Width, B.Height);
                }
                else
                {
                    qx = (p.X * size) / B.Width;
                    qy = (p.Y * size) / B.Height;
                }
                newPoints.Add(new Point(qx, qy));
            }
            return newPoints;
        }

        public static Rect BoundingBox(List<Point> pointssrc)
        {
            double minx = double.MaxValue;
            double maxx = double.MinValue;
            double miny = double.MaxValue;
            double maxy = double.MinValue;
            foreach (Point p in pointssrc)
            {
                if (p.X < minx)
                    minx = p.X;
                if (p.X > maxx)
                    maxx = p.X;
                if (p.Y < miny)
                    miny = p.Y;
                if (p.Y > maxy)
                    maxy = p.Y;
            }
            return new Rect(minx, miny, maxx - minx, maxy - miny);
        }

        public List<Point> CheckRestoreOrientation(List<Point> pointssrc, double w)
        {
            List<Point> newPoints = new List<Point>();
            if (this.usingboundedrotationinvariance)
            {
                newPoints = RotateBy(pointssrc, w);
                return newPoints;
            }
            else
                return pointssrc;
        }

        public List<Point> TranslateTo(List<Point> pointssrc, Point k)
        {
            List<Point> newPoints = new List<Point>(pointssrc.Count);

            Rect r = BoundingBox(pointssrc);
            for (int i = 0; i < points.Count; i++)
            {
                Point p = (Point)points[i];
                p.X += (k.X - r.X);
                p.Y += (k.Y - r.Y);
                newPoints.Add(p);
            }
            return newPoints;
        }

        public Vector CalcStartUnitVector(List<Point> pointssrc, int I)
        {
            Vector v = new Vector();
            double qx = pointssrc[I].X - pointssrc[0].X;
            double qy = pointssrc[I].Y - pointssrc[0].Y;
            v.X = qx / Math.Sqrt(Math.Pow(qx, 2d) + Math.Pow(qy, 2d));
            v.Y = qy / Math.Sqrt(Math.Pow(qx, 2d) + Math.Pow(qy, 2d));
            return v;
        }

    }
}
