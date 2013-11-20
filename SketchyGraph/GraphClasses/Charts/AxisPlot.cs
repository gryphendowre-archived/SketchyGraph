using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Ink;

namespace SketchyGraph
{
    class AxisPlot : BaseGraph
    {
        public Stroke x;
        public Stroke y;
        public Rect bb;
        public Rect x_bounds;
        public Rect y_bounds;
        public Rect plot_bound;

        public override void CalculateBoundingBoxes(double threshold)
        {
            this.x_bounds = new Rect(x.GetBounds().TopLeft.X, x.GetBounds().TopLeft.Y, x.GetBounds().Width, x.GetBounds().Height + 2 * threshold);
            this.y_bounds = new Rect(y.GetBounds().TopLeft.X - 2 * threshold, y.GetBounds().TopLeft.Y, y.GetBounds().Width + 2 * threshold, y.GetBounds().Height);
            this.plot_bound = new Rect(y_bounds.TopRight.X, y_bounds.TopRight.Y, x_bounds.TopRight.X - y_bounds.TopRight.X, x_bounds.TopRight.Y - y_bounds.TopRight.Y);
            this.bb = new Rect((this.y_bounds.TopLeft.X - threshold / 2), (this.y_bounds.TopLeft.Y - threshold / 2), (x_bounds.Width + y_bounds.Width + threshold/2), (y_bounds.Height + x_bounds.Height + threshold/2));
        }

        public string GiveMeAreaChart(Rect rect){
            if (isInside(rect, bb)) { 
                if(isInside(rect, x_bounds))
                    return "x_bounds";
                else if(isInside(rect, y_bounds))
                    return "y_bounds";
                else if(isInside(rect, plot_bound))
                    return "plot_bound";
            }
            return "none";
        }

        private bool isInside(Rect rect, Rect bounds) { 
            if(rect.TopLeft.X > bounds.TopLeft.X && rect.TopLeft.Y > bounds.TopLeft.Y && rect.BottomRight.X < bounds.BottomRight.X && rect.BottomRight.Y < bounds.BottomRight.Y)
                return true;
            return false;
        }

    }
}
