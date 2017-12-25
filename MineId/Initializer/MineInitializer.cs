using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MineId.Models;

namespace MineId.Initializer {
    /// <summary>
    /// Initialize the root user when provider is run for the first time 
    /// </summary>
    public class MineInitializer {
        private RoleManager<IdentityRole> _roleM;
        private readonly ILogger<MineInitializer> _logger;
        private UserManager<MineUser> _userM;

        private IConfiguration _conf;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="conf">Configuration de l'application</param>
        /// <param name="userM">Objet gérant les utilisateurs de Microsoft Identity</param>
        /// <param name="roleM">Objet gérant les rôles de Microsoft Identity</param>        
        public MineInitializer (ILogger<MineInitializer> logger, IConfiguration conf, UserManager<MineUser> userM, RoleManager<IdentityRole> roleM) {
            _conf = conf;
            _userM = userM;
            _roleM = roleM;
            _logger = logger;
        }

        /// <summary>
        /// Insère un utilisateur administrateur de l'api par défaut s'il n'en existe pas actuellement dans la base de données
        /// </summary>
        /// <returns>Une tâche asynchrone</returns>
        public async Task Seed () {

            var userName = "nturchi";
            var userPassword = "Azerty.1234!";
            var userRole = "Admin";

            var user = await _userM.FindByNameAsync (userName);

            // Ajout de l'utilisateur s'il n'existe pas
            if (user == null) {
                _logger.LogInformation ("L'utilisateur administrateur n'existe pas, création en cours...");

                if (!(await _roleM.RoleExistsAsync (userRole))) {
                    _logger.LogInformation ("Le rôle Admin n'existe pas, création en cours...");
                    var role = new IdentityRole (userRole);
                    await _roleM.CreateAsync (role);
                }

                user = new MineUser () {
                    UserName = userName,
                    Email = "nicolas.turchi.dev@gmail.com"
                };

                var userResult = await _userM.CreateAsync (user, userPassword);
                var roleResult = await _userM.AddToRoleAsync (user, userRole);
                var claimResult = await _userM.AddClaimAsync (user, new Claim ("SuperUser", "True"));

                if (!userResult.Succeeded || !roleResult.Succeeded || !claimResult.Succeeded) {
                    throw new InvalidOperationException ("Erreur lors de l'ajout d'utilisateur et de son role par défaut");
                }

            }
        }
    }
}