using EventoWeb.Secretaria.Aplicacao.Seguranca;
using eventoweb_secretaria_back.Controllers.DTOS;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace eventoweb_secretaria_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacaoController : ControllerBase
    {
        private AppUsuarioAutenticacao m_App;

        public AutenticacaoController(AppUsuarioAutenticacao app)
        {
            m_App = app;
        }

        [AllowAnonymous]
        [HttpPut("autenticar")]
        public DTWAutenticacao? Autenticar([FromBody] DTWDadosAutenticacao dadosAutenticacao,
            [FromServices] ConfiguracaoAutorizacao configuracaoAutorizacao)
        {
            m_App.Login = dadosAutenticacao.Login;
            m_App.Senha = dadosAutenticacao.Senha;

            var usuario = m_App.Autenticar();
            if (usuario != null)
                return new DTWAutenticacao
                {
                    Usuario = usuario,
                    Validade = DateTime.Now + TimeSpan.FromSeconds(configuracaoAutorizacao.TempoSegExpirar),
                    TokenAutenticacao = GerarTokenApi(configuracaoAutorizacao, usuario)
                };
            else
                return null;
        }

        [Authorize("Bearer")]
        [HttpDelete("desautenticar")]
        public void Desautenticar()
        {
            HttpContext.SignOutAsync();
        }

        private static string GerarTokenApi(ConfiguracaoAutorizacao configuracaoAutorizacao, DTOUsuario usuario)
        {
            ClaimsIdentity identidade = new(
                    new GenericIdentity(usuario.Login, "Login"),
                    [
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Login)
                    ]
                );

            if (usuario.EhAdministrador)
                identidade.AddClaim(new Claim(ClaimTypes.Role, "ADM"));

            DateTime dataCriacao = DateTime.Now;
            DateTime dataExpiracao = dataCriacao +
                TimeSpan.FromSeconds(configuracaoAutorizacao.TempoSegExpirar);

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = configuracaoAutorizacao.Emissor,
                Audience = configuracaoAutorizacao.Publico,
                SigningCredentials = configuracaoAutorizacao.CredenciasAssinatura,
                Subject = identidade,
                NotBefore = dataCriacao,
                Expires = dataExpiracao
            });

            return handler.WriteToken(securityToken);
        }
    }
}
