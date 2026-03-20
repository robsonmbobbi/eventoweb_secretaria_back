using EventoWeb.Comum.Aplicacao.Inscricoes;
using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Aplicacao.Inscricoes
{
    public class AppInscricaoListagem : AppInscricaoBase
    {
        public AppInscricaoListagem(IContexto contexto, IInscricoes inscricoes) : base(contexto, inscricoes)
        {
        }

        public List<DTOInscricaoListagem> Listar(int idEvento, EnumSituacaoInscricao situacao)
        {
            var lista = new List<DTOInscricaoListagem>();

            ExecutarSeguramente(() =>
            {
                lista = [.. Inscricoes
                    .ListarPorSituacao(idEvento, situacao)
                    .Select(x => x.ConverterParaListagem())];
            });

            return lista;
        }
    }
}
