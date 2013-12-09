using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;

namespace SketchyGraph
{
    public class Element
    {
        public Value domain_val { get; set; }
        public Value range_val { get; set; }
        public Unistroke plot { get; set; }
        public Stroke str { get; set; }
        public Stroke color_str { get; set; }

        public Element() {}

        public bool isInsidePlot(Point p){
            if (plot != null)
            {
                Rect bbplot = Unistroke.BoundingBox(this.plot.points);
                if ((p.X > bbplot.Left) && (p.X < bbplot.Right) && (p.Y > bbplot.Top) && (p.Y < bbplot.Bottom))
                    return true;
            }
            return false;
        }

    }
}
