using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MonteCarloTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            IPlayer player1, player2;

            int input;
            do
            {
                do
                {
                    Console.Write("모드를 선택하세요 (1: 사람 대전, 2: AI 대전, 3: AI vs 사람): ");
                } while (!Int32.TryParse(Console.ReadLine(), out input));
            } while (!(1 <= input && input <= 3));

            switch (input)
            {
                case 1:
                    Console.Write("플레이어1 이름을 입력하세요: ");
                    var p1Name = Console.ReadLine();

                    Console.WriteLine("플레이어2 이름을 입력하세요: ");
                    var p2Name = Console.ReadLine();

                    player1 = new HumanPlayer(p1Name, false);
                    player2 = new HumanPlayer(p2Name, true);
                    break;
                case 2:
                default:
                    player1 = new AiPlayer("삼성", false);
                    player2 = new AiPlayer("애플", true);
                    break;
                case 3:
                    player1 = new AiPlayer("인공지능", false);

                    Console.WriteLine("플레이어2 이름을 입력하세요: ");
                    p2Name = Console.ReadLine();

                    player2 = new HumanPlayer(p2Name, true);
                    break;
            }

            var game = new Game(player1, player2);
            game.StartLoop();
        }
    }
}
