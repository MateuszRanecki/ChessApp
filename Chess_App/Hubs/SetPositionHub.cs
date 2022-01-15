using ChessCoreEngine;
using ChessEngine.Engine;
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
        private static Engine _engine;
        private byte[] movesToParse = new byte[2];
        private readonly MoveParser _moveParser = new MoveParser();
        public override async Task OnConnectedAsync()
        {
            
        }

        public async Task PlayWithComputer(string FEN, string difficulty, string playerColor) 
        {
            _engine = new Engine(FEN);

            if (playerColor == "white")
                _engine.HumanPlayer = ChessPieceColor.White;
            else
                _engine.HumanPlayer = ChessPieceColor.Black;

            switch (difficulty) 
            {
                case "easy":
                    _engine.GameDifficulty = Engine.Difficulty.Easy;
                    break;
                case "hard":
                    _engine.GameDifficulty = Engine.Difficulty.Hard;
                    break;
                case "veryHard":
                    _engine.GameDifficulty = Engine.Difficulty.VeryHard;
                    break;
                default:
                    _engine.GameDifficulty = Engine.Difficulty.Medium;
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
            Engine test = new Engine(FEN);
            if (test.FEN == "8/8/8/8/8/8/8/8 w - - 0 0")             
                await Clients.Caller.SendAsync("WrongFEN");
            else {
                _engine = new Engine(FEN);
                await Clients.Caller.SendAsync("SetBoardFromFEN",FEN);
            }

        }

    }
}
