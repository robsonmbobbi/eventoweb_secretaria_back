using EventoWeb.Secretaria.Negocio.Entidades.Atividades;
using EventoWeb.Secretaria.Relatorios.Aplicacao.Interfaces;
using EventoWeb.Secretaria.Relatorios.Aplicacao.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventoWeb.Secretaria.WS.Controllers
{
    [Produces("application/pdf")]
    [Route("api/[controller]")]
    [ApiController]
    public class RelatoriosDivisoesAtividadesController : ControllerBase
    {
        private readonly AppDivisaoAtividadeRelatorio _appRelatorio;

        public RelatoriosDivisoesAtividadesController(AppDivisaoAtividadeRelatorio appRelatorio)
        {
            _appRelatorio = appRelatorio ?? throw new ArgumentNullException(nameof(appRelatorio));
        }

        /// <summary>
        /// Gera relatório em PDF com as divisões de uma atividade.
        /// Cada divisão é exibida em uma página separada.
        /// </summary>
        /// <param name="idAtividade">ID da atividade.</param>
        /// <param name="detalhar">Se true, inclui ID de inscrição, cidade e UF dos participantes.</param>
        /// <returns>Arquivo PDF contendo o relatório.</returns>
        [HttpGet("gerar-pdf/{idAtividade}")]
        [Produces("application/pdf", "application/json")]
        //[Authorize("Bearer")]
        public async Task<ActionResult> GerarPdf(int idAtividade, [FromQuery] bool detalhar = false)
        {
            try
            {
                var pdf = await _appRelatorio.GerarRelatorioAsync(idAtividade, detalhar);
                
                return File(pdf, "application/pdf", $"relatorio-divisoes-atividade-{idAtividade}.pdf");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erro ao gerar relatório.", details = ex.Message });
            }
        }
    }
}
