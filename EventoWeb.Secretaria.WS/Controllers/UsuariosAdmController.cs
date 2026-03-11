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
    public class UsuariosAdmController : ControllerBase
    {
        private readonly IContexto m_Contexto;
        private readonly IUsuarios m_Usuarios;

        public UsuariosAdmController(IContexto contexto, IUsuarios usuarios)
        {
            m_Contexto = contexto;
            m_Usuarios = usuarios;
        }

        [Authorize("Bearer", Roles = "ADM")]
        [HttpGet("listar-todos")]
        public IList<DTOUsuario> Listar()
        {
            var app = new AppUsuarioListagem(m_Contexto, m_Usuarios);
            return app.ListarTodos();
        }

        [Authorize("Bearer", Roles = "ADM")]
        [HttpGet("obter/{nomeUsuario}")]
        public DTOUsuario? Obter(string nomeUsuario)
        {
            var app = new AppUsuarioObter(m_Contexto, m_Usuarios)
            {
                Login = nomeUsuario
            };
            return app.Obter();
        }

        [Authorize("Bearer", Roles = "ADM")]
        [HttpPost("incluir")]
        public void Incluir(DTOUsuarioInclusao dto)
        {
            var app = new AppUsuarioInclusao(m_Contexto, m_Usuarios)
            {
                DadosUsuario = dto
            };

            app.Incluir();
        }

        [Authorize("Bearer", Roles = "ADM")]
        [HttpPut("atualizar")]
        public void Atualizar(DTOUsuario dto)
        {
            var app = new AppUsuarioAlteracaoDados(m_Contexto, m_Usuarios)
            {
                DadosUsuario = dto
            };

            app.Alterar();
        }

        [Authorize("Bearer", Roles = "ADM")]
        [HttpDelete("excluir/{nomeUsuario}")]
        public void Excluir(string nomeUsuario)
        {
            var app = new AppUsuarioExclusao(m_Contexto, m_Usuarios)
            {
                Login = nomeUsuario
            };
            app.Excluir();
        }

        [Authorize("Bearer", Roles = "ADM")]
        [HttpPut("atualizar-senha/{nomeUsuario}")]
        public void AlteraSenhaAdm(string nomeUsuario, DTOAlteracaoSenhaWS dto)
        {
            var app = new AppUsuarioAlteracaoSenhaPeloAdm(m_Contexto, m_Usuarios)
            {
                Login = nomeUsuario,
                NovaSenha = dto.NovaSenha,
                NovaSenhaRepeticao = dto.NovaSenhaRepetida
            };

            app.Alterar();
        }
    }
}
