using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using SketchyGraph.Util;

namespace SketchyGraph.GraphClasses
{
    public class SliceObject
    {
        private string dataName;
        private double percentage;
        private double angle;
        private Line line1;
        private Line line2;
        private Path polyPath;
        private List<Line> highlightedLines;

        public SliceObject()
        {
            dataName = "";
            percentage = 0.0;
            angle = 0.0;
            line1 = new Line();
            line2 = new Line();
            polyPath = new Path();
        }

        public SliceObject(Line line1, Line line2)
        {
            this.line1 = line1;
            this.line2 = line2;
            polyPath = new Path();
            dataName = "";
            percentage = 0.0;
            angle = 0.0;
            highlightedLines = new List<Line>();
        }

        public void CreatePath(Circle circle, Point rightPoint)
        {
            Vector vectorOriginRight = new Vector(circle.Center.X - rightPoint.X, circle.Center.Y - rightPoint.Y);
            Vector vectorRight = new Vector(circle.Center.X - line1.X2, circle.Center.Y - line1.Y2);
            Vector vectorLeft = new Vector(circle.Center.X - line2.X2, circle.Center.Y - line2.Y2);

            double angleRightOffset = (-1.0) * Vector.AngleBetween(vectorOriginRight, vectorRight);
            double angleLeftOffset = (-1.0) * Vector.AngleBetween(vectorOriginRight, vectorLeft);

            this.highlightedLines.Clear();
            double angleAmt = this.angle;
            int angleInt = (int)angleAmt;
            int angleRight = 0;
            int angleLeft = 0;
            if (angleInt % 2 == 0)
            {
                angleRight = angleInt / 2;
                angleLeft = angleInt / 2;
            }
            else
            {
                angleRight = angleInt / 2;
                angleLeft = (angleInt / 2) + 1;
            }


            for (int i = 0; i < angleRight; i++)
            {

                Line newColor = new Line();

                newColor.X1 = line2.X1;
                newColor.X2 = circle.Center.X + ((circle.Radius * 0.97) * Math.Cos((-(i + 1 + angleRightOffset)) * Math.PI / 180.0));
                newColor.Y1 = line2.Y1;
                newColor.Y2 = circle.Center.Y + ((circle.Radius * 0.97) * Math.Sin((-(i + 1 + angleRightOffset)) * Math.PI / 180.0));
                newColor.Stroke = Brushes.Yellow;
                newColor.StrokeThickness = 2;

                this.highlightedLines.Add(newColor);
            }
            for (int i = 0; i < angleLeft; i++)
            {
                Line newColor = new Line();

                newColor.X1 = line2.X1;
                newColor.X2 = circle.Center.X + ((circle.Radius * 0.97) * Math.Cos((i + 1 - angleLeftOffset) * Math.PI / 180.0));
                newColor.Y1 = line2.Y1;
                newColor.Y2 = circle.Center.Y + ((circle.Radius * 0.97) * Math.Sin((i + 1 - angleLeftOffset) * Math.PI / 180.0));
                newColor.Stroke = Brushes.Yellow;
                newColor.StrokeThickness = 2;

                this.highlightedLines.Add(newColor);
            }
        }

        #region Getter/Setters

        public string GetDataName()
        {
            return this.dataName;
        }
        public void SetDataName(string dataName)
        {
            this.dataName = dataName;
        }
        public double GetPercentage()
        {
            return this.percentage;
        }
        public void SetPercentage(double percentage)
        {
            this.percentage = Math.Round((percentage*100.0), 2, MidpointRounding.AwayFromZero);
        }
        public double GetAngle()
        {
            return this.angle;
        }
        public void SetAngle(double angle)
        {
            if (angle < 0)
            {
                angle = angle + 360;
            }
            this.angle = angle;

            SetPercentage(this.angle / 360.0);
        }
        public Line GetLine1()
        {
            return this.line1;
        }
        public Line GetLine2()
        {
            return this.line2;
        }
        public List<Line> GetHighLightedLines()
        {
            return this.highlightedLines;
        }
        #endregion
    }
}
