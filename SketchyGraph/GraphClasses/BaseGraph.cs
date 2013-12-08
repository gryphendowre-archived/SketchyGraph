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
        public bool blocked = false;

        public void addElement(Element element)
        {
            elements.Add(element);
        }

        public void quicksort(List<RangeValue> input, int low, int high)
        {
            int pivot_loc = 0;

            if (low < high)
            {
                pivot_loc = partition(input, low, high);
                quicksort(input, low, pivot_loc - 1);
                quicksort(input, pivot_loc + 1, high);
            }
        }

        private int partition(List<RangeValue> input, int low, int high)
        {
            Rect pivotbb = input[high].getBoundingBox();
            double pivot = pivotbb.Top + (pivotbb.Height / 2);
            int i = low - 1;
            for (int j = low; j < high; j++)
            {
                Rect bb = input[j].getBoundingBox();
                double center = bb.Top + (bb.Height / 2);
                if ( center <= pivot)
                {
                    i++;
                    swap(input, i, j);
                }
            }
            swap(input, i + 1, high);
            return i + 1;
        }

        public void swap(List<RangeValue> listvalues, int a, int b)
        {
            RangeValue temp = listvalues[a];
            listvalues[a] = listvalues[b];
            listvalues[b] = temp;
        }

        /*private void swap(List<RangeValue> listvalues, int a, int b) {
            RangeValue temp = new RangeValue(listvalues[a]);
            listvalues[a] = new RangeValue(listvalues[b]);
            listvalues[b] = new RangeValue(temp);
        }*/

        public void AddRangeValue(Unistroke u){
            if (rval.Count == 0)
            {
                rval.Add(new RangeValue(u));
                this.maxRange = Convert.ToDouble(u.value);
            }
            else
            {
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

            if (rval.Count > 1)
            {
                this.quicksort(this.rval, 0, this.rval.Count - 1);
                this.maxRange = Convert.ToDouble(this.rval[0].value);
            }
        }

        public List<RangeValue> validateRangeValues(){
            List<RangeValue> redvals = new List<RangeValue>();
            foreach(RangeValue rv in this.rval)
                rv.state = true;
            if (this.rval.Count > 1)
            {
                int i = 0;
                int j = this.rval.Count - 1;
                int k = 0;
                int l = this.rval.Count - 1;
                do
                {
                    i++;
                    j--;
                    if (this.rval[i].value > this.rval[k].value || this.rval[i].state == false)
                        this.rval[i].state = false;
                    else
                        k++;
                    if (this.rval[j].value < this.rval[l].value || this.rval[j].state == false)
                        this.rval[j].state = false;
                    else
                        l--;
                } while (i < j);

                if (this.rval[0].value < this.rval[this.rval.Count - 1].value)
                {
                    this.rval[0].state = true;
                    this.rval[this.rval.Count - 1].state = false;
                }
            }
            foreach (RangeValue rv in this.rval)
                if (rv.state == false)
                    redvals.Add(new RangeValue(rv));
            return redvals;
        }

        private void ReverseArray(ref double[] input)
        {
            Stack<double> tmp = new Stack<double>();
            foreach (int i in input)
                tmp.Push(i);
            input = tmp.ToArray();
        }

        private int inactivateByValue(double val){
            int index = -1, i = 0;
            foreach (RangeValue rv in this.rval){
                if (rv.value == val)
                {
                    rv.state = false;
                    index = i;
                }
                i++;
            }
            return index;
        }

        private double[] getmeAListofvalues() {
            double[] values = new double[this.rval.Count];
            int i = 0;
            foreach (RangeValue rv in this.rval) {
                values[i] = rv.value;
                i++;
            }
            return values;
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
