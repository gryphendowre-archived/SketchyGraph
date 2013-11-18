using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Ink;

namespace SketchyGraph
{
    class BarChart : AxisPlot
    {

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
