using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MonteCarloTestNet;

namespace MonteCarloTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            //IPlayer player1, player2;

            //int input;
            //do
            //{
            //    do
            //    {
            //        Console.Write("모드를 선택하세요 (1: 사람 대전, 2: AI 대전, 3: AI vs 사람): ");
            //    } while (!Int32.TryParse(Console.ReadLine(), out input));
            //} while (!(1 <= input && input <= 3));

            //switch (input)
            //{
            //    case 1:
            //        Console.Write("플레이어1 이름을 입력하세요: ");
            //        var p1Name = Console.ReadLine();

            //        Console.WriteLine("플레이어2 이름을 입력하세요: ");
            //        var p2Name = Console.ReadLine();

            //        player1 = new HumanPlayer(p1Name, false);
            //        player2 = new HumanPlayer(p2Name, true);
            //        break;
            //    case 2:
            //    default:
            //        player1 = new AiPlayer("삼성", false);
            //        player2 = new AiPlayer("애플", true);
            //        break;
            //    case 3:
            //        player1 = new AiPlayer("인공지능", false);

            //        Console.WriteLine("플레이어2 이름을 입력하세요: ");
            //        p2Name = Console.ReadLine();

            //        player2 = new HumanPlayer(p2Name, true);
            //        break;
            //}


            const int iterations = 100;

            int baseWinCount = 0;
            int edgeWinCount = 0;

            AiPlayer basePlayer, edgePlayer;

            var sw = new StreamWriter("./out.txt") { AutoFlush = true };

            ThreadPool.GetMaxThreads(out var workers, out var completions);
            
            Console.WriteLine($"AI Simulation Iterations: {ConfigurationManager.AppSettings["AiMaxIteration"]}\n" +
                              $"Iterations per Fitting Step: {iterations}");
            sw.WriteLine($"AI Simulation Iterations: {ConfigurationManager.AppSettings["AiMaxIteration"]}\n" +
                              $"Iterations per Fitting Step: {iterations}");

            basePlayer = new AiPlayer("베이스", false);
            edgePlayer = new AiPlayer("엣지", true, (w, l) => w / (l + 0.0000001));

            baseWinCount = 0;
            edgeWinCount = 0;

            Console.WriteLine("Base Func: {win * 2 - lose}");
            Console.WriteLine("Edge Func: {win / (lose + 0.0000001)}");
            Console.WriteLine();
            sw.WriteLine("Base Func: {win * 2 - lose}");
            sw.WriteLine("Edge Func: {win / (lose + 0.0000001)}");
            sw.WriteLine();

            var watch = new Stopwatch();
            watch.Start();

            var tasks = new List<Task<IPlayer>>();

            for (var i = 0; i < iterations; i++)
            {
                var iteration = i;
                tasks.Add(Task.Run(() =>
                {
                    var innerWatch = new Stopwatch();
                    innerWatch.Start();
                    var game = new Game(basePlayer, edgePlayer);
                    var winner = game.StartLoop();
                    Console.WriteLine($"Iteration No.{iteration+1:D2} {winner?.Name ?? "Nobody"} Wins, {innerWatch.Elapsed.TotalSeconds:N1}s Elapsed");
                    sw.WriteLine($"Iteration No.{iteration+1:D2} {winner?.Name ?? "Nobody"} Wins, {innerWatch.Elapsed.TotalSeconds:N1}s Elapsed");

                    return winner;
                }));
            }

            Task.WaitAll(tasks.ToArray());

            foreach(var winner in tasks.Select(t => t.Result))
            {
                if (winner == basePlayer)
                    baseWinCount++;
                else if (winner == edgePlayer)
                    edgeWinCount++;
            }


            Console.WriteLine($"Fitting Step Finished! Elapsed Time: {watch.ElapsedMilliseconds/1000.0:N2}s");
            Console.WriteLine($"Base Wins {baseWinCount} Times | Edge Wins {edgeWinCount} Times | Tied {iterations - baseWinCount - edgeWinCount}");
            Console.WriteLine();
            sw.WriteLine();
            sw.WriteLine($"Fitting Step Finished! Elapsed Time: {watch.ElapsedMilliseconds / 1000.0:N2}s");
            sw.WriteLine($"Base Wins {baseWinCount} Times | Edge Wins {edgeWinCount} Times | Tied {iterations - baseWinCount - edgeWinCount}");
            sw.WriteLine();

            sw.Close();
        }
    }
}
