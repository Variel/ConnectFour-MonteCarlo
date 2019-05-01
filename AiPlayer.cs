using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonteCarloTestNet;

namespace MonteCarloTest
{
    class AiPlayer : IPlayer
    {
        private static readonly int MaxSimulation;

        public string Name { get; }
        public bool Identifier { get; }

        static AiPlayer()
        {
            MaxSimulation = Int32.Parse(ConfigurationManager.AppSettings["AiMaxIteration"]);
        }

        public AiPlayer(string name, bool identifier) => (Name, Identifier) = (name, identifier);

        public int NextMove(Board currentBoard)
        {
            List<SimulationResult> results = new List<SimulationResult>();

            for(var i = 0; i < 7; i++)
            {
                results.Add(SelectAndSimulate(currentBoard, i, MaxSimulation));
            }

            return results.OrderByDescending(r => r.Value).First().Column;
        }

        private SimulationResult SelectAndSimulate(Board currentBoard, int col, int limit)
        {
            if (!currentBoard.IsValidMove(col))
                return new SimulationResult(-1, 0);

            var sum = 0;
            for (var i = 0; i < limit; i++)
                sum += Simulate(currentBoard.MakeMove(col, Identifier));

            return new SimulationResult(col, sum);
        }

        private short Simulate(Board currentBoard)
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            BoardState currentState;

            // 이미 Select과정에서 내 수는 정해져 있으므로 상대방 차례
            var currentPlayer = Identifier ? 0 : 1;
            var currentIdentifier = !Identifier;

            do
            {
                int movement;
                bool[] check = new bool[7];
                do
                {
                    movement = rnd.Next(0, 7);
                    check[movement] = true;

                    if (check.All(c => c))
                        return 0;

                } while (!currentBoard.IsValidMove(movement));

                currentBoard = currentBoard.MakeMove(movement, currentIdentifier);

                currentPlayer = (currentPlayer + 1) % 2;
                currentIdentifier = currentPlayer == 1;
            } while (!(currentState = currentBoard.DetermineState()).IsOver);

            if (currentState.WinnerIdentifier == Identifier)
                return 2;

            if (currentState.WinnerIdentifier == null)
                return 0;

            return -1;
        }
    }
}
