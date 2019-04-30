using System;
using System.Collections.Generic;
using System.Text;

namespace MonteCarloTest
{
    public class Board
    {
        private readonly bool?[,] _boardData = new bool?[6, 7];

        private readonly short[] dRow = { 0, 1, 1, 1 };
        private readonly short[] dCol = { 1, 1, 0, -1 };

        private bool isFirstMovement = true;

        public Board() { }

        private Board(Board parent)
        {
            isFirstMovement = parent.isFirstMovement;
            for (var i = 0; i < 6; i++)
                for (var j = 0; j < 7; j++)
                    _boardData[i, j] = parent._boardData[i, j];
        }

        public bool? GetStateAt(int row, int col) => _boardData[row, col];

        public Board MakeMove(int col, bool value)
        {
            var child = new Board(this) {isFirstMovement = false};
            var isValid = false;
            for(var i = 5; i >= 0; i --)
            {
                if (child._boardData[i,col] == null)
                {
                    child._boardData[i, col] = value;
                    isValid = true;

                    break;
                }
            }

            if (!isValid)
                throw new InvalidOperationException("둘 수 없는 곳입니다");

            return child;
        }

        public bool IsValidMove(int col)
        {
            if (isFirstMovement && col == 3)
                return false;

            var isValid = false;

            for (var i = 5; i >= 0; i--)
            {
                if (_boardData[i, col] == null)
                {
                    isValid = true;
                }
            }

            return isValid;
        }
        
        public void Print()
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

        public BoardState DetermineState()
        {
            var anySpace = false;

            for (var i = 0; i < 6; i++)
            for (var j = 0; j < 7; j++)
            for (var k = 0; k < 4; k++)
                if (_boardData[i,j].HasValue && IsFourInARow(i, j, k))
                    return new BoardState(true, _boardData[i, j].Value);

            for(var i = 0; i < 7; i ++)
                anySpace |= IsValidMove(i);

            return !anySpace ? new BoardState(true, null) : new BoardState(false, null);
        }

        private bool IsFourInARow(int row, int col, int direction, int depth = 1)
        {
            if (depth == 4)
                return true;

            var val = _boardData[row, col];

            if (!val.HasValue)
                return false;

            var rowTo = row + dRow[direction];
            var colTo = col + dCol[direction];

            if (rowTo < 0 || rowTo >= 6 || colTo < 0 || colTo >= 7)
                return false;

            if (val.Value == _boardData[rowTo, colTo])
                return IsFourInARow(rowTo, colTo, direction, depth + 1);

            return false;
        }
    }
}
