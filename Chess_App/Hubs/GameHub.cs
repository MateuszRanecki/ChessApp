using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Chess_App.Models;
using Microsoft.AspNetCore.Authorization;
using Chess_App.Data;


namespace Chess_App.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {      

        private static Dictionary<string, string> UserList = new Dictionary<string, string>();
        

        private Chess_AppContext _context;

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public GameHub(Chess_AppContext context)
        {
            _context = context;
        }
        public override async Task OnConnectedAsync()
        {
            var User_Name = Context.User.Identity.Name;
            var User_ConID = Context.ConnectionId;
            UserList.Add(User_ConID, User_Name);
            await SendLoggedUsers();
            await ShowMyPreviousGames();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            UserList.Remove(Context.ConnectionId);
            foreach (var item in UserList)
            {
                if (item.Value != Context.User.Identity.Name)
                {
                    await Clients.Client(item.Key).SendAsync("DisconnectPlayer", Context.User.Identity.Name);
                    
                }
            }
            await LoggedAfterDisconnect();
        }

        public async Task SendLoggedUsers()
        {
            foreach (var item in UserList)
            {
                if (item.Value != Context.User.Identity.Name)
                {
                    await Clients.Caller.SendAsync("ShowLoggedPlayers", item.Value);
                }

            }
            foreach (var item in UserList) 
            {
                if (item.Value != Context.User.Identity.Name)
                {
                    await UpdateLoggedUsers(item.Key,item.Value);
                }
            }
        }

        public async Task UpdateLoggedUsers(string receiverKey,string receiverValue)        
        {
            await Clients.Client(receiverKey).SendAsync("ClearList", Context.User.Identity.Name);
            await Clients.Client(receiverKey).SendAsync("ShowLoggedPlayers", Context.User.Identity.Name);
        }

        public async Task SendInvite(string receiver)
        {
            var toWho = UserList.Where(x => x.Value == receiver).FirstOrDefault();
            var caller = Context.User.Identity.Name;
            await Clients.Client(toWho.Key).SendAsync("ReceiveInvite", caller);
        }

        public async Task RedirectPlayers(string sender)
        {
            var GameCreator = UserList.Where(x => x.Value == sender).FirstOrDefault();            
            await Clients.Client(GameCreator.Key).SendAsync("SendToRoom");
            await Clients.Caller.SendAsync("SendToRoom");            
        }

        public async Task LoggedAfterDisconnect() 
        {
            foreach (var item in UserList) 
            {
                foreach (var subItem in UserList) 
                {
                    if (subItem.Value != item.Value) 
                    {
                        await Clients.Client(item.Value).SendAsync("ShowLoggedPlayers", subItem.Value);
                    }
                }
            }
        }

        public async Task ShowMyPreviousGames() 
        {
            var player = Context.User.Identity.Name;
            var GamesList = _context.GameHistory.Where(x => x.PlayerID == player).ToList();
            foreach (var item in GamesList) 
            {
                await Clients.Caller.SendAsync("ShowPlayedGames", item.Opponnent);
            }
            
        }
    }
}
