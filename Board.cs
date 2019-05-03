using System;
using System.Collections.Generic;
using System.Text;

namespace MonteCarloTest
{
    public class Board
    {
        private readonly bool?[,] _boardData = new bool?[6, 7]; // 게임판 상태

        private readonly short[] dRow = { 0, 1, 1, 1 }; // 행 방향
        private readonly short[] dCol = { 1, 1, 0, -1 }; // 열 방향

        private bool isFirstMovement = true; // 첫 수에서 4번 열에는 돌을 못 놓도록 하기 위해 첫 수인지 여부를 체크하는 변수

        public Board() { } // 게임판을 그냥 생성했을 경우. 게임판 상태는 모두 비어있고 첫 수라고 체크 됨.

        private Board(Board parent) // 부모 상태로부터 게임판을 생성할 경우.
        {
            isFirstMovement = parent.isFirstMovement;
            for (var i = 0; i < 6; i++)
                for (var j = 0; j < 7; j++)
                    _boardData[i, j] = parent._boardData[i, j]; // 부모 상태 복사
        }

        public Board MakeMove(int col, bool value) // 현재 게임판 상태에 다음 수를 반영한 상태를 가지는 새로운 게임판을 반환
        {
            var child = new Board(this) {isFirstMovement = false}; // 현재 게임판 상태를 부모 상태로 한 자식 게임판 상태 생성. 다음 수를 반영 했으므로 당연히 첫 수가 아님.
            var isValid = false; // for 문을 모두 통과했는데도 false이면 둘 수 없는 곳이라는 소리
            for(var i = 5; i >= 0; i --) // 해당 열에 대해 맨 아래 행부터 맨 위 행까지 돌면서 빈 자리가 있는지 체크
            {
                if (child._boardData[i,col] == null) // 빈 자리가 있을 경우
                {
                    child._boardData[i, col] = value; // 자식 게임판 상태의 i 행 col 열의 값을 value(식별자)로 설정
                    isValid = true; // 빈 자리가 있으므로 둘 수 있는 곳

                    break; // 빈 자리는 하나만 있으면 되므로 이하 과정은 break
                }
            }

            if (!isValid)
                throw new InvalidOperationException("둘 수 없는 곳입니다");

            return child; // 자식 게임 상태를 기반으로 만든 게임판 반환
        }

        public bool IsValidMove(int col)
        {
            if (isFirstMovement && col == 3) // 첫 수인데 4번 열(인덱스는 0부터 시작이므로 3번 인덱스)에 두려고 하면 유효하지 않은 수
                return false;

            var isValid = false;

            for (var i = 5; i >= 0; i--) // 사실 30 ~ 39 줄이랑 거의 다른 게 없다. 단지 valid 여부만 체크 할 뿐
            {
                if (_boardData[i, col] == null)
                {
                    isValid = true; break;
                }
            }

            return isValid;
        }
        
        public void Print() // 게임판 상태 출력. 자세한 설명은 생략한다
        {
            Console.WriteLine("  +--+--+--+--+--+--+--+");
            for (int i = 0; i < 6; i ++)
            {
                Console.Write($"{6 - i} |");

                for (int j = 0 ; j < 7; j ++)
                {
                    Console.Write(_boardData[i, j].HasValue ? _boardData[i, j] == true ? "○" : "●" : "  ");
                    Console.Write("|");
                }

                Console.WriteLine("\n  +--+--+--+--+--+--+--+");
            }
            Console.WriteLine("    1  2  3  4  5  6  7");
        }

        public BoardState DetermineState() // 현재 상태에 대해 게임 종료 여부와 승자의 식별자를 반환한다
        {
            var anySpace = false;

            for (var i = 0; i < 6; i++)
            for (var j = 0; j < 7; j++)
            for (var k = 0; k < 4; k++) // 우, 우하, 하, 좌하 네 방향에 대해 Four In A Row 여부를 확인한다
                if (_boardData[i,j].HasValue && IsFourInARow(i, j, k))
                    return new BoardState(true, _boardData[i, j].Value); // 하나라도 Four In A Row가 있으면 종료 됐다는 상태와 누가 이겼는지 반환

            for(var i = 0; i < 7; i ++)
                anySpace |= IsValidMove(i); // 0 ~ 6 열 중에 하나라도 유효한 수가 존재하는지 확인
            // 유효한 수가 존재하지 않는다면 비긴 것으로 반환, 존재한다면 아직 안 끝난 것으로 반환
            return !anySpace ? new BoardState(true, null) : new BoardState(false, null);
        }

        private bool IsFourInARow(int row, int col, int direction, int depth = 1)
        { // 지정한 방향(direction 인자)으로 4개의 같은 돌이 한 줄에 있는지 확인
            if (depth == 4) // 현재까지 전진 해 온 깊이가 4단계라면 4개의 같은 돌이 한 줄에 있는 것이다
                return true;

            var val = _boardData[row, col];

            if (!val.HasValue)
                return false;
            // 지정한 방향으로 한 단계 전진
            var rowTo = row + dRow[direction];
            var colTo = col + dCol[direction];

            if (rowTo < 0 || rowTo >= 6 || colTo < 0 || colTo >= 7) // 인덱스를 벗어나지 않는지 확인
                return false; // 벗어나면 중단

            if (val.Value == _boardData[rowTo, colTo]) // 현재 위치와 전진 하는 위치의 돌 색이 같다면 전진 하는 위치에서 한 단계 깊이를 증가 시키고 Four In A Row 확인 (현재 depth가 1일 경우 앞으로 Three In A Row를 확인하는 것. 현재 depth가 2일 경우 앞으로 Two In A Row만 확인 하면 됨)
                return IsFourInARow(rowTo, colTo, direction, depth + 1);

            return false;
        }
    }
}
