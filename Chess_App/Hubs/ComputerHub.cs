using ChessEngine.Engine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Chess_App.Hubs
{
    [Authorize]
    public class ComputerHub : Hub
    {      
       private static List<string> _moveList =new List<string>();
       private readonly PositionMoveParser _moveParser = new PositionMoveParser();
        private static readonly ChessEngine _engine = new ChessEngine();

       byte[] movesToParse = new byte[2];
       public string engineMove; 

        public override async Task OnConnectedAsync()
        {
            _engine.NewGame();           
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {           
        }

        public async Task PlayerMoved(string source, string target)
        {           
            movesToParse = _moveParser.ParsedMoves(source, target);            
            _engine.HumanMove(movesToParse[0], movesToParse[1]);
            _moveList.Add(_engine.LastMove.PgnMove);
            await MakeComputerMove();
        }
        

        public async Task ComputerStartsGame(string diff, string color) 
        {
            switch (diff)
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

            if (color == "white")
                _engine.HumanPlayer = ChessPieceColor.White;
            else 
            {
                _engine.HumanPlayer = ChessPieceColor.Black;
                await Clients.Caller.SendAsync("Flip");
                await MakeComputerMove();
            }             
        }

        async public Task MakeComputerMove() 
        {            
            _engine.AiPonderMove();            
            engineMove = _engine.LastMove.PgnMove;
            _moveList.Add(_engine.LastMove.PgnMove);
            await Clients.Caller.SendAsync("ComputerMoved", engineMove);            
        }
    }
}
