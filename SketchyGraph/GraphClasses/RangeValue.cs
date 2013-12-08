using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Ink;

namespace SketchyGraph
{
    public class RangeValue
    {
        public List<Unistroke> number = new List<Unistroke>();
        public double value;
        public bool state = true;

        public RangeValue(RangeValue rv) {
            this.number = Clone(rv.number);
            this.value = rv.value;
        }

        public RangeValue(Unistroke u) {
            if (number.Count == 0)
            {
                number.Add(u);
                this.value = this.getNumericalValue();
            }
            else
            {
                int index = getMePositioninListByBB(u);
                number.Insert(index, u);
                this.value = this.getNumericalValue();
            }
        }

        public List<Unistroke> Clone(List<Unistroke> lu) {
            List<Unistroke> copy = new List<Unistroke>();
            foreach (Unistroke u in lu) {
                copy.Add(u);
            }
            return copy;
        }

        public void sortUnistrokebyBB() {
            //Array.Sort(number.ToArray(), Unistroke.sortUnistrokeAscending());
            //number.ToList();
            double q = this.getNumericalValue();
            int a = 8;
        }

        public int getMePositioninListByBB(Unistroke newu) {
            sortUnistrokebyBB();
            int i = 0;
            if (Unistroke.BoundingBox(newu.points).Left < Unistroke.BoundingBox(number[0].points).Left)
                return i;
            else
            {
                foreach (Unistroke u in number)
                {
                    if (Unistroke.BoundingBox(newu.points).Left > Unistroke.BoundingBox(u.points).Left)
                        return i;
                    i++;
                }
            }
            return i;
        }

        public Rect getBoundingBox() {
            List<Point> listp = new List<Point>();
            foreach (Unistroke u in number) {
                listp.AddRange(u.points);
            }
            return Unistroke.BoundingBox(listp);
        }

        public double getNumericalValue(){
            List<Unistroke> copy = this.Clone(this.number);
            copy.Reverse();
            double acum = 0;
            int i = 1;
            foreach (Unistroke u in copy) {
                acum += Convert.ToDouble(u.value) * i;
                i *= 10;
            }
            this.value = acum;
            return this.value;
        }
        
    }
}
