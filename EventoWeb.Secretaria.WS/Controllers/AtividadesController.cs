using EventoWeb.Secretaria.Aplicacao.Atividades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventoWeb.Secretaria.WS.Controllers
{
    /// <summary>
    /// Controller responsável pelas operações relacionadas a atividades.
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AtividadesController : ControllerBase
    {
        private readonly AppDivisaoAutomaticaAtividades _appDivisao;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="AtividadesController"/>.
        /// </summary>
        /// <param name="appDivisao">Serviço de aplicação para divisão de atividades</param>
        public AtividadesController(AppDivisaoAutomaticaAtividades appDivisao)
        {
            _appDivisao = appDivisao ?? throw new ArgumentNullException(nameof(appDivisao));
        }

        /// <summary>
        /// Realiza a divisão automática de participantes em uma atividade do evento.
        /// </summary>
        /// <remarks>
        /// Os participantes são distribuídos de forma equilibrada entre as divisões da atividade,
        /// ordenados por data de nascimento (decrescente) e cidade (crescente).
        /// Coordenadores já atribuídos a uma divisão são mantidos.
        /// </remarks>
        /// <param name="idEvento">ID do evento para o qual será realizada a divisão</param>
        /// <param name="idAtividade">ID da atividade a ser dividida</param>
        /// <returns>Lista de divisões com os participantes atribuídos</returns>
        /// <response code="200">Divisão realizada com sucesso</response>
        /// <response code="400">Evento ou atividade não encontrados, ou atividade sem divisões</response>
        /// <response code="401">Usuário não autenticado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("dividir/{idEvento}/{idAtividade}")]
        //[Authorize("Bearer")]
        public ActionResult<List<DTODivisaoAtividade>> Dividir(int idEvento, int idAtividade)
        {
            try
            {
                var resultado = _appDivisao.Dividir(idEvento, idAtividade);
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Erro ao realizar divisão de atividade.", details = ex.Message });
            }
        }
    }
}
