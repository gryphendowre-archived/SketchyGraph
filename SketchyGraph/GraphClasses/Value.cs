using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SketchyGraph
{
    public class Value
    {
        public string val { get; set; }
        public Unistroke stroke_val { get; set; }
        public double y_val;
        public double x_val;

        public Value()
        {
        }

        public Value(string n, Unistroke str) {
            this.val = n;
            this.stroke_val = str;
            double a = Unistroke.BoundingBox(stroke_val.points).TopLeft.X;
            double b = Unistroke.BoundingBox(stroke_val.points).TopRight.X;
            double c = Unistroke.BoundingBox(stroke_val.points).TopLeft.Y;
            double d = Unistroke.BoundingBox(stroke_val.points).BottomLeft.Y;
            this.x_val = a + (b-a)/2;
            this.y_val = c + (d-c)/2;
    	}

    }
}
