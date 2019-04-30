using System;
using System.Collections.Generic;
using System.Text;

namespace MonteCarloTest
{
    public class BoardState
    {
        public bool IsOver { get; }
        public bool? WinnerIdentifier { get; }

        public BoardState(bool isOver, bool? winnerIdentifier) => (IsOver, WinnerIdentifier) = (isOver, winnerIdentifier);
    }
}
