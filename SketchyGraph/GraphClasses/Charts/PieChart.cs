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
        public List<Circle> grabbingCircles;
        protected List<SliceObject> sliceObjects;
        protected List<Point> circumPoints;
        protected Rect boundingBox;
        protected Point centerPoint;
        protected Circle area;
        protected Point rightSideMed;
        protected bool modified;
        public List<SliceObject> holdingList;
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
            this.modified = false;
            this.holdingList = new List<SliceObject>();
            this.grabbingCircles = new List<Circle>();
        }
        #endregion

        #region Public Methods
        //Method that calculates the right point, for Sorting Slice Lines later, using the concept of the Right Quadrant.
        public Point CalculateRightPoint()
        {
            Point temp = new Point();
            temp.X = this.area.Center.X + this.area.Radius;
            temp.Y = this.area.Center.Y;
            return temp;
        }

        //Add a Slice as a Line, main Slice method added.
        public void addSlices(Line slice)
        {
            sliceLines.Add(slice);
            
            if (sliceLines.Count > 1)
            {
                grabbingCircles.Clear();
                this.sliceLines = PieSort();
                foreach (Line slc in this.sliceLines)
                {
                    Circle tempC = new Circle(new Point(slc.X2, slc.Y2), 20);
                    grabbingCircles.Add(tempC);
                }
                holdingList = new List<SliceObject>(CreateAndReorderSliceObjects());
                this.modified = true;

            }
        }

        public List<SliceObject> UpdateLines()
        {
            grabbingCircles.Clear();
            this.sliceLines = PieSort();
            foreach(Line slc in this.sliceLines)
            {
                Circle tempC = new Circle(new Point(slc.X2, slc.Y2), 10);
                grabbingCircles.Add(tempC);
            }
            holdingList = new List<SliceObject>(CreateAndReorderSliceObjects());
            this.modified = true;
            return holdingList;
        }

        //Keeps the actual stroke drawn stored.
        public void addSlices(Stroke slice)
        {
            slices.Add(slice);
        }

        //Finds the bounding box of the Circle.
        public Rect GetBoundingBox()
        {
            Rect bounds = circumference.GetBounds();
            return bounds;
        }

        //Find the Center of the Circle
        public Point FindCenter(Rect box)
        {
            Point temp = new Point();
            temp.X = box.TopLeft.X + (box.Width / 2.0);
            temp.Y = box.TopLeft.Y + (box.Height / 2.0);
            return temp;
        }

        //Calculates the Circle area from the bounding box, to set up the Circle Object.
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

        //Override method necessary due to extension.
        public override void CalculateBoundingBoxes(double threshold)
        {
        }

        #endregion

        #region Private Methods
        /*
         * Using a clever Sorting method, it takes a pie chart and (using the right side point initialized earlier), begins
         * sorting each line counterclockwise from the position of the right-side point. This is done using the Euclidean
         * Distance between the right-side point and the end-point of each line (Not the center point where they intersect).
         * To solve the issue of two lines having the same distance (hypothetically, due to calculating the distance of lines
         * on the lower side), is to separate the pie chart by dividing it in half and separating lists into TopHalf and
         * BottomHalf, sorting the lines by their distances, then adding the bottomhalf list to the tophalf into a 
         * total sorted list. This results in a completely sorted list of Lines.
         */
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
        }

        /*
         * This method creates and reorders sliceObjects whenever a new object is created, which occurs
         * whenever a new slice line is added to the Pie Chart.
         */
        private List<SliceObject> CreateAndReorderSliceObjects()
        {
            List<SliceObject> holder = new List<SliceObject>(sliceObjects);

            sliceObjects.Clear();
            int count = 0;
            if (holder.Count > 1)
            {
                for (int j = 0; j < sliceLines.Count - 1; j++)
                {
                    foreach (SliceObject slc in holder)
                    {
                        if (slc.GetLine1().Equals(sliceLines[j]) && slc.GetLine2().Equals(sliceLines[j + 1]))
                        {
                            sliceObjects.Add(slc);
                            slc.tagged = true;
                            count = 1;
                            break;
                        }
                    }
                    if (count == 0)
                    {
                         SliceObject temp = new SliceObject(sliceLines[j], sliceLines[j + 1]);
                         sliceObjects.Add(temp);
                    }
                    count = 0;
                }

                foreach (SliceObject slc in holder)
                {
                    if (slc.GetLine1().Equals(sliceLines[sliceLines.Count - 1]) && slc.GetLine2().Equals(sliceLines[0]))
                    {
                        sliceObjects.Add(slc);
                        slc.tagged = true;
                        count = 1;
                        break;
                    }
                }
                if (count == 0)
                {
                    SliceObject temp = new SliceObject(sliceLines[sliceLines.Count - 1], sliceLines[0]);
                    sliceObjects.Add(temp);
                }

            }
            else
            {
                for (int j = 0; j < sliceLines.Count - 1; j++)
                {
                    SliceObject temp = new SliceObject(sliceLines[j], sliceLines[j + 1]);
                    sliceObjects.Add(temp);
                    if (j == sliceLines.Count - 2)
                    {
                        SliceObject temp2 = new SliceObject(sliceLines[sliceLines.Count - 1], sliceLines[0]);
                        sliceObjects.Add(temp2);
                    }
                }
            }

            Debug.WriteLine("Number of SliceObjects: " + sliceObjects.Count);

            //Create the Percentages, call a method to calculate vectors?
            foreach (SliceObject sObj in sliceObjects)
            {
                //sObj.tagged = false;
                if (sObj.newSlice == true || sObj.manipulated == true)
                {
                    sObj.SetAngle(CalculateDegrees(sObj));
                }
                
            }
            foreach (SliceObject sObj in sliceObjects)
            {
                if (sObj.newSlice == true || sObj.manipulated == true)
                {
                    sObj.CreatePath(area, this.rightSideMed);
                    sObj.justUpdated = true;
                }
            }
            return holder;
        }
        /*
         * Finds the vector between two lines of an object and sets the Angle of the SliceObject.
         * Once this angle is calculated, it automatically updates the Percentage each Object takes up.
         */ 
        private double CalculateDegrees(SliceObject obj)
        {
            Line temp1 = obj.GetLine1();
            Line temp2 = obj.GetLine2();
            Vector v1 = new Vector(temp1.X1 - temp1.X2, temp1.Y1 - temp1.Y2);
            Vector v2 = new Vector(temp2.X1 - temp2.X2, temp2.Y1 - temp2.Y2);

            //Using Absolute values actually screws up the angle calculation for some objects. Setting this will
            //allow for checking of bigger angles than 180 later.
            return (-1.0)*Vector.AngleBetween(v1, v2);
        }
        #endregion

        #region Getters/Setters
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
        public List<SliceObject> GetSliceObjects()
        {
            return this.sliceObjects;
        }
        public bool GetModifiedStatus()
        {
            return this.modified;
        }
        public void SetModifiedStatus(bool modified)
        {
            this.modified = modified;
        }
        public List<Line> GetSliceLines()
        {
            return this.sliceLines;
        }
        #endregion
    }
}
