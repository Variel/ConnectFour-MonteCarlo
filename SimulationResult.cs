using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonteCarloTestNet
{
    public class SimulationResult
    {
        public int Column { get; set; }
        public int Value { get; set; }

        public SimulationResult(int col, int val) => (Column, Value) = (col, val);
    }
}
