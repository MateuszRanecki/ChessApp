using ChessCoreEngine;
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
       private static readonly Engine _engine= new Engine();
       private static List<string> _moveList =new List<string>();
       private readonly MoveParser _moveParser = new MoveParser();      

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
        

        public async Task ComputerStartsGame() 
        {            
            _engine.HumanPlayer = ChessPieceColor.Black;
            await MakeComputerMove();           
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
