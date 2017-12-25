using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MineId.Models
{
    /// <summary>
    /// Main generic user for mineId identity provider
    /// </summary>
    public class MineUser : IdentityUser
    {
        /// <summary>
        /// Generic ID 
        /// </summary>
        /// <returns>User unique Id</returns>
        public string UserMineId { get; set; }

        /// <summary>
        /// Application when user credential is available
        /// </summary>
        public List<MineUserApplication> MineUserApplications { get; } = new List<MineUserApplication>();

        /// <summary>
        /// Application managed by the user
        /// </summary>
        public List<MineApplication> ApplicationRoot { get; set; } = new List<MineApplication>();
    }
}