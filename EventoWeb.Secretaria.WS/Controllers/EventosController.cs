using EventoWeb.Comum.Aplicacao.Eventos;
using EventoWeb.Comum.Negocio.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eventoweb_secretaria_back.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class EventosController : ControllerBase
    {
        private AppEventoListagem mAppListagem;

        public EventosController(AppEventoListagem appListagem)
        {
            mAppListagem = appListagem;
        }

        // GET api/eventos/obter-todos
        [HttpGet("obter-todos")]
        [Authorize("Bearer")]
        public ActionResult<IEnumerable<DTOEvento>> Get()
        {
            return Ok(mAppListagem.Listar(EnumFiltroListagemEventos.Todos));
        }

        /*
        // GET api/eventos/obter-id/5
        [HttpGet("obter-id/{id}")]
        [Authorize("Bearer")]
        public ActionResult<DTOEventoCompleto> Get(int id)
        {
            return mAppListagem.ObterPorId(id);
        }

        [HttpGet("obter-para-inscricao/{id}")]
        [Authorize("Bearer")]
        public ActionResult<DTOEventoCompletoInscricao> GetCompletoInscricao(int id)
        {
            return mAppListagem.ObterPorIdCompletoInscricao(id);
        }

        // POST api/eventos/incluir
        [HttpPost("incluir")]
        [Authorize("Bearer")]
        public DTOId Post([FromBody] DTOEvento eventoDTO)
        {
            return mAppListagem.Incluir(eventoDTO);
        }

        // PUT api/eventos/atualizar/5
        [HttpPut("atualizar/{id}")]
        [Authorize("Bearer")]
        public void Put(int id, [FromBody] DTOEvento eventoDTO)
        {
            mAppListagem.Atualizar(id, eventoDTO);
        }

        // DELETE api/eventos/excluir/5
        [HttpDelete("excluir/{id}")]
        [Authorize("Bearer")]
        public void Delete(int id)
        {
            mAppListagem.Excluir(id);
        }*/
    }
}
