using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SketchyGraph.GraphClasses
{
    class SliceObject
    {
        private string dataName;
        private double percentage;
        private double angle;

        public SliceObject()
        {
            dataName = "";
            percentage = 0.0;
            angle = 0.0;
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
            this.percentage = percentage;
        }
        public double GetAngle()
        {
            return this.angle;
        }
        public void SetAngle(double angle)
        {
            this.angle = angle;
        }
        #endregion
    }
}
