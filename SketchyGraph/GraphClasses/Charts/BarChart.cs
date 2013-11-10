using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SketchyGraph.GraphClasses.Charts
{
    class BarChart : ChartClass
    {

        public BarChart(int maxRange, int minRange)
        {
            this.maxRange = maxRange;
            this.minRange = minRange;
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
