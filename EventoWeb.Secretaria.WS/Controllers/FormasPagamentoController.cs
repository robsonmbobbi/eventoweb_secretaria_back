using EventoWeb.Comum.Aplicacao.FormasPagamento;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eventoweb_secretaria_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormasPagamentoController (AppFormasPagamentoListagem appListagem) : ControllerBase
    {
        [HttpGet("listar/{idEvento}")]
        [Authorize("Bearer")]
        public IEnumerable<DTOFormaPagamento> Listar(int idEvento)
        {
            return appListagem.ListarTodas(idEvento);
        }

    }
}
