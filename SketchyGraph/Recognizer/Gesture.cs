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
{// This class has the heuristic for the recognition of the gestures scribble which is like a M, and selection which is a line that passes through the elements you would like to manipulate.
    class Gesture
    {
        Straw t;
        int numlines;
        List<double> linesizes;
        List<double> angles;
        String gesture;

        // Constructor to identify gestures.
        public Gesture(Straw stroke) {
            this.t = stroke;
            numlines = 0;
            linesizes = new List<double>();
            angles = new List<double>();
            gesture = "None";
        }

        // Returns the string of the name of the gesture.
        public String getGestureString() {
            return this.gesture;
        }
        // Function that returns the gesture recognized
        public String RecognizeGesture(List<Point> sampledpoints, List<int> corners){
            setNumberOfLinesSizeAndAngles(sampledpoints, corners);
            if (IsScribble(sampledpoints, corners, 10)) {
                this.gesture = "Scribble";
            }else if(IsGrouping(sampledpoints, corners, 15)){
                this.gesture = "Grouping";
            }
            return this.gesture;
        }

        // Scribble for delete strokes
        protected bool IsScribble(List<Point> sampledpoints, List<int> corners, int epsilon)
        {
            int contangle = 0;
            foreach (double angle in this.angles)
                if (angle > -45 && angle < 45)
                    contangle++;

            if(this.angles.Count == contangle && this.numlines > 2)
                return true;
            return false;
        }

        // Gesture to manipulate a set of gestures.
        private bool IsGrouping(List<Point> sampledpoints, List<int> corners, int epsilon)
        {
            double distancefirstlast = Straw.Distance(sampledpoints[corners[0]], sampledpoints[corners[corners.Count - 1]]);
            if (distancefirstlast > 200)
            {
                return true;
            }
            return false;
        }
 


        // Function to set lines, sizes and angles.
        protected void setNumberOfLinesSizeAndAngles(List<Point> sampledpoints, List<int> corners)
        {
            if (corners.Count > 2)
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
