using System;
using System.Collections.Generic;
using System.Text;

namespace MonteCarloTest
{
    public class HumanPlayer : IPlayer
    {
        public string Name { get; }
        public bool Identifier { get; }

        public HumanPlayer(string name, bool identifier) => (Name, Identifier) = (name, identifier);

        public int NextMove(Board currentBoard)
        {
            int input;
            do
            {
                input = Input();
            } while (!(1 <= input && input <= 7));

            return input - 1;
        }

        private int Input()
        {
            int input;
            do
            {
                Console.Write($"{Name}님, 다음 수를 입력하세요: ");
            } while (!Int32.TryParse(Console.ReadLine(), out input));

            return input;
        }
    }
}
