using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Chess_App.Areas.Identity.Data
{
    [Table("Chess_GameHistory")]
    public class Chess_GameHistory
    {
        public int Chess_GameHistoryId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string PlayerID { get; set; }

        
        [Column(TypeName = "nvarchar(100)")]
        public string Opponnent { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string FEN { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GameDate { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? MoveSequence { get; set; }
    }
}
