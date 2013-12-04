using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Ink;
using SketchyGraph.Util;

namespace SketchyGraph.GraphClasses.Charts
{
    public class PieChart : BaseGraph
    {

        protected Stroke circumference;
        protected List<Stroke> slices;
        protected List<Point> circumPoints;
        protected Rect boundingBox;
        protected Point centerPoint;
        protected Circle area;
        

        public PieChart(Stroke circumference)
        {
            this.circumference = circumference;
            this.slices = new List<Stroke>();
            this.boundingBox = GetBoundingBox();
            this.centerPoint = FindCenter(this.boundingBox);
            this.area = CalculateCircleArea(this.boundingBox, this.centerPoint);
            double x = 0;
        }

        public void addSlices(List<Element> slices)
        {
            foreach (Element obj in slices)
            {
                this.addElement(obj);
            }
        }

        public void addSlices(Stroke slice)
        {
            slices.Add(slice);
        }

        public Rect GetBoundingBox()
        {
            Rect bounds = circumference.GetBounds();
            return bounds;
        }

        public Point FindCenter(Rect box)
        {
            Point temp = new Point();
            temp.X = box.TopLeft.X + (box.Width / 2.0);
            temp.Y = box.TopLeft.Y + (box.Height / 2.0);
            return temp;
        }

        public Circle CalculateCircleArea(Rect boundingBox, Point center)
        {
            Point top = new Point((boundingBox.TopLeft.X + (boundingBox.Width / 2.0)), boundingBox.TopLeft.Y);
            Point right = new Point(boundingBox.TopRight.X, (boundingBox.TopRight.Y + (boundingBox.Height / 2.0)));
            Point bottom = new Point((boundingBox.TopLeft.X + (boundingBox.Width / 2.0)), boundingBox.BottomLeft.Y);
            Point left = new Point(boundingBox.TopLeft.X, (boundingBox.TopLeft.Y + (boundingBox.Height / 2.0)));
            
            double r1 = GeneralUtil.EuclideanDistance(top, center);
            double r2 = GeneralUtil.EuclideanDistance(right, center);
            double r3 = GeneralUtil.EuclideanDistance(bottom, center);
            double r4 = GeneralUtil.EuclideanDistance(left, center);
            double rTot = (r1 + r2 + r3 + r4) / 4.0;

            double rNew = rTot * 1.1;

            Circle temp = new Circle(center, rNew);
            return temp;
        }

        public override void CalculateBoundingBoxes(double threshold)
        {
        }

        public Circle GetCircleArea()
        {
            return this.area;
        }
        public List<Stroke> GetSlices()
        {
            return this.slices;
        }
        public Stroke GetCircumference()
        {
            return this.circumference;
        }
        public Point GetCenterPoint()
        {
            return this.centerPoint;
        }
    }
}
