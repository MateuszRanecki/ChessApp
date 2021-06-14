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
    public class HistoryHub:Hub
    {
        private static Dictionary<string, string> PreviousGamesList = new Dictionary<string, string>();
        private Chess_AppContext _context;
        private static List<string> FENList = new List<string>();
        private static int MoveCounter = 0;

        public HistoryHub(Chess_AppContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {           
            await ShowMyPreviousGames();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await ClearGameList();
            MoveCounter = 0;
        }


        public async Task ShowMyPreviousGames()
        {            
            var player = Context.User.Identity.Name;
            var GamesList = _context.GameHistory.Where(x => x.PlayerID == player).ToList();
            foreach (var item in GamesList)
            {
                await Clients.Caller.SendAsync("ShowPlayedGames", item.Opponnent,item.GameDate.ToString());
            }

        }

        public async Task ReviewGame(string date) 
        {
            FENList.Clear();
            MoveCounter = 0;
            string myFen = null;
            string sequence = null;
            foreach (var item in _context.GameHistory)
            {                
                var example = item.GameDate.ToString();
                if (example.Equals(date) && item.PlayerID == Context.User.Identity.Name) 
                {
                    myFen = item.FEN;
                    sequence = item.MoveSequence;
                }
            }
            fenParser(myFen);
           
            await Clients.Caller.SendAsync("CreateEnvForReview");
            await Clients.Caller.SendAsync("ShowSequence",sequence);
        }

        public async Task ClearGameList() 
        {
            PreviousGamesList.Clear();
            FENList.Clear();
        }

        public void fenParser(string fen) 
        {            
            string parsedPosition = null;
            string splitter = ";";
            for (int i = 4; i < fen.Length; i++) 
            {
                if (fen[i].ToString().Equals(splitter))
                {
                    FENList.Add(parsedPosition);
                    parsedPosition = null;
                }
                else 
                {
                    parsedPosition += fen[i];
                }
            }                     
            
        }

        public async Task UpdateMove(bool isFoward) 
        {
            
            if (isFoward && MoveCounter < FENList.Count)
            {
                ++MoveCounter;
                var fen = FENList[MoveCounter];                
                await Clients.Caller.SendAsync("UpdateBoard", fen);                
            }
            else if (!(isFoward) && MoveCounter >= 0)
            {
                --MoveCounter;
                var fen = FENList[MoveCounter];
                await Clients.Caller.SendAsync("UpdateBoard", fen);                
            }
            else if (isFoward && MoveCounter == FENList.Count)
            {
                --MoveCounter;
                await Clients.Caller.SendAsync("OutOfRangeMoves", true);
            }
            else if (!(isFoward) && MoveCounter < 0)
            {
                MoveCounter = 0;
                await Clients.Caller.SendAsync("OutOfRangeMoves", false);
            }
        }

    }
}
