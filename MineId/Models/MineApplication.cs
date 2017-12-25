using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineId.Models
{
    /// <summary>
    /// Application which user mine identity provider
    /// </summary>
    public class MineApplication
    {
        /// <summary>
        /// Unique identifier for mineApplication object
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Application Name
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Unique application manager user 
        /// </summary>
        public MineUser RootUser { get; set; }

        /// <summary>
        /// User in this application
        /// </summary>
        public List<MineUserApplication> MineUsersApplication { get; } = new List<MineUserApplication>();
    }
}
