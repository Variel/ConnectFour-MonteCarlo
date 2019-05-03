using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MonteCarloTest
{
    public class Game
    {
        private Board _currentBoard;
        private IPlayer[] _players = new IPlayer[2];

        public Game(IPlayer player1, IPlayer player2)
        {
            _players[0] = player1;
            _players[1] = player2;
        }

        public IPlayer StartLoop()
        {
            _currentBoard = new Board();

            BoardState currentState;
            int currentPlayer = 0;

            //_currentBoard.Print();

            do
            {
                int move;
                do
                {
                    move = _players[currentPlayer].NextMove(_currentBoard);
                } while (!_currentBoard.IsValidMove(move));

                _currentBoard = _currentBoard.MakeMove(move, _players[currentPlayer].Identifier);

                currentPlayer = (currentPlayer + 1) % 2;
                //_currentBoard.Print();
                //Console.WriteLine(
                //    $"●: {_players[0].Name}\n" +
                //    $"○: {_players[1].Name}\n" +
                //    $"");
            } while (!(currentState = _currentBoard.DetermineState()).IsOver);

            var winner = (currentState.WinnerIdentifier.HasValue
                ? currentState.WinnerIdentifier.Value ? _players[1] : _players[0]
                : null);

            return winner;
        }
    }
}
