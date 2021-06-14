using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Chess_App.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the Chess_AppUser class
    public class Chess_AppUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string NickName { get; set; }
        public IEnumerable<Chess_GameHistory> GameHistory { get; set; }
    }
}
