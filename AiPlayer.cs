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
        private static readonly int MaxSimulation; // 경우의 수 별로 시뮬레이션을 몇 번 돌릴 것인지 결정하는 변수 21 줄에서 설정 됨

        public string Name { get; }
        public bool Identifier { get; }

        static AiPlayer()
        {
            MaxSimulation = Int32.Parse(ConfigurationManager.AppSettings["AiMaxIteration"]); // 경우의 수 별로 시뮬레이션을 몇 번 돌릴 것인지 결정하는 변수를 설정 파일 (app.config) 에서 불러옴
        }

        public AiPlayer(string name, bool identifier) => (Name, Identifier) = (name, identifier); // 플레이어의 이름과 식별자를 설정하는 부분

        public int NextMove(Board currentBoard)
        {
            List<SimulationResult> results = new List<SimulationResult>(); // 각 경우의 수에 따른 휴리스틱 값을 담기 위한 리스트

            for(var i = 0; i < 7; i++)
            {
                results.Add(SelectAndSimulate(currentBoard, i, MaxSimulation)); // i 열을 선택하는 경우의 수를 선택한 결과에 대한 휴리스틱 값을 담는다
            }

            return results.OrderByDescending(r => r.Value).First().Column; // 결과 리스트를 리스트 요소의 Value 항목에 대한 내림차순으로 정렬하여 가장 첫번째 요소의 Column 항목을 반환한다. 즉, 가장 높은 휴리스틱 값을 가지는 경우의 수는 어떤 열을 선택한 것인지 반환
        }

        private SimulationResult SelectAndSimulate(Board currentBoard, int col, int limit)
        {
            if (!currentBoard.IsValidMove(col)) // 선택한 경우의 수가 둘 수 없는 수인 경우
                return new SimulationResult(col, Int32.MinValue); // 이 경우의 수가 선택 되지 않도록 가장 작은 수 (음수 최댓값) 반환

            var sum = 0;
            for (var i = 0; i < limit; i++) // 설정에 따른 시뮬레이션 횟수 만큼 반복
                sum += Simulate(currentBoard.MakeMove(col, Identifier)); // 시뮬레이션을 돌리고 그 결과를 더한다. 이 때 시뮬레이션 시작 상태는 AI가 이미 col 열에 수를 둔 상태를 시작 상태로 한다.

            return new SimulationResult(col, sum);
        }

        private short Simulate(Board currentBoard)
        {
            Random rnd = new Random((int)DateTime.Now.Ticks); // 랜덤 초기화
            BoardState currentState;

            // 이미 Select과정에서 내 수는 정해져 있으므로 상대방 차례
            var currentPlayer = Identifier ? 0 : 1;
            var currentIdentifier = !Identifier;

            do // ~ 79 줄 까지 Game.cs:27 ~ 41 줄과 거의 동일하다. 즉, 시뮬레이션이란 미니 게임을 랜덤으로, 정말 아무렇게나 돌리는 것이다
            {
                int move;
                bool[] check = new bool[7];
                do
                { // 유효한 수가 나올 때 까지 다음 수를 랜덤으로 결정한다. 이 때 0 ~ 6 까지 모든 수가 유효하지 않을 경우 이 시뮬레이션은 비긴 것으로 끝낸다. 그걸 체크하기 위해 윗줄에서 0 ~ 6까지의 체크 배열을 만들었다.
                    if (check.All(c => c)) // 배열의 모든 요소가 참일 경우
                        return 0; // 모든 수가 유효하지 않다는 얘기므로 비긴 것을 뜻하는 0을 반환.

                    move = rnd.Next(0, 7); // 랜덤으로 다음 수 결정
                    check[move] = true; // 체크 배열에 체크 해 둠

                } while (!currentBoard.IsValidMove(move)); // 랜덤하게 결정한 다음 수가 유효한 수인지 확인, 유효하지 않을 경우 다시 다음 수를 랜덤하게 결정

                currentBoard = currentBoard.MakeMove(move, currentIdentifier); // 결정한 다음 수를 시뮬레이션 상의 게임판에 반영

                currentPlayer = (currentPlayer + 1) % 2; // 플레이어 전환
                currentIdentifier = currentPlayer == 1; // 플레이어 전환

                currentState = currentBoard.DetermineState(); // 시뮬레이션 게임의 현재 상태 확인
            } while (!currentState.IsOver); // 시뮬레이션 게임이 끝난 경우 루프 빠져나옴

            if (currentState.WinnerIdentifier == Identifier) // 시뮬레이션 게임의 승자가 AiPlayer 개체 본인일 경우
                return 2; // 승점 2점 반환

            if (currentState.WinnerIdentifier == null) // 시뮬레이션 게임의 승자가 없을 경우
                return 0; // 승점 0점 반환

            return -1; // 졌을 경우 승점 -1점 반환
        }
    }
}
