using EventoWeb.Comum.Aplicacao.Inscricoes;
using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Secretaria.Aplicacao.Inscricoes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eventoweb_secretaria_back.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class InscricoesController : ControllerBase
    {
        private AppInscricaoListagem mAppListagem;

        public InscricoesController(AppInscricaoListagem appListagem)
        {
            mAppListagem = appListagem;
        }

        // GET api/eventos/obter-todos
        [HttpGet("listar/evento/{idEvento}/situacao/{situacao}")]
        [Authorize("Bearer")]
        public IEnumerable<DTOInscricaoListagem> Get(int idEvento, EnumSituacaoInscricao situacao)
        {
            return mAppListagem.Listar(idEvento, situacao);
        }
    }
}
