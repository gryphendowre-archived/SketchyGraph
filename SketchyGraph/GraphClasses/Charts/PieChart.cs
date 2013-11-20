using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SketchyGraph
{
    class PieChart : BaseGraph
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

        public override void CalculateBoundingBoxes(double threshold)
        {
            Rect r = new Rect();
        }

    }
}
