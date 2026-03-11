using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Aplicacao.Seguranca;
using EventoWeb.Secretaria.Negocio.Repositorios;
using eventoweb_secretaria_back.Controllers.DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eventoweb_secretaria_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosComumController : ControllerBase
    {
        private readonly IContexto m_Contexto;
        private readonly IUsuarios m_Usuarios;

        public UsuariosComumController(IContexto contexto, IUsuarios usuarios)
        {
            m_Contexto = contexto;
            m_Usuarios = usuarios;
        }

        [Authorize("Bearer")]
        [HttpGet("obter")]
        public DTOUsuario? Obter()
        {            
            var app = new AppUsuarioObter(m_Contexto, m_Usuarios)
            {
                Login = User.Identity?.Name! ?? throw new Exception("Usuário não autenticado")
            };
            return app.Obter();
        }
        
        [Authorize("Bearer")]
        [HttpPut("atualizar")]
        public void Atualizar(DTOUsuario dto)
        {
            var login = User.Identity?.Name! ?? throw new Exception("Usuário não autenticado");
            if (login.ToUpper() != dto.Login.ToUpper())
                throw new Exception("Login dos dados de alteração diferente do login autenticado");

            var app = new AppUsuarioAlteracaoDados(m_Contexto, m_Usuarios)
            {
                DadosUsuario = dto
            };

            app.Alterar();
        }

        [Authorize("Bearer")]
        [HttpPut("atualizar-senha")]
        public void AlteraSenhaAdm(DTOAlteracaoSenhaComumWS dto)
        {
            var app = new AppUsuarioAlteracaoSenhaPeloUsuario(m_Contexto, m_Usuarios)
            {
                Login = User.Identity?.Name! ?? throw new Exception("Usuário não autenticado"),
                SenhaAtual = dto.SenhaAtual,
                NovaSenha = dto.NovaSenha,
                NovaSenhaRepeticao = dto.NovaSenhaRepetida
            };

            app.Alterar();
        }
    }
}
