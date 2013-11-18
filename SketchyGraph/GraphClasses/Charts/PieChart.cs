using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SketchyGraph.GraphClasses.Charts
{
    class PieChart : AxisPlot
    {

        public PieChart()
        {
        }

        public void addSlices(List<Object> slices)
        {
            foreach (Object obj in slices)
            {
                this.addElement(obj);
            }
        }

        public override Rect GetBoundingBox()
        {
            Rect r = new Rect();
            return r;
        }

    }
}
