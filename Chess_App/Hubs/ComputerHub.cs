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
    public class ComputerHub : Hub
    {
        Engine engine = new Engine();

        public override async Task OnConnectedAsync()
        {
            //engine.AiPonderMove();           
            //string msg = engine.LastMove.PgnMove;
            //await Clients.Caller.SendAsync("connected",msg);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {           
        }
    }
}
