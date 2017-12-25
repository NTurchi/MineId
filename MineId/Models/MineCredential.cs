using System;
using System.ComponentModel.DataAnnotations;

namespace MineId.Models {
    /// <summary>
    /// Credential model for sign in to the provider
    /// </summary>
    public class MineCredential {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}