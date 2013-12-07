using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace SketchyGraph.GraphClasses
{
    public class SliceObject
    {
        private string dataName;
        private double percentage;
        private double angle;
        private Line line1;
        private Line line2;

        public SliceObject()
        {
            dataName = "";
            percentage = 0.0;
            angle = 0.0;
            line1 = new Line();
            line2 = new Line();
        }

        public SliceObject(Line line1, Line line2)
        {
            this.line1 = line1;
            this.line2 = line2;
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
            this.percentage = Math.Round(percentage, 2, MidpointRounding.AwayFromZero);
        }
        public double GetAngle()
        {
            return this.angle;
        }
        public void SetAngle(double angle)
        {
            this.angle = angle;
            SetPercentage(this.angle / 360.0);
        }
        #endregion
    }
}
