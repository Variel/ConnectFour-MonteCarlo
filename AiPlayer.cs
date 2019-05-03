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

        public Func<int, int, double> HeuristicFunc { get; } = (win, lose) => win * 2 - lose;

        static AiPlayer()
        {
            MaxSimulation = Int32.Parse(ConfigurationManager.AppSettings["AiMaxIteration"]);
        }

        public AiPlayer(string name, bool identifier) => (Name, Identifier) = (name, identifier);

        public AiPlayer(string name, bool identifier, Func<int, int, double> heuristic) : this(name, identifier) =>
            HeuristicFunc = heuristic;

        public int NextMove(Board currentBoard)
        {
            //var sw = Stopwatch.StartNew();
            //Console.WriteLine($"{Name} is Now Thinking ...");
            var tasks = Enumerable.Range(0, 7).Select(col => SelectAndSimulateAsync(currentBoard, col, MaxSimulation)).ToArray();
            //var tasks = Enumerable.Range(0, 7).Select(col => (column: col, value: SelectAndSimulate(currentBoard, col, MaxSimulation))).ToArray();
            Task.WaitAll(tasks);
            //Console.WriteLine($"AI Thinking Time: {sw.ElapsedMilliseconds}ms");

            return tasks.Where(t => t.Result.Column != -1).OrderByDescending(t => t.Result.Value).First().Result.Column;
        }

        private Task<SimulationResult> SelectAndSimulateAsync(Board currentBoard, int col, int limit)
        {
            return new TaskFactory<SimulationResult>().StartNew(() =>
            {
                if (!currentBoard.IsValidMove(col))
                    return new SimulationResult(-1, 0);

                int wins = 0, loses = 0;
                for (var i = 0; i < limit; i++)
                {
                    switch(Simulate(currentBoard.MakeMove(col, Identifier)))
                    {
                        case true:
                            wins++;
                            break;
                        case false:
                            loses++;
                            break;
                    }
                }

                return new SimulationResult(col, HeuristicFunc(wins, loses));
            });
        }

        private bool? Simulate(Board currentBoard)
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
                        return null;

                } while (!currentBoard.IsValidMove(movement));

                currentBoard = currentBoard.MakeMove(movement, currentIdentifier);

                currentPlayer = (currentPlayer + 1) % 2;
                currentIdentifier = currentPlayer == 1;
            } while (!(currentState = currentBoard.DetermineState()).IsOver);

            if (currentState.WinnerIdentifier == Identifier)
                return true;

            if (currentState.WinnerIdentifier == null)
                return null;

            return false;
        }
    }
}
