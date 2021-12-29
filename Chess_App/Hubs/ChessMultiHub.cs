using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Chess_App.Models;
using Microsoft.AspNetCore.Authorization;
using Chess_App.Data;
using Chess_App.Areas.Identity.Data;

namespace Chess_App.Hubs
{
    public class ChessMultiHub : Hub
    {
        private Chess_AppContext _context;

        private static Dictionary<string, string> UserList = new Dictionary<string, string>();
        private static Dictionary<string, PlayersModel> RoomList = new Dictionary<string, PlayersModel>();
        private static Dictionary<string, string> ColorList = new Dictionary<string, string>();


        public ChessMultiHub(Chess_AppContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var User_Name = Context.User.Identity.Name;
            var User_ConID = Context.ConnectionId;
            UserList.Add(User_ConID, User_Name);
            await SendLoggedUsers();
            await DecideColors();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            UserList.Remove(Context.ConnectionId);
            foreach (var item in UserList) 
            {
               await Clients.Client(item.Key).SendAsync("ClearList");
            }
        }

        public async Task SendLoggedUsers()
        {
            var caller = Context.User.Identity.Name;
            if (UserList.Count ==2) 
            {
                var Second = UserList.Where(x => x.Value != caller).FirstOrDefault();
                foreach (var item in UserList)
                {
                    await Clients.Client(Second.Key).SendAsync("ShowLoggedPlayers", item.Value);
                    await Clients.Caller.SendAsync("ShowLoggedPlayers", item.Value);
                    
                }                
            }
        }


        public async Task MakeMove(string source, string target)
        {
            string WhoIsSent = MyOpponent();
            var receiver = UserList.Where(x => x.Value == WhoIsSent).FirstOrDefault();
            await Clients.Client(receiver.Key).SendAsync("UpdateBoard", source, target);
        }




        public string MyOpponent()
        {
            string opponent = null;
            string caller = Context.User.Identity.Name;
            foreach (var item in UserList)
            {
                if (item.Value != caller)
                    opponent = item.Value;
            }

            return opponent;

        }

        public async Task DecideColors()
        {
            if (UserList.Count == 2 )
            {
                var PlayerOne = UserList.FirstOrDefault();
                var PlayerTwo = UserList.Where(x => x.Value != PlayerOne.Value).FirstOrDefault();

                //ColorList.Add(PlayerOne.Value, "w");
                //ColorList.Add(PlayerTwo.Value, "b");

                await Clients.Client(PlayerOne.Key).SendAsync("CanPlayerMove", "w");
                await Clients.Client(PlayerTwo.Key).SendAsync("CanPlayerMove", "b");
            }
        }

        public async Task PlayerResigned( string color)
        {
            var PlayerOne = UserList.FirstOrDefault();
            var PlayerTwo = UserList.Where(x => x.Value != PlayerOne.Value).FirstOrDefault();

            await Clients.Client(PlayerOne.Key).SendAsync("OverByResign",color);
            await Clients.Client(PlayerTwo.Key).SendAsync("OverByResign", color);
        }

        public async Task SendDrawOffer() 
        {
            var fromWho = Context.User.Identity.Name;
            var toWho = UserList.Where(x => x.Value != fromWho).FirstOrDefault();
            await Clients.Client(toWho.Key).SendAsync("ReceiveDrawOffer",fromWho);
        }


        public async Task DrawOfferAccepted()
        
        {
            var PlayerOne = UserList.FirstOrDefault();
            var PlayerTwo = UserList.Where(x => x.Value != PlayerOne.Value).FirstOrDefault();

            await Clients.Client(PlayerOne.Key).SendAsync("ShowDraw");
            await Clients.Client(PlayerTwo.Key).SendAsync("ShowDraw");
        }


        public async Task SendMessageToAll(string message) 
        {
            var caller = Context.User.Identity.Name;
            foreach (var item in UserList) 
            {
                await Clients.Client(item.Key).SendAsync("ReceiveMessageToAll",message,caller);
            }
        }

        public async Task SaveFenFromGame(string fen,string moves) 
        {
            var PlayerOne = Context.User.Identity.Name;
            var PlayerTwo = UserList.Where(x => x.Value != PlayerOne).FirstOrDefault();

            var game = new Chess_GameHistory()
            {
                PlayerID = PlayerOne,
                Opponnent = PlayerTwo.Value,
                FEN = fen,
                GameDate=DateTime.Now,
                MoveSequence=moves                
            };
            _context.GameHistory.Add(game);
            _context.SaveChanges();
         
        }

    }
}
