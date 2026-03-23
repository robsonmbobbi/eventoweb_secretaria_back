using EventoWeb.Secretaria.Aplicacao.ContasBancarias;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventoWeb.Secretaria.WS.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class ContasBancariasController : ControllerBase
{
    private readonly AppContaBancariaListagem m_AppListagem;

    public ContasBancariasController(AppContaBancariaListagem appListagem)
    {
        m_AppListagem = appListagem ?? throw new ArgumentNullException(nameof(appListagem));
    }

    [HttpGet("listar")]
    [Authorize("Bearer")]
    public ActionResult<IEnumerable<DTOContaBancaria>> Listar()
    {
        return Ok(m_AppListagem.Listar());
    }
}
