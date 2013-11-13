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
    class Trazo
    {
        static double omega = 30.0;
        static int N = 96;

        public static Unistroke Combine_Strokes(List<Unistroke> strokes) {
            Unistroke combined = new Unistroke();
            foreach( Unistroke stroke in strokes){
                for( int j = 0; j < stroke.points.Count; j++)
                {
                    Point p = new Point();
                    p.X = stroke.points[j].X;
                    p.Y = stroke.points[j].Y;
                    combined.points.Add(p);
                    combined.oldpoints.Add(p);
                }
            }
            return combined;
        }

        public static List<Unistroke> MakeUnistrokes(List<Unistroke> strokes, List<List<int>> orders)
        {
            List<Unistroke> unistrokes = new List<Unistroke>();
            foreach (List<int> order in orders)
            {
                for (int b = 0; b < Math.Pow(2d, order.Count); b++)
                {
                    Unistroke unistroke = new Unistroke();
                    for (int i = 0; i < order.Count; i++)
                    {
                        Unistroke stroke = new Unistroke(strokes[(int)(order[i])].points);
                        if (Utils.BitAt(b, i))
                            stroke.points.Reverse();
                        unistroke.points.AddRange(stroke.points);
                        unistroke.oldpoints.AddRange(stroke.points);
                    }
                    unistrokes.Add(unistroke);
                }
            }
            return unistrokes;
        }

        public static List<Unistroke> GenerateUnistrokePermutations(List<Unistroke> strokes)
        {
            List<int> order = new List<int>(strokes.Count);
            List<Unistroke> M = new List<Unistroke>();
            List<List<int>> orders = new List<List<int>>();
            for (int i = 0; i < strokes.Count; i++)
                order.Add(i);
            Utils.Heap_Permute(strokes.Count, order, orders);
            M = MakeUnistrokes(strokes, orders);
            foreach (Unistroke U in M) {
                U.points = U.Resample(U.points, N);
                double w = U.IndicativeAngle(U.points);
                U.points = U.RotateBy(U.points, -w);
                U.points = U.ScaleDimTo(U.points, Unistroke.SIZE, U.d);
                U.points = U.CheckRestoreOrientation(U.points, +w);
                U.points = U.TranslateTo(U.points, U.O);
                U.vector = U.CalcStartUnitVector(U.points, U.I);
            }
            return M;
        }

        public static Tuple<Unistroke, double, string, double> Recognize(Unistroke stroke, Vector v, int S, List<Samples> multistrokes)
        {
            double b = double.MaxValue;
            double score;
            string name = "";
            double bestangle = -1.0;
            Unistroke Mprima = new Unistroke();
            foreach (Samples sam in multistrokes)
            {
                foreach (List<Unistroke> N in sam.samples)
                {
                    List<Unistroke> M = GenerateUnistrokePermutations(N);
                    //if (S == N.Count)
                    //{
                        foreach (Unistroke U in M)
                        {
                            double angle = Vector.AngleBetween(stroke.vector, U.vector);
                            //double anotherangle = Utils.Rad2Deg(Math.Acos(stroke.vector.X * U.vector.X + stroke.vector.Y * U.vector.Y));
                            if (Math.Abs(angle) <= omega)
                            {
                                double[] d = U.GoldenSectionSearch(stroke.points, U.points, Utils.Deg2Rad(-1 * 45.0), Utils.Deg2Rad(45.0), Utils.Deg2Rad(2.0));
                                //double[] d = U.DistanceAtBestAngle(stroke.points, U, Utils.Deg2Rad(-1 * 45.0), Utils.Deg2Rad(45.0), Utils.Deg2Rad(2.0), omega);//U.PathDistance(stroke.points, U.points);
                                if (d[0] < b)
                                {
                                    b = d[0];
                                    Mprima = U;
                                    name = sam.name;
                                    bestangle = d[1];
                                }
                            }
                        }
                    //}
                }
            }
            score = 1 - (b / (0.5 * Math.Sqrt(Math.Pow(Unistroke.SIZE, 2.0) + Math.Pow(Unistroke.SIZE, 2.0))));
            return new Tuple<Unistroke,double, string, double>(Mprima,score, name, bestangle);
        }
    }
}