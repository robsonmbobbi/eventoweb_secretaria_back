using EventoWeb.Secretaria.Aplicacao.Quartos;
using EventoWeb.Secretaria.Relatorios.Aplicacao.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventoWeb.Secretaria.WS.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class QuartosController : ControllerBase
    {
        private readonly AppDivisaoAutomaticaQuartos _appDivisao;
        private readonly AppRelatorioQuartos _appRelatorio;

        public QuartosController(AppDivisaoAutomaticaQuartos appDivisao, AppRelatorioQuartos appRelatorio)
        {
            _appDivisao = appDivisao ?? throw new ArgumentNullException(nameof(appDivisao));
            _appRelatorio = appRelatorio ?? throw new ArgumentNullException(nameof(appRelatorio));
        }

        /// <summary>
        /// Realiza a divisão automática de inscrições pelos quartos do evento.
        /// </summary>
        /// <param name="idEvento">ID do evento para o qual será realizada a divisão.</param>
        /// <returns>Lista de quartos com os inscritos atribuídos a cada um.</returns>
        [HttpPost("dividir-inscricoes/{idEvento}")]
        [Authorize("Bearer")]
        public ActionResult<List<DTOQuarto>> DividirInscricoes(int idEvento)
        {
            try
            {
                var resultado = _appDivisao.Dividir(idEvento);
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Erro ao realizar divisão de inscrições.", details = ex.Message });
            }
        }

        /// <summary>
        /// Gera relatório em PDF com a listagem de participantes agrupados por quarto.
        /// </summary>
        /// <param name="idEvento">ID do evento para o qual será gerado o relatório.</param>
        /// <param name="detalhar">Se true, inclui ID de inscrição, cidade e UF no relatório.</param>
        /// <returns>Arquivo PDF para download.</returns>
        [HttpGet("relatorio/{idEvento}")]
        [Authorize("Bearer")]
        public async Task<IActionResult> GerarRelatorioAsync(int idEvento, [FromQuery] bool detalhar = false)
        {
            try
            {
                var pdf = await _appRelatorio.GerarRelatorioAsync(idEvento, detalhar);
                return File(pdf, "application/pdf", $"Relatorio_Quartos_{idEvento}.pdf");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Erro ao gerar relatório de quartos.", details = ex.Message });
            }
        }
    }
}
