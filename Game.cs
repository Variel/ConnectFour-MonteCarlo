using System;
using System.Collections.Generic;
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

        public void StartLoop()
        {
            _currentBoard = new Board();

            BoardState currentState;
            int currentPlayer = 0;

            PrintState();

            do
            {
                int move;
                do
                {
                    move = _players[currentPlayer].NextMove(_currentBoard);
                } while (!_currentBoard.IsValidMove(move));

                _currentBoard = _currentBoard.MakeMove(move, _players[currentPlayer].Identifier);

                currentPlayer = (currentPlayer + 1) % 2;

                PrintState();
                currentState = _currentBoard.DetermineState();
            } while (!currentState.IsOver);

            Console.WriteLine("Winner is " + GetPlayerName(currentState.WinnerIdentifier));
        }

        public void PrintState()
        {
            _currentBoard.Print();
            Console.WriteLine(
                $"●: {_players[0].Name}\n" +
                $"○: {_players[1].Name}\n" +
                $"");
        }

        public string GetPlayerName(bool? identifier)
        {
            if (!identifier.HasValue)
                return "Nobody";

            switch(identifier.Value)
            {
                case true:
                    return _players[1].Name;
                case false:
                    return _players[0].Name;
            }

            return "Nobody";
        }
    }
}
