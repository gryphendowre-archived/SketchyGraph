using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SketchyGraph.GraphClasses.Charts
{
    class PieChart : ChartClass
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

    }
}
