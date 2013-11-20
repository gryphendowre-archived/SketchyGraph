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

        public Value(string n, Unistroke str) {
            this.val = n;
            this.stroke_val = str;
            this.y_val = Unistroke.BoundingBox(stroke_val.points).TopLeft.Y - Unistroke.BoundingBox(stroke_val.points).BottomLeft.Y;
            this.x_val = Unistroke.BoundingBox(stroke_val.points).TopLeft.X - Unistroke.BoundingBox(stroke_val.points).BottomLeft.X;
    	}

    }
}
