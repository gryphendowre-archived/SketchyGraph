using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SketchyGraph
{
    public class Samples
    {
        public string name = "";
    	public List<List<Unistroke>> samples = new List<List<Unistroke>>();

    	public Samples(string n) {
        	this.name = n;
    	}
    }
}
