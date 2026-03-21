using EventoWeb.Comum.Aplicacao.Precos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eventoweb_secretaria_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrecosInscricaoController(
    AppPrecoInscricaoObtencaoIdade appObtencaoIdade) : ControllerBase
{
    [HttpGet("evento/{idEvento}/obter/nascimento/{dataNascimento}")]
    [Authorize("Bearer")]
    public DTOPrecoInscricao? ObterPorIdade(int idEvento, DateTime dataNascimento)
    {
        return appObtencaoIdade.Obter(idEvento, dataNascimento);
    }
}
