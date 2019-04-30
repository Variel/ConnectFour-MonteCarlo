using System;
using System.Collections.Generic;
using System.Text;

namespace MonteCarloTest
{
    public interface IPlayer
    {
        string Name { get; }
        bool Identifier { get; }
        int NextMove(Board currentBoard);
    }
}
