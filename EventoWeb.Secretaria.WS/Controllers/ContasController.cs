using EventoWeb.Secretaria.Aplicacao.Contas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventoWeb.Secretaria.WS.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class ContasController : ControllerBase
{
    private readonly AppContaLiquidacao m_AppLiquidacao;

    public ContasController(AppContaLiquidacao appLiquidacao)
    {
        m_AppLiquidacao = appLiquidacao ?? throw new ArgumentNullException(nameof(appLiquidacao));
    }

    [HttpPost("liquidar")]
    [Authorize("Bearer")]
    public void Liquidar([FromBody] DTOLiquidacaoConta dto)
    {
        m_AppLiquidacao.Liquidar(dto);
    }
}
