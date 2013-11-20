using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;

namespace SketchyGraph
{
    class BarChart : AxisPlot
    {

        public BarChart(Stroke xy_axis) {
            Straw puntos = new Straw(xy_axis);
            List<Point> resampled = puntos.Sampling();
            List<int> corners = puntos.GetCorners(resampled);
            StylusPointCollection pointcol_x = new StylusPointCollection();
            StylusPointCollection pointcol_y = new StylusPointCollection();
            for (int i = 0; i < resampled.Count; i++) {
                if (i <= corners[1]+1)
                    pointcol_y.Add(new StylusPoint(resampled[i].X, resampled[i].Y));
                if (i >= corners[1])
                    pointcol_x.Add(new StylusPoint(resampled[i].X, resampled[i].Y));
            }
            this.y = new Stroke(pointcol_y);
            this.x = new Stroke(pointcol_x);
        }

        public BarChart(Stroke x_axis, Stroke y_axis)
        {
            this.x = x_axis;
            this.y = y_axis;
        }

        public BarChart(int maxRange)
        {
            this.maxRange = maxRange;
        }

        public void AddBars(List<Object> bars)
        {
            foreach(Object obj in bars)
            {
                this.addElement(obj);
            }
        }

        
    }
}
