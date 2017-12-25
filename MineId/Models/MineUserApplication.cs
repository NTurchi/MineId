using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineId.Models
{
    /// <summary>
    /// For relation Many to Many
    /// </summary>
    public class MineUserApplication
    {
        public int MineUserId { get; set; }
        public MineUser MineUser { get; set; }

        public int MineApplicationId { get; set; }

        public MineApplication MineApplication { get; set; }
    }
}
