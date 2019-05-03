using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonteCarloTestNet
{
    public class SimulationResult
    {
        public int Column { get; set; }
        public double Value { get; set; }

        public SimulationResult(int col, double val) => (Column, Value) = (col, val);
    }
}
