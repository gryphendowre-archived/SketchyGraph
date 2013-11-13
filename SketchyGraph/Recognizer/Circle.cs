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
    public class Circle
    {
        Point center;
        double radius;

    	public Circle(Point c, double r) {
        	this.center = c;
            this.radius = r;
    	}

        public Point Center
        {
            get
            {
                return this.center;
            }
            set
            {
                this.center = value;
            }
        }

        public double Radius {
            get {
                return this.radius;
            }
            set {
                this.radius = value;
            }
        }

    }
}
