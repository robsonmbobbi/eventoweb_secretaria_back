using EventoWeb.Comum.Aplicacao.Inscricoes;
using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Secretaria.Aplicacao.Inscricoes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eventoweb_secretaria_back.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class InscricoesController(
        AppInscricaoListagem appListagem,
        AppInscricaoInclusao appInclusao,
        AppInscricaoAtualizacao appAtualizacao,
        AppInscricaoObtencao appObtencao,
        AppInscricaoPesquisaPessoa appPesquisa,
        AppInscricaoAtualizacaoSituacao appAtualizacaoSituacao) : ControllerBase
    {

        // GET api/eventos/obter-todos
        [HttpGet("listar/evento/{idEvento}/situacao/{situacao}")]
        [Authorize("Bearer")]
        public IEnumerable<DTOInscricaoListagem> Get(int idEvento, EnumSituacaoInscricao situacao)
        {
            return appListagem.Listar(idEvento, situacao);
        }

        [HttpPost("incluir")]
        [Authorize("Bearer")]
        public DTOInscricao Incluir([FromBody] DTOInscricao dto)
        {
            appInclusao.DtoInscricao = dto;
            return appInclusao.Incluir();
        }

        [HttpPut("atualizar")]
        [Authorize("Bearer")]
        public void Atualizar([FromBody] DTOInscricao dto)
        {
            appAtualizacao.DtoInscricao = dto;
            appAtualizacao.Atualizar();
        }

        [HttpGet("obter/{id:int}")]
        [Authorize("Bearer")]
        public DTOInscricao? Obter(int id)
        {
            return appObtencao.Obter(id);
        }

        [HttpGet("pesquisar/evento/{idEvento}/cpf/{cpf}")]
        [Authorize("Bearer")]
        public DTOInscricaoPesquisaPessoa Pesquisar(int idEvento, string cpf)
        {
            return appPesquisa.Pesquisar(idEvento, cpf);
        }

        [HttpPut("aceitar/{idInscricao}")]
        [Authorize("Bearer")]
        public void Aceitar(int idInscricao)
        {
            appAtualizacaoSituacao.Aceitar(idInscricao);
        }

        [HttpPut("rejeitar/{idInscricao}")]
        [Authorize("Bearer")]
        public void Rejeitar(int idInscricao)
        {
            appAtualizacaoSituacao.Rejeitar(idInscricao);
        }
    }
}
