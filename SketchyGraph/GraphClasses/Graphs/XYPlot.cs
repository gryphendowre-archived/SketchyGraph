using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SketchyGraph
{
    class XYPlot : AxisPlot
    {
        public XYPlot(int xMax, int xMin, int yMax, int yMin)
        {
            this.maxDomain = xMax;
            this.minDomain = xMin;
            this.maxRange = yMax;
            this.minRange = yMin;
        }

        public XYPlot(int xMax, int yMax)
        {
            this.maxDomain = xMax;
            this.maxRange = yMax;
        }

        //Should we keep this? Or should we just have Points in the BaseGraph class?
        public void AddPoints(List<Point> points)
        {
            foreach (Point pt in points)
            {
                Object x = (Object)pt;
                this.addElement(x);
            }
        }
    }
}
