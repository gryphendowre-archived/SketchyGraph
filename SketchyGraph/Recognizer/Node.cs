using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SketchyGraph
{
    class Node<T> where T : IComparable
    {
        private T data;
        public Node<T> Left, Right;

        public Node(T item)
        {
            data = item;
            Left = null;
            Right = null;
        }
        public T Data
        {
            set { data = value; }
            get { return data; }
        }

        public override string ToString()
        {
            var leftString = this.Left != null ? this.Left.ToString() : "null";
            var rightString = this.Right != null ? this.Right.ToString() : "null";
            var dataString = this.Data != null ? this.Data.ToString() : "null";

            leftString = String.Join("\n", leftString.Split('\n').Select(a => "\t" + a));
            rightString = String.Join("\n", rightString.Split('\n').Select(a => "\t" + a));

            return String.Format("\nData: {0}\n"
                                + "Left: {1}\n"
                                + "Right: {2}",
                                dataString, leftString, rightString);
        }

    }
}
