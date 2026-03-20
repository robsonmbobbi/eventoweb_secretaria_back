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

        public RegistrosIntegracaoController(AppRegistroIntegracaoObtencao appObtencao)
        {
            m_AppObtencao = appObtencao ?? throw new ArgumentNullException(nameof(appObtencao));
        }

        // GET api/registrosintegracao/listar-conta/5
        [HttpGet("listar-conta/{idConta}")]
        [Authorize("Bearer")]
        public ActionResult<IList<DTORegistroIntegracao>> ListarPorConta(int idConta)
        {
            return Ok(m_AppObtencao.ObterPorConta(idConta));
        }
    }
}
