using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SketchyGraph
{
    public class Element
    {
        public Value domain_val { get; set; }
        public Value range_val { get; set; }
        public Unistroke plot { get; set; }

        public Element() {}

    }
}
