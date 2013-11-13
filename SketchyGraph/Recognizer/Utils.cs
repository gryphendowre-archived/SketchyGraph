using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Ink;

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
    }
}
