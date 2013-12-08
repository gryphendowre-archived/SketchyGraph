using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Windows.Ink;

namespace SketchyGraph
{
    public class RangeValue
    {
        public List<Unistroke> number = new List<Unistroke>();
        public List<Stroke> operation = new List<Stroke>();

        public double value;
        public bool state = true;
        Queue queue = new Queue();
        Node<string> tree;

        public string parse(List<Samples> samples)
        {
            string val = "";
            tree = new Node<string>("");
            while (operation.Count != 0)
            {
                Stroke sel = operation[0];
                operation.RemoveAt(0);
                Tuple<List<Stroke>, List<int>> check = Utils.RobustIntersection(sel, operation);
                if (check.Item1.Count == 1)
                {
                    Tuple<double, string, double> eval = Recognizer.RecognizedSelected(check.Item1, false, samples);
                    val += eval.Item2;
                }
                else
                {
                    Tuple<double, string, double> eval = Recognizer.RecognizedSelected(check.Item1, false, samples);
                    if (eval.Item2 == "star")
                    {
                        val += "*";
                    }
                    else
                    {
                        val += eval.Item2;
                    }
                    foreach (int index in check.Item2)
                        operation.RemoveAt(index);
                }
            }
            string number = "";
            foreach (char c in val)
            {
                if (char.IsDigit(c))
                    number += c;
                else {
                    queue.Enqueue(number);
                    queue.Enqueue(c);
                    number = "";
                }
            }
            if (number != "")
                queue.Enqueue(number);

            BuildTree();
            return val;
        }

        public void BuildTree()
        {
            int i = 1;
            foreach (string el in queue)
            {
                int n;
                if (int.TryParse(el, out n))
                {
                    if (tree.Data == "")
                    {
                        tree.Left = new Node<string>(el);
                    }
                    else
                    {
                        tree.Right = new Node<string>(el);
                        Node<string> parent = new Node<string>("");
                        parent.Left = tree;
                        tree = parent;
                    }
                }
                else
                {
                    tree.Data = el;
                }
                i++;
                if (i == queue.Count - 2)
                {
                    break;
                }
            }
        }

        public RangeValue(RangeValue rv) {
            this.number = Clone(rv.number);
            this.value = rv.value;
        }

        public RangeValue(Unistroke u, Stroke e) {
            if (number.Count == 0)
            {
                number.Add(u);
                operation.Add(e);
                this.value = this.getNumericalValue();
            }
            else
            {
                int index = getMePositioninListByBB(u);
                number.Insert(index, u);
                operation.Insert(index, e);
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
