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
{// Class to Recognize shapes on the ink canvas. THis class presents all the Heuristics for Primitive recognition.
    class Recognizer
    {

        Straw t;
        int numlines;
        List<double> linesizes;
        List<double> angles;
        String shaperecognized;

        // COnstructor for recognizer.
        public Recognizer(Straw stroke) {
            this.t = stroke;
            numlines = 0;
            linesizes = new List<double>();
            angles = new List<double>();
            shaperecognized = "None";
        }
        // return the number of lines in the stroke
        public int getNumLines() {
            return this.numlines;
        }

        // return the sizes of each line
        public List<double> getLineSizes() {
            return this.linesizes;
        }

        // return the angles between each one of the corners.
        public List<double> getAngles() {
            return this.angles;
        }

        // Shape recognized
        public String getShapeRecognized() {
            return this.shaperecognized;
        }

        // Function that recognize all the shapes defined in the assignment.
        public Shape Recognize(List<Point> sampledpoints, List<int> corners)
        {
            setNumberOfLinesSizeAndAngles(sampledpoints, corners);
            Shape def = new Rectangle();
            Shape figure1 = IsSquare(10,20);
            Shape figure2 = IsRectangle(15,20);
            Shape figure3 = IsTriangle(sampledpoints, corners, 10);
            Shape figure4 = IsCircle(sampledpoints, corners, 5);
            Shape figure5 = IsEllipse(sampledpoints, corners, 10);
            Shape figure6 = isArrow(sampledpoints, corners);

            if (figure1.StrokeThickness == 2)
            {
                this.shaperecognized = "Square";
                return figure1;
            }
            else if (figure2.StrokeThickness == 2)
            {
                this.shaperecognized = "Rectangle";
                return figure2;
            }else if( figure3.StrokeThickness == 2){
                this.shaperecognized = "Triangle";
                return figure3;
            }
            else if (figure4.StrokeThickness == 2)
            {
                this.shaperecognized = "Circle";
                return figure4;
            }
            else if (figure5.StrokeThickness == 2) {
                this.shaperecognized = "Ellipse";
                return figure5;
            }
            else if (figure6.StrokeThickness == 2) {
                this.shaperecognized = "Arrow";
                return figure6;
            }
            else
            {
                this.shaperecognized = "None";
                return def;
            }

        }
        // Return a Square or an empty shape.
        protected Shape IsSquare(int sizewindow, int anglewindow){
            Shape rect;
            rect = new Rectangle();

            double meanlines = Straw.Mean(this.linesizes.ToArray());
            int samelength = 0;
            foreach (double linesize in this.linesizes)
                if (linesize < (meanlines + sizewindow) && linesize > (meanlines - sizewindow))
                    samelength++;
            int contangle = 0;
            foreach (double angle in this.angles)
                if (angle > (90 - anglewindow) && angle < (90 + anglewindow))
                    contangle++;

            if (this.numlines == 4 && samelength == 4 && contangle == 4)
            {
                rect.Width = this.t.getBounds().Width;
                rect.Height = this.t.getBounds().Height;
                rect.Stroke = Brushes.Blue;
                rect.StrokeThickness = 2;
            }
            return rect;
        }
        // Returns a rectangle shape or an empty rectangle.
        protected Shape IsRectangle(int window, int anglewindow) {
            Shape rect;
            rect = new Rectangle();
            bool paralel = false;
            if(linesizes.Count > 3)
                if ((this.linesizes[0] >= (linesizes[2] - window)) && (this.linesizes[0] <= (linesizes[2] + window)) && (this.linesizes[1] >= (linesizes[3] - window)) && (this.linesizes[1] <= (linesizes[3] + window)))
                    paralel = true;
            
            int contangle = 0;
            foreach (double angle in this.angles)
                if (angle > (90 - anglewindow) && angle < (90 + anglewindow))
                    contangle++;

            if (this.numlines == 4 && paralel && contangle == 4)
            {
                rect.Width = this.t.getBounds().Width;
                rect.Height = this.t.getBounds().Height;
                rect.Stroke = Brushes.Blue;
                rect.StrokeThickness = 2;
            }
            return rect;
        }
        // return a triangle or an empty shape
        protected Shape IsTriangle(List<Point> sampledpoints, List<int> corners, int anglewindow) { 
            Polygon triangle = new Polygon();
            bool angles180 = false;

            double sumangle = 0.0;
            foreach (double angle in this.angles)
                sumangle += angle;
            if (Math.Abs(sumangle) > 180-anglewindow && Math.Abs(sumangle) < 180+anglewindow)
                angles180 = true;

            double distancefirstlast = Straw.Distance(sampledpoints[corners[0]], sampledpoints[corners[corners.Count - 1]]);

            if (this.numlines == 3 && angles180 && distancefirstlast > 0 && distancefirstlast < 30) {
                triangle.Stroke = Brushes.Blue;
                triangle.StrokeThickness = 2;
                PointCollection myPointCollection = new PointCollection();
                myPointCollection.Add(sampledpoints[corners[0]]);
                myPointCollection.Add(sampledpoints[corners[1]]);
                myPointCollection.Add(sampledpoints[corners[2]]);
                //myPointCollection.Add(sampledpoints[corners[3]]);
                triangle.Points = myPointCollection;
            }
            return triangle;
        }
        // Return a Circle or an empty Shape.
        protected Shape IsCircle(List<Point> sampledpoints, List<int> corners, int radiuswindow)
        {
            Shape circle = new Ellipse();
            if (corners.Count > 2 && sampledpoints.Count > 10)
            {
                Rect l = this.t.getBounds();
                Point a = new Point(l.Left + l.Width / 2, l.Top + l.Height / 2);
                double distancefirstlast = Straw.Distance(sampledpoints[corners[0]], sampledpoints[corners[corners.Count - 1]]);

                List<double> distancesbb = new List<double>(4);
                distancesbb.Add(Straw.Distance(a, new Point(l.Left, l.Top + l.Height / 2)));
                distancesbb.Add(Straw.Distance(a, new Point(l.Right, l.Top + l.Height / 2)));
                distancesbb.Add(Straw.Distance(a, new Point(l.Left + l.Width / 2, l.Top)));
                distancesbb.Add(Straw.Distance(a, new Point(l.Left + l.Width / 2, l.Bottom)));

                double meandistances = Straw.Mean(distancesbb.ToArray());
                int sameradius = 0;
                foreach (double distance in distancesbb)
                    if (distance > (meandistances - radiuswindow) && distance < (meandistances + radiuswindow))
                        sameradius++;

                List<double> distancesIn = new List<double>();
                int allradius = 0;
                int cut = sampledpoints.Count / 8;
                for (int i = 0; i < sampledpoints.Count; i += cut)
                    distancesIn.Add(Straw.Distance(a, sampledpoints[i]));
                foreach (double distance in distancesIn)
                    if (distance > (meandistances - 10) && distance < (meandistances + 10))
                        allradius++;

                if (this.numlines < 4 && sameradius == 4 && (allradius >= ((sampledpoints.Count / cut) - 1) && allradius <= ((sampledpoints.Count / cut) + 1)) && distancefirstlast > 0 && distancefirstlast < 15)
                {
                    circle.Width = l.Width;
                    circle.Height = l.Height;
                    circle.StrokeThickness = 2;
                    circle.Stroke = Brushes.Blue;
                }
            }
            return circle;
        }
        // Return an ellipse or an empty shape
        protected Shape IsEllipse(List<Point> sampledpoints, List<int> corners, int window)
        {
            Shape ellipse = new Ellipse();
            Rect l = this.t.getBounds();
            Point a = new Point(l.Left + l.Width / 2, l.Top + l.Height / 2);
            double distancefirstlast = Straw.Distance(sampledpoints[corners[0]], sampledpoints[corners[corners.Count - 1]]);
            bool isellipse = false;

            List<double> distancesbb = new List<double>(4);
            distancesbb.Add(Straw.Distance(a, new Point(l.Left, l.Top + l.Height / 2)));
            distancesbb.Add(Straw.Distance(a, new Point(l.Right, l.Top + l.Height / 2)));
            distancesbb.Add(Straw.Distance(a, new Point(l.Left + l.Width / 2, l.Top)));
            distancesbb.Add(Straw.Distance(a, new Point(l.Left + l.Width / 2, l.Bottom)));

            if ((distancesbb[0] >= (distancesbb[1] - window)) && (distancesbb[0] <= (distancesbb[1] + window)) && (distancesbb[2] >= (distancesbb[3] - window)) && (distancesbb[2] <= (distancesbb[3] + window)))
                isellipse = true;

            double gx = -1;
            double gy = -1;
            double lx = 9999;
            double ly = 9999;
            int igx = 0;
            int igy = 0;
            int ilx = 0;
            int ily = 0;
            for (int i = 0; i < sampledpoints.Count; i++) { 
                if(gx < sampledpoints[i].X){
                    gx = sampledpoints[i].X;
                    igx = i;
                }
                if (lx > sampledpoints[i].X) {
                    lx = sampledpoints[i].X;
                    ilx = i;
                }
                if (gy < sampledpoints[i].Y)
                {
                    gy = sampledpoints[i].Y;
                    igy = i;
                }
                if (ly > sampledpoints[i].Y)
                {
                    ly = sampledpoints[i].Y;
                    ily = i;
                }
            }

            double d1 = Straw.Distance(a,sampledpoints[igx]);
            double d2 = Straw.Distance(a, sampledpoints[ilx]);
            double d3 = Straw.Distance(a, sampledpoints[igy]);
            double d4 = Straw.Distance(a, sampledpoints[ily]);

            if ( d1 >= (d2 - window) && d1 <= (d2 + window) && d3 >= (d4 - window) && d3 <= (d4 + window) && isellipse && this.numlines < 4 && distancefirstlast > 0 && distancefirstlast < 15)
            {
                ellipse.Width = l.Width;
                ellipse.Height = l.Height;
                ellipse.StrokeThickness = 2;
                ellipse.Stroke = Brushes.Blue;
            }
            return ellipse;
        }
        
        //Return a polygon or an empty shape.
        protected Shape isArrow(List<Point> sampledpoints, List<int> corners) {
            Polygon arrow = new Polygon();
            double distancefirstlast = Straw.Distance(sampledpoints[corners[0]], sampledpoints[corners[corners.Count - 1]]);

            if (this.numlines == 6 && distancefirstlast > 0 && distancefirstlast < 30 && (this.linesizes[0] > (this.linesizes[this.linesizes.Count - 1] - 20)) && (this.linesizes[0] < (this.linesizes[this.linesizes.Count - 1] + 20)))
            {
                arrow.Stroke = Brushes.Blue;
                arrow.StrokeThickness = 2;

                PointCollection myPointCollection = new PointCollection();
                foreach (int corner in corners) {
                    myPointCollection.Add(sampledpoints[corner]);
                }
                arrow.Points = myPointCollection;
            }
            return arrow;
        }
        // Detecting lines, and angles measures.
        protected void setNumberOfLinesSizeAndAngles(List<Point> sampledpoints, List<int> corners)
        {
            if (corners.Count > 1)
            {
                if (Straw.IsLine(sampledpoints, corners[0], corners[corners.Count - 1]))
                {
                    this.numlines++;
                    this.linesizes.Add(Straw.Distance(sampledpoints[corners[0]], sampledpoints[corners[corners.Count - 1]]));
                }

                Vector OL = new Vector(sampledpoints[corners[0]].X - sampledpoints[corners[corners.Count - 2]].X, sampledpoints[corners[0]].Y - sampledpoints[corners[corners.Count - 2]].Y);
                Vector OF = new Vector(sampledpoints[corners[0]].X - sampledpoints[corners[1]].X, sampledpoints[corners[0]].Y - sampledpoints[corners[1]].Y);
                this.angles.Add(Vector.AngleBetween(OL, OF));

                for (int i = 1; i < corners.Count; i++)
                {
                    if (Straw.IsLine(sampledpoints, corners[i - 1], corners[i]))
                    {
                        this.numlines++;
                        this.linesizes.Add(Straw.Distance(sampledpoints[corners[i - 1]], sampledpoints[corners[i]]));
                    }
                    if (i != (corners.Count - 1))
                    {
                        Vector OQ = new Vector(sampledpoints[corners[i]].X - sampledpoints[corners[i - 1]].X, sampledpoints[corners[i]].Y - sampledpoints[corners[i - 1]].Y);
                        Vector OM = new Vector(sampledpoints[corners[i]].X - sampledpoints[corners[i + 1]].X, sampledpoints[corners[i]].Y - sampledpoints[corners[i + 1]].Y);
                        this.angles.Add(Vector.AngleBetween(OQ, OM));
                    }
                }
            }
        }

    }
}
