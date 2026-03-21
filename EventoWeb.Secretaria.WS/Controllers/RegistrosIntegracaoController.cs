using EventoWeb.Secretaria.Aplicacao.RegistrosIntegracao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eventoweb_secretaria_back.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrosIntegracaoController : ControllerBase
    {
        private readonly AppRegistroIntegracaoObtencao m_AppObtencao;
        private readonly AppRegistroIntegracaoInclusao m_AppInclusao;

        public RegistrosIntegracaoController(
            AppRegistroIntegracaoObtencao appObtencao,
            AppRegistroIntegracaoInclusao appInclusao)
        {
            m_AppObtencao = appObtencao ?? throw new ArgumentNullException(nameof(appObtencao));
            m_AppInclusao = appInclusao ?? throw new ArgumentNullException(nameof(appInclusao));
        }

        // GET api/registrosintegracao/listar-conta/5
        [HttpGet("listar-conta/{idConta}")]
        [Authorize("Bearer")]
        public ActionResult<IList<DTORegistroIntegracao>> ListarPorConta(int idConta)
        {
            return Ok(m_AppObtencao.ObterPorConta(idConta));
        }

        // POST api/registrosintegracao/incluir
        [HttpPost("incluir")]
        [Authorize("Bearer")]
        public ActionResult<DTORegistroIntegracao> Incluir([FromBody] DTORegistroIntegracaoInclusao dto)
        {
            return Ok(m_AppInclusao.Incluir(dto));
        }
    }
}
