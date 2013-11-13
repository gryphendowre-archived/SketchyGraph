using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Windows.Media;

namespace SketchyGraph
{
    class Straw
    {
        List<Point> points;
        List<Point> apoints;
        Rect bounds;
        double S;

        // Constructor for Trazo which transform the Stylus points in a list of points for simplicity.
        public Straw(InkCanvasStrokeCollectedEventArgs e)
        {
            this.bounds = e.Stroke.GetBounds();
            this.S = (Math.Sqrt(Math.Pow(this.bounds.Width, 2) + Math.Pow(this.bounds.Height, 2))) / 40.0;
            this.points = new List<Point>(e.Stroke.StylusPoints.Count);
            for (int j = 0; j < e.Stroke.StylusPoints.Count; j++)
            {
                Point p = new Point();
                p.X = e.Stroke.StylusPoints[j].X;
                p.Y = e.Stroke.StylusPoints[j].Y;
                this.points.Add(p);
            }
            this.apoints = new List<Point>();
        }

        public Straw(Stroke e) {
            this.bounds = e.GetBounds();
            this.S = (Math.Sqrt(Math.Pow(this.bounds.Width, 2) + Math.Pow(this.bounds.Height, 2))) / 40.0;
            this.points = new List<Point>(e.StylusPoints.Count);
            for (int j = 0; j < e.StylusPoints.Count; j++)
            {
                Point p = new Point();
                p.X = e.StylusPoints[j].X;
                p.Y = e.StylusPoints[j].Y;
                this.points.Add(p);
            }
            this.apoints = new List<Point>();
        }

        public List<Point> getPoints() {
            return this.points;
        }

        public List<Point> getSampledPoints() {
            return this.apoints;
        }

        //Return the attribute for the rectangle bounding box of the stroke.
        public Rect getBounds(){
            return this.bounds;
        }
        
        // Keeps all the points at the same distance
        public List<Point> Sampling()
        {
            double D = 0.0;
            List<Point> sampledpoints = new List<Point>();
            int j = 0;
            sampledpoints.Add(new Point(this.points[0].X, this.points[0].Y));
            for (int i = 1; i < this.points.Count; i++)
            {
                double d = Distance(this.points[i - 1], this.points[i]); //distance
                if ((D + d) >= this.S) // if the distance is greater than the diagonal/40.0.
                {
                    Point q = new Point();
                    q.X = (int)(this.points[i - 1].X + ((this.S - D) / d) * (this.points[i].X - this.points[i - 1].X));
                    q.Y = (int)(this.points[i - 1].Y + ((this.S - D) / d) * (this.points[i].Y - this.points[i - 1].Y));
                    sampledpoints.Add(q);
                    this.apoints.Insert(j, q);
                    D = 0.0;
                    j += 1;
                }
                else // if the distance between points is not enough continue.
                    D = D + d;
            }
            return sampledpoints;
        }

        public void SamplingExperiment()
        {
            for (int i = 0; i < this.points.Count - 1; i++)
            {
                double d = Distance(this.points[i], this.points[i + 1]); //distance
                while (d > 1.00)
                {
                    this.points.Insert(i + 1, new Point((this.points[i].X + this.points[i + 1].X) / 2, (this.points[i].Y + this.points[i + 1].Y) / 2));
                    d = Distance(this.points[i], this.points[i + 1]);
                }
            }
        }

        // Function that finds the point in original closest to the point in resampled
        public List<int> TransformCoordinatesFromSampledtoOriginal(List<Point> resampled, List<int> corners, int window) {
            List<int> cornersoriginal = new List<int>(corners.Count);
            for (int i = 1; i < corners.Count - 1;  i++) {
                double distance = 9999.0;
                int index = -1;
                for (int j = corners[i] - window; j < corners[i] + window; j++) {
                    if (Distance(this.points[j], resampled[corners[i]]) < distance)
                    {
                        distance = Distance(this.points[j], resampled[corners[i]]);
                        index = j;
                    }
                }
                cornersoriginal.Add(index);
            }
            return cornersoriginal;
        }

        // Return a list of indexes where corners were found from the list of points.
        public List<int> GetCorners(List<Point> points) {
            List<int> corners = new List<int>();
            corners.Add(0);
            int W = 3;
            if (points.Count >= 2 * W)
            {
                double[] straws = new double[points.Count - (2 * W)];
                for (int i = W; i < points.Count - W; i++)
                    straws[i - W] = Distance(points[i - W], points[i + W]);
                double t = Mean(straws) * 0.95;
                for (int i = W; i < points.Count - W; i++)
                {
                    if (straws[i - W] < t)
                    {
                        double localMin = 100000.0;
                        int localMinIndex = i;
                        while (i < straws.Length && straws[i - W] < t)
                        {
                            if (straws[i - W] < localMin)
                            {
                                localMin = straws[i - W];
                                localMinIndex = i;
                            }
                            i += 1;
                        }
                        corners.Add(localMinIndex);
                    }
                }
                corners.Add(points.Count - 1);
                corners = PostProcessCorners(points, corners, straws);
            }
            return corners;
        }
        // Identify wrong corners or missed ones.
        public List<int> PostProcessCorners(List<Point> points, List<int> corners, double[] straws) {
            bool continuar;
            do
            {
                continuar = true;
                for (int i = 1; i < corners.Count; i++)
                {
                    int c1 = corners[i - 1];
                    int c2 = corners[i];
                    if (!IsLine(points, c1, c2)) {
                        int newcorner = HalfwayCorner(straws, c1, c2);
                        if (newcorner > c1 && newcorner < c2)
                        {
                            corners.Insert(i, newcorner);
                            continuar = false;
                        }
                    }
                }
            } while (!continuar);
            for (int i = 1; i < corners.Count - 1; i++) {
                int c1 = corners[i - 1];
                int c2 = corners[i + 1];
                if (IsLine(points, c1, c2)) {
                    corners.RemoveAt(i);
                    i -= 1;
                }
            }
            return corners;
        }
        // Returns if there is a line between two points.
        public static bool IsLine(List<Point> points, int a, int b) {
            double threshold = 0.95;
            double distance = Distance(points[a] , points[b]);
            double pathdistance = PathDistance(points, a, b);
            if ((distance / pathdistance ) > threshold)
                return true;
            else
                return false;
        }

        // Function to evaluate if there is anny corner missed by false corner detection.
        protected int HalfwayCorner(double[] straws, int a, int b) {
            int quarter = (b - a) / 2;
            double minValue = 1000000;
            int minIndex = -1;
            for (int i = a + quarter; i < b - quarter - 1; i++) {
                if (straws[i] < minValue) {
                    minValue = straws[i];
                    minIndex = i;
                }
            }
            return minIndex;
        }

        protected static double Median(double[] sourceNumbers) {
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

        public static double Mean(double[] sourceNumbers) {
            double acum = 0.0;
            foreach ( double n in sourceNumbers ){
                acum += n;
            }
            return acum / sourceNumbers.Length;
        }

        public static double Distance(Point p, Point q){
            return Math.Sqrt(Math.Pow(p.X - q.X, 2.0) + Math.Pow(p.Y - q.Y, 2.0));
        }

        public static double PathDistance(List<Point> points, int a, int b) {
            double d = 0;
            for (int i = a; i < b - 1; i++)
                d = d + Distance(points[i], points[i+1]);
            return d;
        }

        public String printCorners(List<int> corners) {
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

        public String printStraw() {
            String strokepoints = "";
            for (int i = 0; i < this.points.Count; i++)
            {
                strokepoints += "(" + points[i].X.ToString() + "," + points[i].Y.ToString() + "),";
            }
            return strokepoints;
        }

        public String printPoints(List<Point> points) {
            String stringpoints = "";
            for (int i = 0; i < points.Count; i++)
            {
                stringpoints += "(" + points[i].X.ToString() + "," + points[i].Y.ToString() + "),";
            }
            return stringpoints;
        }
    }
}
