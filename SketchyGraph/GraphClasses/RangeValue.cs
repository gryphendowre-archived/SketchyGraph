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
using NCalc;

namespace SketchyGraph
{
    public class RangeValue
    {
        public List<Unistroke> number = new List<Unistroke>();
        public List<Stroke> operation = new List<Stroke>();

        public double value;
        public bool state = true;
        public bool modified = false;
        Queue queue = new Queue();
        Node<string> tree;

        public void parse(List<Samples> samples)
        {
            string val = "";
            queue.Clear();
            List<Stroke> operation_copy = new List<Stroke>();
            foreach (Stroke str in operation)
                operation_copy.Add(str.Clone());
            tree = new Node<string>("");
            while (operation_copy.Count != 0)
            {
                Stroke sel = operation_copy[0];
                operation_copy.RemoveAt(0);
                Tuple<List<Stroke>, List<int>> check = Utils.RobustIntersection(sel, operation_copy);
                if (check.Item1.Count == 1)
                {
                    Tuple<double, string, double> eval = Recognizer.RecognizedSelected(check.Item1, false, samples);
                    if(eval.Item1 >= 0.75)
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
                        operation_copy.RemoveAt(index);
                }
            }
            string number = "";
            foreach (char c in val)
            {
                if (char.IsDigit(c))
                    number += c;
                else {
                    queue.Enqueue(number);
                    queue.Enqueue(c+"");
                    number = "";
                }
            }
            if (number != "")
                queue.Enqueue(number);

            BuildTree();
            string expression = tree.Traverse();
            int n;

            if (expression != "")
            {
                char c = expression[expression.Length-1];
                if (c != '+' && c!='-' )
                {
                    NCalc.Expression res = new NCalc.Expression(expression);
                    this.value = Convert.ToDouble(res.Evaluate().ToString());
                }
            }
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
            }
        }

        public RangeValue(RangeValue rv) {
            this.number = Clone(rv.number);
            this.value = rv.value;
            this.modified = true;
        }

        public RangeValue(Unistroke u, Stroke e) {
            if (number.Count == 0)
            {
                number.Add(u);
                operation.Add(e);
            }
            else
            {
                int index = getMePositioninListByBB(u);
                number.Insert(index, u);
                operation.Insert(index, e);
            }
            this.modified = true;
        }

        public RangeValue(Stroke e) {
            operation.Add(e);
            this.modified = true;
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

        public Rect getBounds() { 
            List<Point> listp = new List<Point>();
            foreach(Stroke e in this.operation)
                listp.AddRange(Utils.TransformStrokeToListPoints(e));
            if (listp.Count == 0)
                return new Rect();
            else
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
