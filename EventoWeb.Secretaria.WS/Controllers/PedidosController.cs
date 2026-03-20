using EventoWeb.Comum.Aplicacao.Pedidos;
using EventoWeb.Secretaria.Aplicacao.Pedidos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eventoweb_secretaria_back.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController(AppPedidoObtencao appObtencao, AppPedidoInclusao appInclusao) : ControllerBase
    {
        private readonly AppPedidoObtencao m_AppObtencao = appObtencao ?? throw new ArgumentNullException(nameof(appObtencao));
        private readonly AppPedidoInclusao m_AppInclusao = appInclusao ?? throw new ArgumentNullException(nameof(appInclusao));

        // GET api/pedidos/obter-por-inscricao/5
        [HttpGet("obter-por-inscricao/{idInscricao}")]
        [Authorize("Bearer")]
        public ActionResult<DTOPedido?> ObterPorInscricao(int idInscricao)
        {
            var pedido = m_AppObtencao.ObterPorInscricao(idInscricao);
            return Ok(pedido);
        }

        [HttpPost("incluir")]
        [Authorize("Bearer")]
        public DTOResultadoPedido Incluir([FromBody] DTOPedidoInclusao dto)
        {
            return m_AppInclusao.Incluir(dto);
        }
    }
}
