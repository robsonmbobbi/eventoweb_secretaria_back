using EventoWeb.Secretaria.Aplicacao.Quartos;
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

        public QuartosController(AppDivisaoAutomaticaQuartos appDivisao)
        {
            _appDivisao = appDivisao ?? throw new ArgumentNullException(nameof(appDivisao));
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
    }
}
