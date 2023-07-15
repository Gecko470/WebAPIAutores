using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPIAutores.DTOs;
using WebAPIAutores.Services;

namespace WebAPIAutores.Controllers.v1
{
    [ApiController]
    [Route("api/v1/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly HashService hash;
        private readonly IDataProtector dataProtector;

        public CuentasController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, IDataProtectionProvider dataProtectionProvider, HashService hash)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.hash = hash;
            dataProtector = dataProtectionProvider.CreateProtector("jflfj@#vniugqvm~@vwqp$mwqv");
        }

        [HttpGet("hash/{textoPlano}")]
        public ActionResult Hash(string textoPlano)
        {
            var resultado1 = hash.Hash(textoPlano);
            var resultado2 = hash.Hash(textoPlano);

            return Ok(new
            {
                TextoPlano = textoPlano,
                Resultado1 = resultado1,
                Resultado2 = resultado2
            });
        }

        [HttpGet("encriptacion")]
        public ActionResult Encriptacion()
        {
            var textoPlano = "WebArtisanX";
            var textoEnciptado = dataProtector.Protect(textoPlano);
            var textoDesencriptado = dataProtector.Unprotect(textoEnciptado);

            return Ok(new
            {
                textoPlano,
                textoEncriptado = textoEnciptado,
                textoDesencriptado
            });
        }

        [HttpGet("encriptacionTiempo")]
        public ActionResult EncriptacionPorTiempo()
        {
            var protectorPorTiempo = dataProtector.ToTimeLimitedDataProtector();
            var textoPlano = "WebArtisanX";
            var textoEnciptado = protectorPorTiempo.Protect(textoPlano, TimeSpan.FromSeconds(5));
            Thread.Sleep(6000);
            var textoDesencriptado = protectorPorTiempo.Unprotect(textoEnciptado);

            return Ok(new
            {
                textoPlano,
                textoEncriptado = textoEnciptado,
                textoDesencriptado
            });
        }


        [HttpPost("registrar", Name = "registrarUsuario")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser() { UserName = credencialesUsuario.Email, Email = credencialesUsuario.Email };

            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        [HttpPost("login", Name = "loginUsuario")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email, credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login incorrecto, revise sus credenciales..");
            }
        }

        [HttpGet("renovarToken", Name = "renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> RenovarToken()
        {
            var emailClaim = HttpContext.User.Claims.Where(x => x.Type == "Email").FirstOrDefault();
            var email = emailClaim.Value;

            var credencialesUsuario = new CredencialesUsuario()
            {
                Email = email
            };

            return await ConstruirToken(credencialesUsuario);
        }

        private async Task<ActionResult<RespuestaAutenticacion>> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>()
            {
                new Claim("Email", credencialesUsuario.Email)
            };

            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["LlaveJWT"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
        }

        [HttpPost("hacerAdmin", Name = "hacerAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDTO adminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(adminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));

            return NoContent();
        }

        [HttpPost("deshacerAdmin", Name = "deshacerAdmin")]
        public async Task<ActionResult> DeshacerAdmin(EditarAdminDTO adminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(adminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));

            return NoContent();
        }
    }
}
