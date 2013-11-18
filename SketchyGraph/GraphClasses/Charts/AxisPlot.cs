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
        protected Stroke x;
        protected Stroke y;

        public override Rect GetBoundingBox() {
            Rect x_bounds = x.GetBounds();
            Rect y_bounds = y.GetBounds();
            double delta_x = x_bounds.Left - y_bounds.Left;
            Rect r = new Rect((y_bounds.TopLeft.X - delta_x), y_bounds.TopLeft.Y, x_bounds.Width, x_bounds.Height);
            return r;
        }

    }
}
