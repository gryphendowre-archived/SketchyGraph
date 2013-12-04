using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SketchyGraph
{
    public abstract class BaseGraph
    {
        public double maxRange;
        public int minRange = 0;
        public int maxDomain;
        public int minDomain = 0;
        public string type;
        public bool hasbeenmodified = false;
        public bool hasbeendrawn = false;
        public double thresh = 10.0;
        public List<RangeValue> rval = new List<RangeValue>();
        public List<Element> elements = new List<Element>();

        public void addElement(Element element)
        {
            elements.Add(element);
        }

        public void AddRangeValue(Unistroke u){
            if (rval.Count == 0)
                rval.Add(new RangeValue(u));
            else {
                Rect r = Unistroke.BoundingBox(u.points);
                int i = getIndexofRangeValues(r);
                if (i >= rval.Count)
                    rval.Add(new RangeValue(u));
                else
                {
                    rval[i].number.Add(u);
                    rval[i].sortUnistrokebyBB();
                }
            }
        }

        public int getIndexofRangeValues(Rect r) {
            int i = 0;
            double centroidr = r.Top + (r.Height / 2);
            foreach (RangeValue rv in rval)
            {
                Rect rt = rv.getBoundingBox();
                double centroid = rt.Top + (rt.Height / 2);
                if (centroidr > centroid - thresh && centroidr < centroid + thresh )
                {
                    return i;
                }
                i++;
            }
            return i;
        }

        public int getIndexofRangeValues(double rtop)
        {
            int i = 0;
            foreach (RangeValue rv in rval)
            {
                Rect rt = rv.getBoundingBox();
                double centroid = rt.Top + (rt.Height / 2);
                if (rtop < centroid + thresh)
                {
                    return i;
                }
                i++;
            }
            return i;
        }

        public double returnMaxRange() {
            double least = 99999;
            double value = -1;
            foreach (RangeValue rv in rval)
            {
                double top = rv.getBoundingBox().Top;
                if (top < least)
                {
                    least = top;

                    value = rv.getNumericalValue();
                }
            }
            return value;
        }

        public abstract void CalculateBoundingBoxes(double threshold);
    }
}
