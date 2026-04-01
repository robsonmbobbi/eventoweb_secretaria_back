using EventoWeb.Secretaria.Relatorios.Aplicacao.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventoWeb.Secretaria.WS.Controllers
{
    [Produces("application/pdf")]
    [Route("api/[controller]")]
    [ApiController]
    public class EtiquetasImpressaoController : ControllerBase
    {
        private readonly AppEtiquetasCracha _appCracha;
        private readonly AppEtiquetasCaderninho _appCaderninho;

        public EtiquetasImpressaoController(
            AppEtiquetasCracha appCracha,
            AppEtiquetasCaderninho appCaderninho)
        {
            _appCracha = appCracha ?? throw new ArgumentNullException(nameof(appCracha));
            _appCaderninho = appCaderninho ?? throw new ArgumentNullException(nameof(appCaderninho));
        }

        /// <summary>
        /// Gera etiquetas de crachá para todos os participantes aceitos no evento.
        /// </summary>
        /// <param name="idEvento">ID do evento.</param>
        /// <returns>Arquivo PDF contendo as etiquetas de crachá.</returns>
        [HttpGet("cracha/{idEvento}")]
        [Produces("application/pdf", "application/json")]
        //[Authorize("Bearer")]
        public ActionResult GerarEtiquetasCracha(int idEvento)
        {
            var pdf = _appCracha.GerarEtiquetasCracha(idEvento);
            return File(pdf, "application/pdf", $"etiquetas-cracha-{idEvento}.pdf");
        }

        /// <summary>
        /// Gera etiquetas de caderninho para participantes aceitos e infantis de 10 a 12 anos.
        /// </summary>
        /// <param name="idEvento">ID do evento.</param>
        /// <returns>Arquivo PDF contendo as etiquetas de caderninho.</returns>
        [HttpGet("caderninho/{idEvento}")]
        [Produces("application/pdf", "application/json")]
        //[Authorize("Bearer")]
        public ActionResult GerarEtiquetasCaderninho(int idEvento)
        {
            var pdf = _appCaderninho.GerarEtiquetasCaderninho(idEvento);
            return File(pdf, "application/pdf", $"etiquetas-caderninho-{idEvento}.pdf");
        }
    }
}
