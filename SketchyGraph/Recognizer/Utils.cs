using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Windows.Media;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace SketchyGraph
{
    class Utils
    { 

        public static void Heap_Permute(int n, List<int> order, List<List<int>> orders)
        {
            if (n == 1)
                orders.Add(new List<int>(order));
            else
            {
                for (int i = 0; i < n; i++)
                {
                    Heap_Permute(n - 1, order, orders);
                    if (IsOdd(n))
                        Swap(order, 0, n - 1);
                    else
                        Swap(order, i, n - 1);
                }
            }
        }

        public static bool BitAt(int b, int i)
        {
            if (((b >> i) & 1) == 1)
                return true;
            return false;
        }

        public static bool IsOdd(int n)
        {
            if (n % 2 == 0)
                return false;
            return true;
        }

        public static void Swap(IList<int> order, int index1, int index2)
        {
            int temp = order[index1];
            order[index1] = order[index2];
            order[index2] = temp;
        }
        
        public static double Median(double[] sourceNumbers)
        {
            if (sourceNumbers == null || sourceNumbers.Length == 0)
                return 0D;
            double[] sortedPNumbers = (double[])sourceNumbers.Clone();
            sourceNumbers.CopyTo(sortedPNumbers, 0);
            Array.Sort(sortedPNumbers);
            int size = sortedPNumbers.Length;
            int mid = size / 2;
            double median = (size % 2 != 0) ? (double)sortedPNumbers[mid] : ((double)sortedPNumbers[mid] + (double)sortedPNumbers[mid - 1]) / 2;
            return median;
        }

        public static double Mean(double[] sourceNumbers)
        {
            double acum = 0.0;
            foreach (double n in sourceNumbers)
            {
                acum += n;
            }
            return acum / sourceNumbers.Length;
        }

        public static double Distance(Point p, Point q)
        {
            return Math.Sqrt(Math.Pow(p.X - q.X, 2.0) + Math.Pow(p.Y - q.Y, 2.0));
        }

        public static double PathDistance(List<Point> points, int a, int b)
        {
            double d = 0;
            for (int i = a; i < b - 1; i++)
                d = d + Distance(points[i], points[i + 1]);
            return d;
        }

        public static double Rad2Deg(double rad)
        {
            return (rad * 180d / Math.PI);
        }

        public static double Deg2Rad(double deg)
        {
            return (deg * Math.PI / 180d);
        }

        public String printCorners(List<int> corners)
        {
            String Scorners = "";
            for (int i = 0; i < corners.Count; i++)
            {
                Scorners += "(" + corners[i].ToString() + "),";
            }
            return Scorners;
        }

        public static String printDoubles(List<double> numbers)
        {
            String Scorners = "";
            for (int i = 0; i < numbers.Count; i++)
            {
                Scorners += "(" + numbers[i].ToString() + "),";
            }
            return Scorners;
        }

        public String printTrazo(List<Point> points)
        {
            String strokepoints = "";
            for (int i = 0; i < points.Count; i++)
            {
                strokepoints += "(" + points[i].X.ToString() + "," + points[i].Y.ToString() + "),";
            }
            return strokepoints;
        }

        public String printPoints(List<Point> points)
        {
            String stringpoints = "";
            for (int i = 0; i < points.Count; i++)
            {
                stringpoints += "(" + points[i].X.ToString() + "," + points[i].Y.ToString() + "),";
            }
            return stringpoints;
        }

        public static Tuple<List<Stroke>, List<int>> RobustIntersection(Stroke si, List<Stroke> CS)
        {
            List<Stroke> temp = new List<Stroke>();
            List<int> indexes = new List<int>();
            temp.Add(si);
            for (int i = 0; i < CS.Count; i++)
                if (CircleIntersection(si, CS[i]))
                {
                    temp.Add(CS[i]);
                    indexes.Add(i);
                }
            return new Tuple<List<Stroke>, List<int>>(temp, indexes);
        }

        private static bool CircleIntersection(Stroke s1, Stroke s2)
        {
            List<Point> P = TransformStrokeToListPoints(s1);
            List<Point> Q = TransformStrokeToListPoints(s2);
            Straw e1 = new Straw(s1);
            e1.SamplingExperiment();
            Straw e2 = new Straw(s2);
            e2.SamplingExperiment();
            for (int i = 0; i < e1.getPoints().Count; i++)
                for (int j = 0; j < e2.getPoints().Count; j++)
                {
                    //if (i <= j) {
                    bool val = Intersect(new Circle(e1.getPoints()[i], s1.DrawingAttributes.Width), new Circle(e2.getPoints()[j], s2.DrawingAttributes.Width));
                    if (val)
                        return true;
                }
            //}
            return false;
        }

        private static bool Intersect(Circle c1, Circle c2)
        {
            double dist = Utils.Distance(c1.Center, c2.Center);
            if (dist > (c1.Radius + c2.Radius))
                return false;
            else if (dist < Math.Abs(c1.Radius - c2.Radius))
                return false;
            else if (dist == 0)
                return true;
            else if (dist > ((c1.Radius + c2.Radius) * 0.9) || dist < ((c1.Radius + c2.Radius) * 1.1))
                return true;
            else
                return true;
        }

        public static PointCollection TransformListToPointCollection(List<Point> points)
        {
            PointCollection pcol = new PointCollection(points.Count);
            for (int i = 0; i < points.Count; i++)
                pcol.Add(points[i]);
            return pcol;
        }

        public static List<Unistroke> TransformStrokesToUnistrokes(List<Stroke> points)
        {
            List<Unistroke> pcol = new List<Unistroke>(points.Count);
            foreach (Stroke e in points)
                pcol.Add(new Unistroke(TransformStrokeToListPoints(e)));
            return pcol;
        }

        public static List<Point> TransformStrokesToPointCollection(List<Stroke> points)
        {
            List<Point> pcol = new List<Point>(points.Count);
            foreach (Stroke e in points)
                foreach (StylusPoint p in e.StylusPoints)
                    pcol.Add(new Point(p.X, p.Y));
            return pcol;
        }

        public static List<Point> TransformStrokeToListPoints(Stroke e)
        {
            List<Point> pcol = new List<Point>(e.StylusPoints.Count);
            foreach (StylusPoint p in e.StylusPoints)
                pcol.Add(new Point(p.X, p.Y));
            return pcol;
        }

        public static List<Samples> ReadFiles(string folderpath)
        {
            List<Samples> samples = new List<Samples>();
            foreach (string subdirectory in Directory.GetDirectories(folderpath))
            {
                string[] name_gest = subdirectory.Split(new Char[] { '\\' });
                Samples sam = new Samples(name_gest[name_gest.Length - 1]);

                foreach (string file in Directory.EnumerateFiles(subdirectory, "*.ink"))
                {
                    FileStream fs = null;
                    List<Unistroke> gesture;
                    try
                    {
                        fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                        StrokeCollection strokes = new StrokeCollection(fs);
                        gesture = new List<Unistroke>(strokes.Count);
                        foreach (Stroke stroke in strokes)
                        {
                            Unistroke unistroke = new Unistroke(stroke.StylusPoints.Count);
                            foreach (Point p in stroke.StylusPoints)
                                unistroke.points.Add(p);
                            gesture.Add(unistroke);
                        }
                        sam.samples.Add(gesture);
                    }
                    finally
                    {
                        if (fs != null)
                            fs.Close();
                    }
                }
                samples.Add(sam);
            }
            return samples;
        }

    }
}
