
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess_App.Hubs
{
    [Authorize]
    public class SetPositionHub:Hub
    {
        private static ChessEngine _engine;
        private byte[] movesToParse = new byte[2];
        private readonly PositionMoveParser _moveParser = new PositionMoveParser();
        public override async Task OnConnectedAsync()
        {
            
        }

        public async Task PlayWithComputer(string FEN, string difficulty, string playerColor) 
        {
            _engine = new ChessEngine(FEN);

            if (playerColor == "white")
                _engine.HumanPlayer = ChessPieceColor.White;
            else
                _engine.HumanPlayer = ChessPieceColor.Black;

            switch (difficulty) 
            {
                case "easy":
                    _engine.GameDifficulty = ChessEngine.Difficulty.Easy;
                    break;
                case "hard":
                    _engine.GameDifficulty = ChessEngine.Difficulty.Hard;
                    break;
                case "veryHard":
                    _engine.GameDifficulty = ChessEngine.Difficulty.VeryHard;
                    break;
                default:
                    _engine.GameDifficulty = ChessEngine.Difficulty.Medium;
                    break;
            }

            if (_engine.WhoseMove == ChessPieceColor.White && _engine.HumanPlayer == ChessPieceColor.Black ||
                _engine.WhoseMove == ChessPieceColor.Black && _engine.HumanPlayer == ChessPieceColor.White)
            {
                _engine.AiPonderMove();
                string lastMove = _engine.LastMove.PgnMove;
                await Clients.Caller.SendAsync("ComputerStarts", lastMove);
            }
            
        }

        public async Task MakeHumanMove(string source, string target) 
        {
            movesToParse = _moveParser.ParsedMoves(source,target);
            _engine.HumanMove(movesToParse[0], movesToParse[1]);
            string lastMove = _engine.LastMove.PgnMove;
            _engine.AiPonderMove();
            lastMove = _engine.LastMove.PgnMove;
            await Clients.Caller.SendAsync("ComputerStarts", lastMove);
        }

        public async Task ValidateFEN(string FEN) 
        {
            ChessEngine test = new ChessEngine(FEN);
            if (test.FEN == "8/8/8/8/8/8/8/8 w - - 0 0")             
                await Clients.Caller.SendAsync("WrongFEN");
            else {
                _engine = new ChessEngine(FEN);
                await Clients.Caller.SendAsync("SetBoardFromFEN",FEN);
            }

        }

    }
}
