using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MineId.Models;

namespace MineId.Controllers {
    /// <summary>
    /// Controller lié gérant l'authentification à l'application
    /// </summary>
    [Route ("api/[controller]")]
    public class AuthController : Controller {
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<MineUser> _signInM;
        private readonly UserManager<MineUser> _userManager;
        private readonly IPasswordHasher<MineUser> _hasher;

        public AuthController (IConfiguration config,
            ILogger<AuthController> logger,
            SignInManager<MineUser> signInM,
            UserManager<MineUser> userManager,
            IPasswordHasher<MineUser> hasher) {
            _config = config;
            _logger = logger;
            _signInM = signInM;
            _userManager = userManager;
            _hasher = hasher;
        }

        /// <summary>
        /// Route pour tester le déploiement de l'API
        /// </summary>
        /// <returns>Un message de confirmation du bon fonctionnement</returns>
        [HttpGet ("/ping")]
        public ActionResult Ping () {
            return Ok ("It's ok !");
        }

        public async Task<IActionResult> Login ([FromBody] MineCredential model) {
            try {
                var user = await _userManager.FindByNameAsync (model.UserName);
                if (user != null) {
                    if (_hasher.VerifyHashedPassword (user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success) {
                        var userClaims = await _userManager.GetClaimsAsync (user);
                        var claims = new [] {
                            new Claim (JwtRegisteredClaimNames.Sub, user.UserName),
                                new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid ().ToString ()),
                                new Claim (JwtRegisteredClaimNames.GivenName, "MySoundBoxUser"),
                                new Claim (JwtRegisteredClaimNames.Email, user.Email)
                        }.Union (userClaims);

                        var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (_config["Tokens:Key"]));
                        var creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken (
                            issuer : _config["Tokens:Issuer"],
                            audience : _config["Tokens:Audience"],
                            claims : claims,
                            expires : DateTime.UtcNow.AddMinutes (15),
                            signingCredentials : creds
                        );

                        return Ok (new {
                            token = new JwtSecurityTokenHandler ().WriteToken (token),
                                expiration = token.ValidTo
                        });
                    }
                }
            } catch (Exception ex) {
                _logger.LogError ($"Erreur lors d'une tentative de connexion à l'API : {ex}");
            }
            return BadRequest (new {
                message = "Erreur durant la tentative de connexion",
                    code = "400"
            });
        }
    }
}