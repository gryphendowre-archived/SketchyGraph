using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SketchyGraph
{
    class BaseGraph
    {
        public int maxRange;
        public int minRange = 0;
        public int maxDomain;
        public int minDomain = 0;

        public List<Object> elements = null;

        public void addElement(Object element)
        {
            elements.Add(element);
        }
    }
}
