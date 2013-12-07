using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Ink;
using SketchyGraph.Util;
using System.Windows.Shapes;
using System.Diagnostics;

namespace SketchyGraph.GraphClasses.Charts
{
    public class PieChart : BaseGraph
    {
        #region Elements 
        protected Stroke circumference;
        protected List<Stroke> slices;
        protected List<Line> sliceLines;
        protected List<SliceObject> sliceObjects;
        protected List<Point> circumPoints;
        protected Rect boundingBox;
        protected Point centerPoint;
        protected Circle area;
        protected Point rightSideMed;
        #endregion

        #region Constructor
        public PieChart(Stroke circumference)
        {
            this.circumference = circumference;
            this.slices = new List<Stroke>();
            this.sliceLines = new List<Line>();
            this.sliceObjects = new List<SliceObject>();
            this.boundingBox = GetBoundingBox();
            this.centerPoint = FindCenter(this.boundingBox);
            this.area = CalculateCircleArea(this.boundingBox, this.centerPoint);
            this.rightSideMed = new Point();
            this.rightSideMed = CalculateRightPoint();
            double x = 0;
        }
        #endregion

        #region Public Methods
        public Point CalculateRightPoint()
        {
            Point temp = new Point();
            temp.X = this.area.Center.X + this.area.Radius;
            temp.Y = this.area.Center.Y;
            return temp;
        }

        public void addSlices(Line slice)
        {
            sliceLines.Add(slice);
            if (sliceLines.Count > 1)
            {
                this.sliceLines = PieSort();
                CreateAndReorderSliceObjects();
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
        #endregion

        #region Private Methods
        private List<Line> PieSort()
        {
            List<Line> topHalf = new List<Line>();
            List<Line> bottomHalf = new List<Line>();
            List<Line> resortedList = new List<Line>();
            Dictionary<double, Line> topHalfDict = new Dictionary<double, Line>();
            Dictionary<double, Line> bottomHalfDict = new Dictionary<double, Line>();
            Point temp;

            foreach (Line line in sliceLines)
            {
                if (line.Y2 <= rightSideMed.Y)
                {
                    topHalf.Add(line);
                }
                else
                {
                    bottomHalf.Add(line);
                }

            }

            sliceLines.Clear();

            if (topHalf.Count > 0)
            {
                double topHalfDist = 0;
                for (int i = 0; i < topHalf.Count; i++)
                {
                    temp = new Point(topHalf[i].X2, topHalf[i].Y2);
                    topHalfDist = GeneralUtil.EuclideanDistance(temp, rightSideMed);
                    topHalfDict.Add(topHalfDist, topHalf[i]);
                }
                List<double> tempList = topHalfDict.Keys.ToList();
                tempList.Sort();

                for (int i = 0; i < tempList.Count; i++)
                {
                    resortedList.Add(topHalfDict[tempList[i]]);
                }

            }
            if (bottomHalf.Count > 0)
            {
                double bottomHalfDist;
                for (int i = 0; i < bottomHalf.Count; i++)
                {
                    temp = new Point(bottomHalf[i].X2, bottomHalf[i].Y2);
                    bottomHalfDist = GeneralUtil.EuclideanDistance(temp, rightSideMed);
                    bottomHalfDict.Add(bottomHalfDist, bottomHalf[i]);
                }
                List<double> tempList = bottomHalfDict.Keys.ToList();
                tempList.Sort();
                tempList.Reverse();

                for (int i = 0; i < tempList.Count; i++)
                {
                    resortedList.Add(bottomHalfDict[tempList[i]]);
                }
            }

            return resortedList;
            //rightSideMed;
        }

        private void CreateAndReorderSliceObjects()
        {
            sliceObjects.Clear();
            for (int j = 0; j < sliceLines.Count-1; j++)
            {

                SliceObject temp = new SliceObject(sliceLines[j], sliceLines[j + 1]);
                sliceObjects.Add(temp);

            }
            SliceObject turnaround = new SliceObject(sliceLines[sliceLines.Count - 1], sliceLines[0]);
            sliceObjects.Add(turnaround);
            Debug.WriteLine("Number of SliceObjects: " + sliceObjects.Count);

            //Create the Percentages, call a method to calculate vectors?
            foreach (SliceObject sObj in sliceObjects)
            {
                sObj.SetAngle(CalculateDegrees());
            }
        }
        private double CalculateDegrees()
        {
            return 0;
        }
        #endregion
    }
}
